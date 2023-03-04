using DevelopmentHell.Hubba.Cryptography.Service;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.AccountDeletion.Service.Abstractions;
using DevelopmentHell.Hubba.AccountDeletion.Manager.Abstraction;
using DevelopmentHell.Hubba.Authentication.Manager.Abstractions;
using System.Security.Claims;

namespace DevelopmentHell.Hubba.AccountDeletion.Manager.Implementation
{
    public class AccountDeletionManager : IAccountDeletionManager
    {
        private IAccountDeletionService _accountDeletionService;
        private ILoggerService _loggerService;
        private IAuthorizationService _authorizationService;
        private IAuthenticationManager _authenticationManager;
        public AccountDeletionManager(IAccountDeletionService accountDeletionService,IAuthenticationManager authenticationManager, IAuthorizationService authorizationService, ILoggerService loggerService)
        {
            _accountDeletionService = accountDeletionService;
            _loggerService = loggerService;
            _authorizationService = authorizationService;
            _authenticationManager = authenticationManager;
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
                if (_authorizationService.authorize(new string[] { "VerifiedUser" }).IsSuccessful)
                {
                    // this is a verified user who is deleting someone else's account
                    if (thisAccountIDInt != accountId)
                    {
                        result.IsSuccessful = false;
                        result.ErrorMessage = "Error, cannot delete selected user";
                        return result;
                    }
                    // this is a verified user who is deleting their own account
                    Result deletionResult = await DeleteAccountNotifyListingsBookings(accountId);
                    if (!deletionResult.IsSuccessful)
                    {
                        result.IsSuccessful = false;
                        result.ErrorMessage = deletionResult.ErrorMessage;
                        return result;
                    }
                    // log the user out before deleting their account
                    _authenticationManager.Logout();
                    return deletionResult;
                }

                // The user is an admin
                else if (_authorizationService.authorize(new string[] { "AdminUser" }).IsSuccessful)
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
                    Result deletionResult = await DeleteAccountNotifyListingsBookings(accountId);
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
        private async Task<Result> DeleteAccountNotifyListingsBookings(int accountID)
        {     // The user is deleting an account
            Result result = new();
            Result deletionResult = await _accountDeletionService.DeleteAccountNotifyListingsBookings(accountID).ConfigureAwait(false);
            //if (!deletionResult.IsSuccessful)
            //{
            //    result.IsSuccessful = false;
            //    result.ErrorMessage = deletionResult.ErrorMessage;
            //    return result;
            //}
            Result logRes = _loggerService.Log(Models.LogLevel.INFO, Category.BUSINESS, "AccountDelectionManager.DeleteAccount", $"Account has been deleted. ID: {accountID}");

            return deletionResult;
        }
    }

}