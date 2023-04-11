using DevelopmentHell.Hubba.AccountDeletion.Manager.Abstraction;
using DevelopmentHell.Hubba.AccountDeletion.Service.Abstractions;
using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using System.Security.Claims;

namespace DevelopmentHell.Hubba.AccountDeletion.Manager.Implementations
{
    public class AccountDeletionManager : IAccountDeletionManager
    {
        private IAccountDeletionService _accountDeletionService;
        private ILoggerService _loggerService;
        private IAuthorizationService _authorizationService;
        private IAuthenticationService _authenticationService;

        public AccountDeletionManager(IAccountDeletionService accountDeletionService, IAuthenticationService authenticationService, IAuthorizationService authorizationService, ILoggerService loggerService)
        {
            _accountDeletionService = accountDeletionService;
            _loggerService = loggerService;
            _authorizationService = authorizationService;
            _authenticationService = authenticationService;
        }

        public async Task<Result<string>> DeleteAccount(int accountId)
        {
            Result<string> result = new Result<string>();
            if (Thread.CurrentPrincipal is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, user is not logged in";
                return result;
            }

            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;

            // Get the ID of current thread
            string thisAccountIDStr = principal.FindFirstValue("sub");

            if (int.TryParse(thisAccountIDStr, out int thisAccountIDInt))
            {
                //TODO: GOLD PLATING notify other users affiliated with this account
                // Result<List<Dictionary<string, object>>> listingsBookingsResult = await _accountDeletionService.GetListingsBookings(accountId).ConfigureAwait(false);

                if (_authorizationService.Authorize(new string[] { "VerifiedUser" }).IsSuccessful)
                {
                    // this is a verified user who is deleting someone else's account
                    if (thisAccountIDInt != accountId)
                    {
                        result.IsSuccessful = false;
                        result.ErrorMessage = "Error, cannot delete selected user";
                        return result;
                    }
                    // this is a verified user who is deleting their own account

                    Result deletionResult = await DeleteAccountAndLog(accountId);
                    if (!deletionResult.IsSuccessful)
                    {
                        result.IsSuccessful = false;
                        result.ErrorMessage = deletionResult.ErrorMessage;
                        return result;
                    }
                    // log the user out before deleting their account
                    Result<string> logoutResult = _authenticationService.Logout();
                    result.Payload = logoutResult.Payload!;

                    // TODO: GOLD PLATING notify affected listings and bookings
                    //Result notifyResult = await _accountDeletionService.NotifyListingsBookings(accountId, listingsBookingsResult.payload).ConfigureAwait(false);
                    //if (!notifyResult.IsSuccessful)
                    //{
                    //    Result logRes = _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, $"Unable to notify affected accounts of account deletion. Deleted ID: {accountID}", null);
                    //}
                    result.IsSuccessful = true;
                    return result;
                }

                // The user is an admin
                else if (_authorizationService.Authorize(new string[] { "AdminUser" }).IsSuccessful)
                {
                    // The user is trying to delete itself
                    if (thisAccountIDInt == accountId)
                    {
                        Result<int> countAdminResult = await _accountDeletionService.CountAdmin().ConfigureAwait(false);
                        if (countAdminResult.IsSuccessful)
                        {
                            // check to make sure there are other admin accounts first
                            if (countAdminResult.Payload == 1)
                            {
                                result.IsSuccessful = false;
                                result.ErrorMessage = "Error, cannot delete last Admin account";
                                return result;
                            }
                        }
                    }
                    Result deletionResult = await DeleteAccountAndLog(accountId).ConfigureAwait(false);
                    if (!deletionResult.IsSuccessful)
                    {
                        result.IsSuccessful = false;
                        result.ErrorMessage = deletionResult.ErrorMessage;
                        return result;
                    }
                    // TODO: GOLD PLATING notify affected listings and bookings
                    //Result notifyResult = await _accountDeletionService.NotifyListingsBookings(accountId, listingsBookingsResult.payload).ConfigureAwait(false);
                    //if (!notifyResult.IsSuccessful)
                    //{
                    //    Result logRes = _loggerService.Log(Models.LogLevel.ERROR, Category.BUSINESS, $"Unable to notify affected accounts of account deletion. Deleted ID: {accountID}", null);
                    //}

                    if (thisAccountIDInt == accountId)
                    {
                        Result<string> logoutResult = _authenticationService.Logout();
                    }

                    result.IsSuccessful = true;
                    return result;
                }
                else
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = "Error, current user role is not of correct type.";
                    return result;
                }
            }
            else
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, account ID of current user is not of correct type. Expected type: Int";
                return result;
            }
        }
        private async Task<Result> DeleteAccountAndLog(int accountID)
        {
            Result deletionResult = await _accountDeletionService.DeleteAccount(accountID).ConfigureAwait(false);
            if (deletionResult.IsSuccessful)
            {
                Result logRes = _loggerService.Log(Models.LogLevel.INFO, Category.BUSINESS, $"Account has been deleted. ID: {accountID}", null);
            }

            return deletionResult;
        }
    }

}