using DevelopmentHell.Hubba.AccountRecovery.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Validation.Service;

namespace DevelopmentHell.Hubba.AccountRecovery.Service.Implementation
{
    public class AccountRecoveryService : IAccountRecoveryService
    {
        private IUserAccountDataAccess _userAccountDao;
        private ILoggerService _loggerService;
        private IUserLoginDataAccess _userLoginDao;
        private IRecoveryRequestDataAccess _recoveryRequestDao;

        public AccountRecoveryService(IUserAccountDataAccess userAccountDao, ILoggerService loggerService, IUserLoginDataAccess userLoginDao, IRecoveryRequestDataAccess recoveryRequestDao)
        {
            _userAccountDao = userAccountDao;
            _loggerService = loggerService;
            _userLoginDao = userLoginDao;
            _recoveryRequestDao = recoveryRequestDao;
        }

        public async Task<Result<int>> Verification(string email)
        {
            Result<int> result = new Result<int>();

            if (!ValidationService.ValidateEmail(email).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Invalid username or password provided. Retry again or contact system admin";
                return result;
            }

            Result<int> getIdFromEmail = await _userAccountDao.GetId(email).ConfigureAwait(false);
            if (!getIdFromEmail.IsSuccessful && String.IsNullOrEmpty(getIdFromEmail.Payload.ToString()))
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Invalid username or password provided. Retry again or contact system admin";
                return result;
            }
            result.Payload = getIdFromEmail.Payload;
            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result<string>> CompleteRecovery(int accountId, string ipAddress)
        {
            Result<string> result = new Result<string>();


            Result<string[]> getIpAddress = await _userLoginDao.GetIPAddress(accountId).ConfigureAwait(false);
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
                Result addManualRecovery = await _recoveryRequestDao.AddManualRecovery(accountId).ConfigureAwait(false);
                if (!addManualRecovery.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = addManualRecovery.ErrorMessage;
                    return result;
                }
                result.IsSuccessful = true;
                result.Payload = "Added to manual recovery";
                return result;
            }
            else
            {
                Result enableAccount = await _userAccountDao.Update(new UserAccount { Id = accountId, Disabled = false, LoginAttempts = 0 }).ConfigureAwait(false);
                if (!enableAccount.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = enableAccount.ErrorMessage;
                    return result;
                }
                result.IsSuccessful = true;
                result.Payload = "Successfully enabled account";
                return result;
            }
        }

        public bool CheckIpAddress(string[]? successfulIpAddress, string ipAddress)
        {
            if (successfulIpAddress is null) {
                Console.WriteLine("No IPs given");
                return false;
            }
            for (int i = 0; i < successfulIpAddress.Length; i++)
            {
                if ((successfulIpAddress[i].Trim()).Equals(ipAddress.Trim()))
                {
                    Console.WriteLine("Matching IP Found");
                    return true;
                }
            }
            Console.WriteLine("No IPs match");
            return false;
        }
    }
}
