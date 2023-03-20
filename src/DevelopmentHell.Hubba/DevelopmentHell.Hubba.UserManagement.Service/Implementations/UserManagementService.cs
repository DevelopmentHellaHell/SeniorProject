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

        public async Task<Result> DisableAccount(string email)
        {
            var getResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            if (!getResult.IsSuccessful)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = "Unable to get Id to Disable account: " + getResult.ErrorMessage
                };
            }
            var setResult = await _userAccountDataAccess.SetEnabledStatus(getResult.Payload!, false).ConfigureAwait(false);
            if (!setResult.IsSuccessful)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = "Unable to Disable account: " + getResult.ErrorMessage
                };
            }
            return setResult;
        }

        public async Task<Result> EnableAccount(string email)
        {
            var getResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            if (!getResult.IsSuccessful)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = "Unable to get Id to Enable account: " + getResult.ErrorMessage
                };
            }
            var setResult = await _userAccountDataAccess.SetEnabledStatus(getResult.Payload!, true).ConfigureAwait(false);
            if (!setResult.IsSuccessful)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = "Unable to Enable account: " + getResult.ErrorMessage
                };
            }
            return setResult;
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
        public async Task<Result> UpdateNames(string email, Dictionary<string, object> data)
        {
            var getResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            if (!getResult.IsSuccessful)
            {
                return getResult;
            }
            return await _userNamesDataAccess.Update(getResult.Payload, data).ConfigureAwait(false);
        }

        public Task<Result> UpdateAccount(string email, Dictionary<string, object> data)
        {
            throw new NotImplementedException();
        }
    }
}
