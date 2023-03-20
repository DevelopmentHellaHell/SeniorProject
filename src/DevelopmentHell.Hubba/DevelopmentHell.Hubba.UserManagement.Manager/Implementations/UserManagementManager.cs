using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.UserManagement.Manager.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.UserManagement.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;

namespace DevelopmentHell.Hubba.UserManagement.Manager.Implementations
{
    public class UserManagementManager : IUserManagementManager
    {
        IAuthorizationService _authorizationService;
        ILoggerService _loggerService;
        IRegistrationService _registrationService;
        IUserManagementService _userManagementService;
        IValidationService _validationService;

        public UserManagementManager(IAuthorizationService authorizationService, ILoggerService loggerService, IRegistrationService registrationService, IUserManagementService userManagementService, IValidationService validationService) 
        { 
            _authorizationService = authorizationService;
            _loggerService = loggerService;
            _registrationService = registrationService;
            _userManagementService = userManagementService;
            _validationService = validationService;
        }

        public async Task<Result> ElevatedCreateAccount(string email, string passphrase, string accountType, string? firstName, string? lastName, string? userName)
        {
            if (!_authorizationService.Authorize(new[] { "AdminUser" }).IsSuccessful)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = "Unauthorized Access"
                };
            }
            var regResult = await _registrationService.RegisterAccount(email, passphrase, accountType).ConfigureAwait(false);
            if (!regResult.IsSuccessful)
            {
                return new Result
                {
                    IsSuccessful = false,
                    ErrorMessage = "Error in Creating account with Elevated permissions:"+regResult.ErrorMessage,
                };
            }

            if (firstName != null || lastName != null || userName != null)
            {
                var updResult = await _userManagementService.SetNames(email, firstName, lastName, userName).ConfigureAwait(false);
                if (!updResult.IsSuccessful)
                {
                    return new Result
                    {
                        IsSuccessful = false,
                        ErrorMessage = "Error in Updating newly created account with Elevated Permissions:"+updResult.ErrorMessage,
                    };
                }
            }
            return new Result { IsSuccessful = true };
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
            if (!_authorizationService.Authorize(new[] { "AdminUser" }).IsSuccessful)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = "Unauthorized Access"
                };
            }
            var validateResult = _validationService.ValidateEmail(email);
            if (!validateResult.IsSuccessful)
            {
                return new()
                {
                    ErrorMessage = validateResult.ErrorMessage,
                    IsSuccessful = false
                };
            }
            var disableResult = await _userManagementService.DisableAccount(email).ConfigureAwait(false);
            if (!disableResult.IsSuccessful)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = disableResult.ErrorMessage,
                };
            }
            return new()
            {
                IsSuccessful = true
            };
        }

        public async Task<Result> ElevatedEnableAccount(string email)
        {
            if (!_authorizationService.Authorize(new[] { "AdminUser" }).IsSuccessful)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = "Unauthorized Access"
                };
            }
            var validateResult = _validationService.ValidateEmail(email);
            if (!validateResult.IsSuccessful)
            {
                return new()
                {
                    ErrorMessage = validateResult.ErrorMessage,
                    IsSuccessful = false
                };
            }
            var enableResult = await _userManagementService.EnableAccount(email).ConfigureAwait(false);
            if (!enableResult.IsSuccessful)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = enableResult.ErrorMessage,
                };
            }
            return new()
            {
                IsSuccessful = true
            };
        }

        public async Task<Result> ElevatedUpdateAccount(string email, Dictionary<string, object> data)
        {
            throw new NotImplementedException();
        }
    }
}
