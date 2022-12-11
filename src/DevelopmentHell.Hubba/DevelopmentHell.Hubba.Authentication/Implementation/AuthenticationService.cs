using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Validation.Service;

namespace DevelopmentHell.Hubba.Authentication.Service.Implementation
{
	public class AuthenticationService : IAuthenticationService
	{
		private IUserAccountDataAccess _dao;
		private ILoggerService _loggerService;
		public AuthenticationService(IUserAccountDataAccess dao, ILoggerService loggerService)
		{
			_dao = dao;
			_loggerService = loggerService;
		}

		public async Task<Result<int>> AuthenticateCredentials(string email, string password, string ipAddress)
		{
			Result<int> result = new Result<int>();

			HashData hashData = HashService.HashString(password).Payload!;
			Result<int> getIdFromEmail = await _dao.GetId(email).ConfigureAwait(false);
			Result<int> getIdFromCredentialsResult = await _dao.GetId(email, hashData).ConfigureAwait(false);
			if (!getIdFromEmail.IsSuccessful || !getIdFromCredentialsResult.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Error, please contact system administrator.";
				return result;
			}

			int accountIdFromEmail = getIdFromEmail.Payload;
			int accountIdFromCredentials = getIdFromCredentialsResult.Payload;
			// Wrong Credentials
			if (accountIdFromCredentials == 0)
			{
				// Valid Email
				if (accountIdFromEmail != 0)
				{
					DateTime currentTime = DateTime.UtcNow;
					Result<UserAccount> getAttemptResult = await _dao.GetAttempt(accountIdFromEmail).ConfigureAwait(false);
					UserAccount? loginAttemptData = getAttemptResult.Payload;
					if (!getAttemptResult.IsSuccessful || loginAttemptData is null)
					{
						result.IsSuccessful = false;
						result.ErrorMessage = "Error, please contact system administrator.";
						return result;
					}

					int loginAttempts = (int)loginAttemptData.LoginAttempts!;
					DateTime? activeFailureTime = loginAttemptData.FailureTime is null ? null : DateTime.Parse(loginAttemptData.FailureTime!.ToString()!);

					_loggerService.Log(LogLevel.INFO, Category.BUSINESS, "AuthenticationService.AuthenticateCredentials", $"{ipAddress} attempted to log in to {email} using the wrong password. (Attempt {loginAttempts + 1})");

					// Current time is greater than stored time
					// Reset login attempts
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

					Result updateResult = await _dao.Update(new UserAccount()
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
				result.ErrorMessage = "The email or password provided is invalid.";
				return result;
			}

			if (!ValidationService.ValidateEmail(email).IsSuccessful ||
				!ValidationService.ValidatePassword(password).IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "The email or password provided is invalid.";
				return result;
			}

			Result<bool> getDisabledResult = await _dao.GetDisabled(accountIdFromCredentials).ConfigureAwait(false);
			if (!getDisabledResult.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Error, please contact system administrator.";
				return result;
			}

			if (getDisabledResult.Payload)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "This account has temporarily been disabled.";
				return result;
			}
			
			Result updateResult1 = await _dao.Update(new UserAccount()
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
			result.Payload = accountIdFromCredentials;
			return result;
		}
	}
}