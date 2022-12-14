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
		private UserSessionDataAccess _userSessionDataAccess;
		public AuthenticationService(IUserAccountDataAccess dao, ILoggerService loggerService, UserSessionDataAccess userSessionDataAccess)
		{
			_dao = dao;
			_loggerService = loggerService;
			_userSessionDataAccess = userSessionDataAccess;
		}

		public async Task<Result<int>> AuthenticateCredentials(string email, string password, string ipAddress)
		{
			Result<int> result = new Result<int>();

			if (!ValidationService.ValidateEmail(email).IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Invalid username or password provided. Retry again or contact system admin.";
				return result;
			}

			Result<UserAccount> userHashData = await _dao.GetHashData(email).ConfigureAwait(false);
			UserAccount payload = userHashData.Payload!;
			if (!userHashData.IsSuccessful || payload is null)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Invalid username or password provided. Retry again or contact system admin.";
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
						_loggerService.Log(LogLevel.WARNING, Category.BUSINESS, "AuthenticationService.AuthenticateCredentials", "Failure attempt did not complete successfully.");
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

		public async Task<Result<AuthTicket>> CreateSession(int accountId)
        {
			//TODO: update later, arbitrary right now
			return await CreateSession(accountId, DateTime.UtcNow.AddYears(1));
		}

		public async Task<Result<AuthTicket>> CreateSession(int accountId, DateTime expiration)
		{
			Result<AuthTicket> output = new();


            Result deleteResult = await _userSessionDataAccess.DeleteUserSessions(accountId);
            if (!deleteResult.IsSuccessful)
            {
                output.ErrorMessage = deleteResult.ErrorMessage;
                return output;
            }

			Result insertResult = await _userSessionDataAccess.InsertSession(accountId, expiration, DateTime.UtcNow);
			if (!insertResult.IsSuccessful)
			{
				output.ErrorMessage = insertResult.ErrorMessage;
				return output;
			}

            Result<List<AuthTicket>> getResult = await _userSessionDataAccess.GetUserSessions(accountId).ConfigureAwait(false);
            if (!getResult.IsSuccessful || getResult.Payload is null)
            {
                output.ErrorMessage = getResult.ErrorMessage;
                return output;
            }
            if (getResult.Payload.Count != 1)
            {
                output.ErrorMessage = "Unexpected number of Sessions found for account associated with given account ID";
                return output;
            }

			output.Payload = new()
			{
				SessionId = getResult.Payload[0].SessionId,
				AccountId = getResult.Payload[0].AccountId,
				Expiration = getResult.Payload[0].Expiration,
				LastActivity = getResult.Payload[0].LastActivity,
				Self = EncryptionService.Encrypt(AuthTicketConversionService.ToBytes(getResult.Payload[0]))
			};

			Result updateResult = await _userSessionDataAccess.UpdateSession(output.Payload).ConfigureAwait(false);
            if (!updateResult.IsSuccessful)
            {
                output.ErrorMessage = updateResult.ErrorMessage;
                return output;
            }

			await _userSessionDataAccess.PruneSessions().ConfigureAwait(false);

			output.IsSuccessful = true;
			return output;
        }

		public async Task<Result<AuthTicket>> RenewSession(AuthTicket ticket)
		{
			Result<AuthTicket> output = new() { IsSuccessful = false };

			AuthTicket copy = ticket;
			copy.LastActivity = DateTime.UtcNow;
			//TODO update along with other magic number
			copy.Expiration = DateTime.UtcNow.AddYears(1);
			copy.Self = EncryptionService.Encrypt(AuthTicketConversionService.ToBytes(copy));

            Result updateResult = await _userSessionDataAccess.UpdateSession(copy).ConfigureAwait(false);
            if (!updateResult.IsSuccessful)
            {
                output.ErrorMessage = updateResult.ErrorMessage;
                return output;
            }

			output.IsSuccessful = true;
			output.Payload = copy;
			return output;
        }
		public async Task<Result<bool>> ValidateSession(AuthTicket ticket)
		{
			Result<bool> output = new() { IsSuccessful=false };

			AuthTicket storedTicket = new();

			if (ticket.Self is null)
			{
				output.ErrorMessage = "Given ticket, the self property is null.";
				return output;
			}

			try
			{
				storedTicket = AuthTicketConversionService.FromBytes(Cryptography.Service.EncryptionService.DecryptToBytes(ticket.Self));
            } catch (Exception e)
			{
				output.ErrorMessage = $"Unhandled exception during decryption of marshalled AuthCookieTicket: {e}";
				return output;
			}

			AuthTicket copy = ticket;
			copy.Self = null;

			Result<List<AuthTicket>> getResult = await _userSessionDataAccess.GetUserSessions(ticket.AccountId).ConfigureAwait(false);
			if (!getResult.IsSuccessful || getResult.Payload is null)
			{
				output.ErrorMessage = getResult.ErrorMessage;
				return output;
			}
			if (getResult.Payload.Count != 1)
			{
                output.ErrorMessage = "Unexpected number of users found for given account ID";
                return output;
            }

            output.IsSuccessful = true;
            if (getResult.Payload[0].Self != copy.Self || !storedTicket.Equals(copy))
			{
                output.ErrorMessage = "Evidence of tampering with ticket values";
                output.Payload = false;
                return output;
            }

            if (DateTime.UtcNow > storedTicket.Expiration)
			{
				output.ErrorMessage = "Session Expired";
				output.Payload = false;
				return output;
			}

			output.Payload = true;
			return output;
		}
	}
}