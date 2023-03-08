﻿using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Manager.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace DevelopmentHell.Hubba.Registration.Manager.Implementations
{
    public class RegistrationManager : IRegistrationManager
    {
        private IRegistrationService _registrationService;
		private IAuthorizationService _authorizationService;
        private ICryptographyService _cryptographyService;
		private ILoggerService _loggerService;
        
        public RegistrationManager(IRegistrationService registrationService, IAuthorizationService authorizationService, ICryptographyService cryptographyService, ILoggerService loggerService)
        {
            _registrationService = registrationService;
            _authorizationService = authorizationService;
            _cryptographyService = cryptographyService;
            _loggerService = loggerService;
        }

        public async Task<Result> ElevatedCreateAccount(string email, string passphrase, string firstName, string lastName, string accountType)
        {
            Result result = new Result();

            if (!_authorizationService.Authorize(new string[] { "AdminUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, user already logged in.";
                return result;
            }

            Result registerResult = await Register(email, passphrase, accountType, true).ConfigureAwait(false);
            if (!registerResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = registerResult.ErrorMessage;
                return result;
            }
        }

        public async Task<Result> Register(string email, string password, string accountType = "VerifiedUser", bool bypass = false)
        {
            Result result = new Result();

			if (!bypass && _authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Error, user already logged in.";
				return result;
			}

			Result registerResult = await _registrationService.RegisterAccount(email, password, accountType).ConfigureAwait(false);
            if (!registerResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = registerResult.ErrorMessage;
                return result;
            }

            string userHashKey = ConfigurationManager.AppSettings["UserHashKey"]!;
            Result<HashData> userHashResult = _cryptographyService.HashString(email, userHashKey);
            if (!userHashResult.IsSuccessful || userHashResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, unexpected error. Please contact system administrator.";
                return result;
            }

            string userHash = Convert.ToBase64String(userHashResult.Payload.Hash!);
            _loggerService.Log(LogLevel.INFO, Category.BUSINESS, $"New registered user: {email}.", userHash);

            result.IsSuccessful = true;
            return result;
        }
    }
}