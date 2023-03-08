using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.AccountDeletion.Service.Abstractions;
using DevelopmentHell.Hubba.AccountDeletion.Manager.Abstraction;
using System.Security.Claims;
using DevelopmentHell.Hubba.Authentication.Service.Abstractions;

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

        public async Task<Result> DeleteAccount(int accountId)
        {
            Result result = new Result();
            if (Thread.CurrentPrincipal is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, user is not logged in";
                return result;
            }

            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;

            // Get the ID of current thread
            string thisAccountIDStr = principal.FindFirstValue("accountId");

            if (int.TryParse(thisAccountIDStr, out int thisAccountIDInt))
            {
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

                    //TODO: GOLD PLATING notify other users affiliated with this account
                    //Result<List<Dictionary<string, object>>> listingsBookingsResult = await _accountDeletionService.GetListingsBookings(accountId).ConfigureAwait(false);

                    Result deletionResult = await DeleteAccountAndLog(accountId);
                    if (!deletionResult.IsSuccessful)
                    {
                        result.IsSuccessful = false;
                        result.ErrorMessage = deletionResult.ErrorMessage;
                        return result;
                    }
                    // log the user out before deleting their account
                    _authenticationService.Logout();

                    return deletionResult;
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
                            if (countAdminResult.Payload == 1)
                            {
                                result.IsSuccessful = false;
                                result.ErrorMessage = "Error, cannot delete last Admin account";
                                return result;
                            }
                        }
                    }
                    // Admin deleting any other account than their own
                    Result deletionResult = await DeleteAccountAndLog(accountId);
                    if (!deletionResult.IsSuccessful)
                    {
                        result.IsSuccessful = false;
                        result.ErrorMessage = deletionResult.ErrorMessage;
                        return result;
                    }

                    return deletionResult;
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