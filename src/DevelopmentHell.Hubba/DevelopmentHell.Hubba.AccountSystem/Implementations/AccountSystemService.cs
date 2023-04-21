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
        public async Task<Result> UpdateEmailInformation(string email, string newEmail, string newSalt, string newHashPassword)
        {
            Result result = new Result();
            return await _accountSystemDataAccess.SaveEmailAlterations(email, newEmail, newSalt, newHashPassword);
        }

    }
}
