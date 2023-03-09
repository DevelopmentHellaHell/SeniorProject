using System.Configuration;
using System.Security.Claims;
using System.Text;
using Development.Hubba.JWTHandler.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Implementations;
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
		private readonly string _cryptographyKey;
		private IUserAccountDataAccess _userAccountDataAccess;
		private IUserLoginDataAccess _userLoginDataAccess;
        private ICryptographyService _cryptographyService;
		private IJWTHandlerService _jwtHandlerService;
		private IValidationService _validationService;
        private ILoggerService _loggerService;

		public AuthenticationService(string cryptographyKey, IUserAccountDataAccess userAccountDataAccess, IUserLoginDataAccess userLoginDataAccess, ICryptographyService cryptographyService, IJWTHandlerService jWTHandlerService, IValidationService validationService, ILoggerService loggerService)
		{
			_cryptographyKey = cryptographyKey;
			_userAccountDataAccess = userAccountDataAccess;
			_userLoginDataAccess = userLoginDataAccess;
			_cryptographyService = cryptographyService;
			_jwtHandlerService = jWTHandlerService;
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

        public Result<string> Logout()
        {
			Result<string> result = new Result<string>();
            result.IsSuccessful = true;
			result.Payload = _jwtHandlerService.GenerateInvalidToken();
			return result;
		}

		public Result<string> GenerateIdToken(int accountId, string accessToken)
		{
			Result<string> result = new Result<string>();

			try
			{
				Result<HashData> hashData = _cryptographyService.HashString(accessToken, _cryptographyKey);
				if (!hashData.IsSuccessful || hashData.Payload is null)
				{
					result.IsSuccessful = false;
					result.Payload = "Unable to generate access token.";
					return result;
				}

				string accessTokenHash = Convert.ToBase64String(hashData.Payload.Hash!);

				var header = new Dictionary<string, object>()
				{
					{ "alg", "HS256" },
					{ "typ", "JWT" }
				};
				var payload = new Dictionary<string, object>()
				{
					{ "iss", "Hubba" },
					{ "aud", "*" },
					{ "sub", accountId },
					{ "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
					{ "exp", DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds() },
					{ "at_hash", accessTokenHash }
				};

				string jwtToken = _jwtHandlerService.GenerateToken(header, payload);

				result.IsSuccessful = true;
				result.Payload = jwtToken;
				return result;
			}
			catch
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Unable to generate access token.";
				return result;
			}
		}
	}
}