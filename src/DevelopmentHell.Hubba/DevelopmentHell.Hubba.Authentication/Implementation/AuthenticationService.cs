using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Validation.Service;
using System.Security.Principal;

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

			if (!ValidationService.ValidateEmail(email).IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Invalid username or password provided. Retry again or contact system admin";
				return result;
			}

			Result<UserAccount> userHashData = await _dao.GetHashData(email).ConfigureAwait(false);
			UserAccount payload = userHashData.Payload!;
			if (!userHashData.IsSuccessful || payload is null)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Invalid username or password provided. Retry again or contact system admin";
				return result;
			}

			Result<HashData> hashData = HashService.HashString(password, payload.PasswordSalt!);
			var oldHash = payload.PasswordHash;
			var newHash = Convert.ToBase64String(hashData.Payload!.Hash!);
			Result<int> getIdFromEmail = await _dao.GetId(email).ConfigureAwait(false);

			int accountIdFromEmail = getIdFromEmail.Payload;

			// Wrong Password
			if (oldHash != newHash)
			{
				// Valid Email
				if (accountIdFromEmail != 0)
				{
					DateTime currentTime = DateTime.UtcNow;
					Result<UserAccount> getAttemptResult = await _dao.GetAttempt(accountIdFromEmail).ConfigureAwait(false);
					UserAccount? loginAttemptData = getAttemptResult.Payload;
					if (!getAttemptResult.IsSuccessful || loginAttemptData is null)
					{
						_loggerService.Log(LogLevel.WARNING, Category.BUSINESS, "Failure attempt did not complete successfully.");
						result.IsSuccessful = false;
						result.ErrorMessage = "Error, please contact system administrator.";
						return result;
					}

					int loginAttempts = (int)loginAttemptData.LoginAttempts!;
					DateTime? activeFailureTime = loginAttemptData.FailureTime is null ? null : DateTime.Parse(loginAttemptData.FailureTime!.ToString()!);

					_loggerService.Log(LogLevel.INFO, Category.BUSINESS, $"{ipAddress} attempted to log in to {email} using the wrong password. (Attempt {loginAttempts + 1})");

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
				result.ErrorMessage = "Invalid username or password provided. Retry again or contact system admin";
				return result;
			}

			Result<bool> getDisabledResult = await _dao.GetDisabled(accountIdFromEmail).ConfigureAwait(false);
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
			result.Payload = accountIdFromEmail;
			return result;
		}

		public Result<GenericPrincipal> CreateSession(int accountId)
		{
			Result<GenericPrincipal> result = new Result<GenericPrincipal>();

			var identity = new GenericIdentity(accountId.ToString());
			var principal = new GenericPrincipal(identity, new string[] { "VerifiedUser" });

			Thread.CurrentPrincipal = principal;

			result.IsSuccessful = true;
			result.Payload = principal;
			return result;
		}
	}
}