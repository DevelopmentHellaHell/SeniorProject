using DevelopmentHell.Hubba.AccountSystem.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;

namespace DevelopmentHell.Hubba.AccountSystem.Implementations
{
    public class AccountSystemService : IAccountSystemService
    {
        private IAccountSystemDataAccess _accountSystemDataAccess;

        public AccountSystemService(IAccountSystemDataAccess accountSystemDataAccess)
        {
            _accountSystemDataAccess = accountSystemDataAccess;
        }
        //TODO: Remember to write what needs to be checked in this layer
        public async Task<Result> UpdateEmailInformation(string email, string newEmail)
        {
            Result result = new Result();
            return await _accountSystemDataAccess.SaveEmailAlterations(email, newEmail);
        }

        public async Task<Result<List<Dictionary<string, object>>>> GetPasswordData(int userId)
        {
            Result result = new Result();
            return await _accountSystemDataAccess.GetPasswordData(userId); ;
        }

        public async Task<Result> UpdatePassword(string newHashPassword, string email)
        {
            Result result = new Result();
            return await _accountSystemDataAccess.SavePassword(newHashPassword, email);
        }

        public async Task<Result> UpdateUserName(int userId, string firstName, string lastName)
        {
            Result result = new Result();
            return await _accountSystemDataAccess.UpdateUserName(userId, firstName, lastName);
        }

        public async Task<Result<List<Dictionary<string, object>>>> GetAccountSettings(int userId)
        {
            return await _accountSystemDataAccess.GetAccountSettings(userId);
        }
    }
}
