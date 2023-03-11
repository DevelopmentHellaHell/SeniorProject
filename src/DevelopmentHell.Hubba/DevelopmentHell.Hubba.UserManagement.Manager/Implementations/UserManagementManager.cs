﻿using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.UserManagement.Manager.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.UserManagement.Service.Abstractions;

namespace DevelopmentHell.Hubba.UserManagement.Manager.Implementations
{
    public class UserManagementManager : IUserManagementManager
    {
        IAuthorizationService _authorizationService;
        ILoggerService _loggerService;
        IRegistrationService _registrationService;
        IUserManagementService _userManagementService;

        public UserManagementManager(IAuthorizationService authorizationService, ILoggerService loggerService, IRegistrationService registrationService, IUserManagementService userManagementService) 
        { 
            _authorizationService = authorizationService;
            _loggerService = loggerService;
            _registrationService = registrationService;
            _userManagementService = userManagementService;
        }

        public async Task<Result> ElevatedCreateAccount(string email, string passphrase, string accountType, string? firstName, string? lastName, string? userName)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> ElevatedDeleteAccount(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> ElevatedDeleteAccountNotifyListingsBookings(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> ElevatedDisableAccount(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> ElevatedEnableAccount(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> ElevatedUpdateAccount(string email, Dictionary<string, object> data)
        {
            throw new NotImplementedException();
        }
    }
}
