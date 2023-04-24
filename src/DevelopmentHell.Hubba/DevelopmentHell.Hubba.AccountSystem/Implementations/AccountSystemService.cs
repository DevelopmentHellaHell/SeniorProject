using DevelopmentHell.Hubba.AccountSystem.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;

namespace DevelopmentHell.Hubba.AccountSystem.Implementations
{
    public class AccountSystemService : IAccountSystemService
    {
        private IUserAccountDataAccess _userAccountDataAccess;
        private ILoggerService _loggerService;

        public AccountSystemService(IUserAccountDataAccess userAccountDataAccess, ILoggerService loggerService)
        {
            _userAccountDataAccess = userAccountDataAccess;
            _loggerService = loggerService;
        }
        //TODO: Remember to write what needs to be checked in this layer
        public async Task<Result> UpdateEmailInformation(int userId, string newEmail)
        {
            Result result = new Result();
            return await _userAccountDataAccess.SaveEmailAlterations(userId, newEmail);
        }

        public async Task<Result<PasswordInformation>> GetPasswordData(int userId)
        {
            return await _userAccountDataAccess.GetPasswordData(userId);
        }

        public async Task<Result> UpdatePassword(string newHashPassword, string email)
        {
            Result result = new Result();
            return await _userAccountDataAccess.SavePassword(newHashPassword, email);
        }

        public async Task<Result> UpdateUserName(int userId, string? firstName, string? lastName)
        {
            Result result = new Result();
            if (firstName == null && lastName == null) //MOVE
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Please enter a valid name for First Name and/or Last Name. ";
                return result;
            }
            Result updateResult = await _userAccountDataAccess.UpdateUserName(userId, firstName!, lastName!);
            if (!updateResult.IsSuccessful) 
            {
                result.IsSuccessful = false;
                result.ErrorMessage = updateResult.ErrorMessage;
                return result;
            }
            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result<AccountSystemSettings>> GetAccountSettings(int userId)
        {
            return await _userAccountDataAccess.GetAccountSettings(userId).ConfigureAwait(false);
        }

        public async Task<Result<int>> CheckNewEmail(string newEmail)
        {
            return await _userAccountDataAccess.GetId(newEmail).ConfigureAwait(false);
        }

    }
}
