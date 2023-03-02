using DevelopmentHell.Hubba.AccountRecovery.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Validation.Service;

namespace DevelopmentHell.Hubba.AccountRecovery.Service.Implementation
{
    public class AccountRecoveryService : IAccountRecoveryService
    {
        private IUserAccountDataAccess _userAccountDataAccess;
        private IUserLoginDataAccess _userLoginDataAccess;
        private IRecoveryRequestDataAccess _recoveryRequestDataAccess;
        private ILoggerService _loggerService;

        public AccountRecoveryService(IUserAccountDataAccess userAccountDataAccess, IUserLoginDataAccess userLoginDataAccess, IRecoveryRequestDataAccess recoveryRequestDataAccess, ILoggerService loggerService)
        {
            _userAccountDataAccess = userAccountDataAccess;
            _userLoginDataAccess = userLoginDataAccess;
            _recoveryRequestDataAccess = recoveryRequestDataAccess;
            _loggerService = loggerService;
        }

        public async Task<Result<int>> Verification(string email)
        {
            Result<int> result = new Result<int>();

            if (!ValidationService.ValidateEmail(email).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Invalid email. Retry again or contact system admin";
                return result;
            }

            Result<int> getIdFromEmail = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            if (!getIdFromEmail.IsSuccessful || getIdFromEmail.Payload == 0)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Invalid email. Retry again or contact system admin";
                return result;
            }
            result.Payload = getIdFromEmail.Payload;
            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result<bool>> CompleteRecovery(int accountId, string ipAddress)
        {
            Result<bool> result = new Result<bool>();


            Result<string[]> getIpAddress = await _userLoginDataAccess.GetIPAddress(accountId).ConfigureAwait(false);
            if (!getIpAddress.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Could not retreive IP address for given account.";
                return result;
            }
            string[] successfulIpAddress = getIpAddress.Payload!;

            bool matchingIpAddress = CheckIpAddress(successfulIpAddress, ipAddress);

            if (!matchingIpAddress)
            {
                Result<int> selectAccount = await _recoveryRequestDataAccess.GetId(accountId).ConfigureAwait(false);
                if (selectAccount.IsSuccessful && selectAccount.Payload != 0)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = "Request already exists.";
                    return result;
                }

                Result addManualRecovery = await _recoveryRequestDataAccess.AddManualRecovery(accountId).ConfigureAwait(false);
                if (!addManualRecovery.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = "Fatal error. Please contact system adminstrator.";
                    return result;
                }
                result.IsSuccessful = true;
                result.Payload = false;
                return result;
            }
            else
            {
                Result enableAccount = await _userAccountDataAccess.Update(new UserAccount { Id = accountId, Disabled = false, LoginAttempts = 0 }).ConfigureAwait(false);
                if (!enableAccount.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = "Fatal error. Please contact system adminstrator.";
                    return result;
                }
                result.IsSuccessful = true;
                result.Payload = true;
                return result;
            }
        }

        public bool CheckIpAddress(string[]? successfulIpAddress, string ipAddress)
        {
            if (successfulIpAddress is null) {
                //Console.WriteLine("No IPs given");
                return false;
            }
            for (int i = 0; i < successfulIpAddress.Length; i++)
            {
                if ((successfulIpAddress[i].Trim()).Equals(ipAddress.Trim()))
                {
                    //Console.WriteLine("Matching IP Found");
                    return true;
                }
            }
            //Console.WriteLine("No IPs match");
            return false;
        }
    }
}
