﻿using DevelopmentHell.Hubba.AccountDeletion.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.UserManagement.Manager.Abstractions;
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
        IAccountDeletionService _accountDeletionService;
        IUserAccountDataAccess _userAccountDataAccess;

        public UserManagementManager(IAuthorizationService authorizationService, ILoggerService loggerService, IRegistrationService registrationService, IUserManagementService userManagementService, IValidationService validationService, IAccountDeletionService accountDeletionService, IUserAccountDataAccess userAccountDataAccess)
        {
            _authorizationService = authorizationService;
            _loggerService = loggerService;
            _registrationService = registrationService;
            _userManagementService = userManagementService;
            _validationService = validationService;
            _accountDeletionService = accountDeletionService;
            _userAccountDataAccess = userAccountDataAccess;
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
                    ErrorMessage = "Error in Creating account with Elevated permissions:" + regResult.ErrorMessage,
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
                        ErrorMessage = "Error in Updating newly created account with Elevated Permissions:" + updResult.ErrorMessage,
                    };
                }
            }
            return new Result { IsSuccessful = true };
        }

        public async Task<Result> ElevatedDeleteAccount(string email)
        {
            if (!_authorizationService.Authorize(new[] { "AdminUser" }).IsSuccessful)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = "Unauthorized Access"
                };
            }
            var getResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            if (!getResult.IsSuccessful || getResult.Payload == 0)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = "Unable to get Id to delete Account: " + getResult.ErrorMessage,
                };
            }
            var delResult = await _accountDeletionService.DeleteAccount(getResult.Payload!).ConfigureAwait(false);
            if (!delResult.IsSuccessful)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = "Unable To delete Account: " + delResult.ErrorMessage,
                };
            }
            return delResult;
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
            if (!_authorizationService.Authorize(new[] { "AdminUser" }).IsSuccessful)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = "Unauthorized Access"
                };
            }
            HashSet<string> names = new(new string[] { "FirstName", "LastName", "UserName" });
            HashSet<string> accepted = new(new string[] { "Role", "CellPhoneNumber", "CellPhoneProvider" });
            HashSet<string> acceptedRoles = new(new[] { "AdminUser", "VerifiedUser" });
            Dictionary<string, object> updatedNames = new();
            Dictionary<string, object> updatedAccount = new();
            foreach (var pair in data)
            {
                if (names.Contains(pair.Key))
                {
                    updatedNames.Add(pair.Key, pair.Value);
                }
                else if (accepted.Contains(pair.Key))
                {
                    if (pair.Key == "Email")
                    {
                        var valResult = _validationService.ValidateEmail((string)pair.Value);
                        if (!valResult.IsSuccessful)
                        {
                            continue;
                        }
                    }
                    else if (pair.Key == "Role" && !acceptedRoles.Contains(pair.Value))
                    {
                        continue;
                    }
                    else if (pair.Key == "CellPhoneNumber")
                    {
                        var valResult = _validationService.ValidatePhoneNumber((string)pair.Value);
                        if (!valResult.IsSuccessful)
                        {
                            continue;
                        }
                    }
                    updatedAccount.Add(pair.Key, pair.Value);
                }
            }
            if (updatedNames.Count > 0)
            {
                var nameResult = await _userManagementService.UpdateNames(email, updatedNames).ConfigureAwait(false);
                if (!nameResult.IsSuccessful)
                {
                    return new()
                    {
                        IsSuccessful = false,
                        ErrorMessage = "Unable to update names: " + nameResult.ErrorMessage
                    };
                }
            }
            if (updatedAccount.Count > 0)
            {
                var updResult = await _userManagementService.UpdateAccount(email, updatedAccount).ConfigureAwait(false);
                if (!updResult.IsSuccessful)
                {
                    return new()
                    {
                        IsSuccessful = false,
                        ErrorMessage = "Unable to update account: " + updResult.ErrorMessage
                    };
                }
            }
            return new() { IsSuccessful = true };
        }
    }
}