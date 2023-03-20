using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.UserManagement.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;

namespace DevelopmentHell.Hubba.UserManagement.Service.Implementations
{
    public class UserManagementService : IUserManagementService
    {
        private readonly ILoggerService _loggerService;
        private IUserAccountDataAccess _userAccountDataAccess;
        private IUserNamesDataAccess _userNamesDataAccess;
        public UserManagementService(ILoggerService loggerService, IUserAccountDataAccess userAccountDataAccess, IUserNamesDataAccess userNamesDataAccess)
        {
            _loggerService = loggerService;
            _userAccountDataAccess = userAccountDataAccess;
            _userNamesDataAccess = userNamesDataAccess;
        }

        public Task<Result> CreateAccount(string email, string passphrase, string role, string? firstName, string? lastName, string? userName)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DisableAccount(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> EnableAccount(int accountId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> SetNames(string email, string? firstName, string? lastName, string? userName)
        {
            var getResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            if (!getResult.IsSuccessful)
            {
                return getResult;
            }
            return await _userNamesDataAccess.Insert(getResult.Payload, firstName, lastName, userName).ConfigureAwait(false);
        }

        public Task<Result> UpdateAccount(string email, Dictionary<string, object> data)
        {
            throw new NotImplementedException();
        }
    }
}
