using DevelopmentHell.Hubba.Cryptography.Service;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.AccountDeletion.Service.Abstractions;
using System.Configuration;
using System.Security.Principal;
using System.Text;
using DevelopmentHell.Hubba.Authorization.Service.Implementation;
using Microsoft.Identity.Client;

namespace DevelopmentHell.Hubba.AccountDeletion.Manager
{
    public class AccountDeletionManager
    {
        private IAccountDeletionService _accountDeletionService;
        private ILoggerService _loggerService;
        private IAuthorizationService _authorizationService;
        public AccountDeletionManager(IAccountDeletionService accountDeletionService, IAuthorizationService authorizationService, ILoggerService loggerService)
        {
            _accountDeletionService = accountDeletionService;
            _loggerService = loggerService;
            _authorizationService = authorizationService;
        }

        public async Task<Result> DeleteAccount(int accountID, IPrincipal? principal = null)
        {
            Result result = new Result();
            if (Thread.CurrentPrincipal is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, user is not logged in";
                return result;
            }

            if (principal is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, user is not logged in";
                return result;
            }

            if (Thread.CurrentPrincipal.Identity is null || Thread.CurrentPrincipal.Identity.Name is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, no identity associated with calling user";
                return result;
            }

            // Get the ID of current thread
            string thisAccountIDStr = Thread.CurrentPrincipal.Identity.Name;

            if (int.TryParse(thisAccountIDStr, out int thisAccountIDInt))
            {
                if (_authorizationService.authorize(principal, new string[] { "VerifiedUser" }).IsSuccessful)
                {
                    // current thread is a verified user who is deleting someone else's account
                    if (thisAccountIDInt != accountID)
                    {
                        result.IsSuccessful = false;
                        result.ErrorMessage = "Error, cannot delete selected user";
                        return result;
                    }
                    // this thread is a verified user who is deleting their own account
                    Result deletionResult = await DeleteAccountNotifyListingsBookings(accountID);
                    if (!deletionResult.IsSuccessful)
                    {
                        result.IsSuccessful = false;
                        result.ErrorMessage = deletionResult.ErrorMessage;
                        return result;
                    }
                    return deletionResult;
                }

                // The user is an admin
                else if (_authorizationService.authorize(principal, new string[] { "Admin" }).IsSuccessful)
                {
                    // The user is trying to delete itself
                    if (thisAccountIDInt == accountID)
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
                    Result deletionResult = await DeleteAccountNotifyListingsBookings(accountID);
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