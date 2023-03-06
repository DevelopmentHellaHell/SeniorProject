﻿using System.Configuration;
using System.Security.Claims;
using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;

namespace DevelopmentHell.Hubba.Authentication.Service.Implementations
{
    public class AuthenticationService : IAuthenticationService
	{
		private IUserAccountDataAccess _userAccountDataAccess;
		private IUserLoginDataAccess _userLoginDataAccess;
        private ICryptographyService _cryptographyService;
		private IValidationService _validationService;
        private ILoggerService _loggerService;

		public AuthenticationService(IUserAccountDataAccess userAccountDataAccess, IUserLoginDataAccess userLoginDataAccess, ICryptographyService cryptographyService, IValidationService validationService, ILoggerService loggerService)
		{
			_userAccountDataAccess = userAccountDataAccess;
			_userLoginDataAccess = userLoginDataAccess;
			_cryptographyService = cryptographyService;
			_validationService = validationService;
            _loggerService = loggerService;
		}

		public async Task<Result<int>> AuthenticateCredentials(string email, string password, string ipAddress)
		{
			Result<int> result = new Result<int>();

			if (!_validationService.ValidateEmail(email).IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Invalid email or password provided. Retry again or contact system admin";
				return result;
			}

			Result<UserAccount> userHashData = await _userAccountDataAccess.GetHashData(email).ConfigureAwait(false);
			UserAccount payload = userHashData.Payload!;
			if (!userHashData.IsSuccessful || payload is null)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Invalid email or password provided. Retry again or contact system admin";
				return result;
			}

			Result<HashData> hashData = _cryptographyService.HashString(password, payload.PasswordSalt!);
			var oldHash = payload.PasswordHash;
			var newHash = Convert.ToBase64String(hashData.Payload!.Hash!);
			Result<int> getIdFromEmail = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);

			int accountIdFromEmail = getIdFromEmail.Payload;

			// Wrong Password
			if (oldHash != newHash)
			{
				// Valid Email
				if (accountIdFromEmail != 0)
				{
					DateTime currentTime = DateTime.Now;
					Result<UserAccount> getAttemptResult = await _userAccountDataAccess.GetAttempt(accountIdFromEmail).ConfigureAwait(false);
					UserAccount? loginAttemptData = getAttemptResult.Payload;
					if (!getAttemptResult.IsSuccessful || loginAttemptData is null)
					{
						_loggerService.Log(Models.LogLevel.WARNING, Category.BUSINESS, "Failure attempt did not complete successfully.");
						result.IsSuccessful = false;
						result.ErrorMessage = "Error, please contact system administrator.";
						return result;
					}

					int loginAttempts = (int)loginAttemptData.LoginAttempts!;
					DateTime? activeFailureTime = loginAttemptData.FailureTime is null ? null : DateTime.Parse(loginAttemptData.FailureTime!.ToString()!);

					_loggerService.Log(Models.LogLevel.INFO, Category.BUSINESS, $"{ipAddress} attempted to log in to {email} using the wrong password. (Attempt {loginAttempts + 1})");

					// Current time is greater than stored time
					// Reset login attempts as long as activeFailureTime is greater than 1 day
					if (activeFailureTime is not null && currentTime.CompareTo(activeFailureTime) > 0)
					{
						loginAttempts = 0;
					}

					object? failureTime = null;
					bool disabled = false;
					// First failed login attempt
					if (loginAttempts == 0)
					{
						failureTime = currentTime.AddDays(1); // TODO: move to config
					}
					if (loginAttempts + 1 >= 3)
					{
						disabled = true;
					}

					Result updateResult = await _userAccountDataAccess.Update(new UserAccount()
					{
						Id = accountIdFromEmail,
						FailureTime = failureTime,
						LoginAttempts = loginAttempts + 1,
						Disabled = disabled,
					}).ConfigureAwait(false);
					if (!updateResult.IsSuccessful)
					{
						result.IsSuccessful = false;
						result.ErrorMessage = "Error, please contact system administrator.";
						return result;
					}
				}
				result.IsSuccessful = false;
				result.ErrorMessage = "Invalid email or password provided. Retry again or contact system admin";
				return result;
			}

			Result<bool> getDisabledResult = await _userAccountDataAccess.GetDisabled(accountIdFromEmail).ConfigureAwait(false);
			if (!getDisabledResult.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Error, please contact system administrator.";
				return result;
			}

			if (getDisabledResult.Payload)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Account disabled. Perform account recovery or contact system admin.";
				return result;
			}

			Result updateResult1 = await _userAccountDataAccess.Update(new UserAccount()
			{
				Id = accountIdFromEmail,
				FailureTime = DBNull.Value,
				LoginAttempts = 0,
			}).ConfigureAwait(false);
			if (!updateResult1.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Error, please contact system administrator.";
				return result;
			}

			result.IsSuccessful = true;
			result.Payload = accountIdFromEmail;
			return result;
		}

		public async Task<Result> RegisterIpAddress(int accoundId, string ipAddress)
		{
			Result result = new Result();

			Result registerIpAddress = await _userLoginDataAccess.AddLogin(accoundId, ipAddress).ConfigureAwait(false);
			if (!registerIpAddress.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Address already exists.";
				return result;
			}

			result.IsSuccessful = true;
			return result; ;
		}

        public Result Logout()
        {
            Result result = new Result();

            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var email = principal.FindFirstValue(ClaimTypes.Email);
            if (email is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, unexpected error. Please context system administrator.";
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
            _loggerService.Log(LogLevel.INFO, Category.BUSINESS, $"Successful logout attempt from: {email}.", userHash);

            result.IsSuccessful = true;
            return result;
        }
    }
}