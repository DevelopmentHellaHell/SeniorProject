using System.Configuration;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Notification.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Manager.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;

namespace DevelopmentHell.Hubba.Registration.Manager.Implementations
{
    public class RegistrationManager : IRegistrationManager
    {
        private IRegistrationService _registrationService;
        private IAuthorizationService _authorizationService;
        private ICryptographyService _cryptographyService;
        private INotificationService _notificationService;
        private ILoggerService _loggerService;

        public RegistrationManager(IRegistrationService registrationService, IAuthorizationService authorizationService, ICryptographyService cryptographyService, INotificationService notificationService, ILoggerService loggerService)
        {
            _registrationService = registrationService;
            _authorizationService = authorizationService;
            _cryptographyService = cryptographyService;
            _notificationService = notificationService;
            _loggerService = loggerService;
        }

        public async Task<Result> Register(string email, string password)
        {
            Result result = new Result();

            if (_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, user already logged in.";
                return result;
            }

            Result registerResult = await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            if (!registerResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = registerResult.ErrorMessage;
                return result;
            }

            Result<int> userResult = await _notificationService.GetId(email).ConfigureAwait(false);
            if (userResult.IsSuccessful)
            {
                Result notificationSettingsResult = await _notificationService.CreateNewNotificationSettings(userResult.Payload).ConfigureAwait(false);
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