using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using System.Security.Claims;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;

namespace DevelopmentHell.Hubba.Authorization.Service.Implementations
{
	public class AuthorizationService : IAuthorizationService
	{
		private readonly string _jwtKey;
		private IUserAccountDataAccess _userAccountDataAccess;
		private ICryptographyService _cryptographyService;
		private ILoggerService _loggerService;

        public AuthorizationService(string jwtKey, IUserAccountDataAccess userAccountDataAccess, ICryptographyService cryptographyService, ILoggerService loggerService)
		{
			_jwtKey = jwtKey;
			_userAccountDataAccess = userAccountDataAccess;
			_cryptographyService = cryptographyService;
			_loggerService = loggerService;
		}

        public Result Authorize(string[] roles)
		{
			Result result = new Result()
			{
				IsSuccessful = false,
			};

			var principal = Thread.CurrentPrincipal as ClaimsPrincipal;

			if (principal is null)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Unauthorized.";
				return result;
			}
			
			if (principal is not null)
			{
				foreach (string role in roles)
				{
					if (principal.IsInRole(role))
					{
						result.IsSuccessful = principal.IsInRole(role);
						return result;
					}
				}
			}

			result.IsSuccessful = false;
			result.ErrorMessage = "Unauthorized.";
			return result;
		}

		public async Task<Result<string>> GenerateAccessToken(int accountId, bool defaultUser = false)
		{
			var result = new Result<string>();

			var getResult = await _userAccountDataAccess.GetUser(accountId).ConfigureAwait(false);
			if (!getResult.IsSuccessful || getResult.Payload is null)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Invalid email.";
				return result;
			}

			var role = getResult.Payload.Role;
			var email = getResult.Payload.Email;
			if (role is null || email is null)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Empty user.";
				return result;
			}

			try
			{
				var header = new Dictionary<string, object>()
				{
					{ "alg", "HS256" },
					{ "typ", "JWT" }
				};
				var payload = new Dictionary<string, object>()
				{
					{ "iss", "Hubba" },
					{ "aud", "*" },
					{ "azp", email },
					{ "sub", accountId },
					{ "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
					{ "exp", DateTimeOffset.UtcNow.AddMinutes(defaultUser ? 2 : 30).ToUnixTimeSeconds() },
					{ "role", defaultUser ? "DefaultUser" : "VerifiedUser" }
				};

				string headerJson = _cryptographyService.SerializeToJson(header);
				string headerBase64 = _cryptographyService.EncodeBase64(headerJson);
				string payloadJson = _cryptographyService.SerializeToJson(payload);
				string payloadBase64 = _cryptographyService.EncodeBase64(payloadJson);

				string unsignedToken = string.Format("{0}.{1}", headerBase64, payloadBase64);
				string signature = _cryptographyService.SignToken(unsignedToken, _jwtKey);
				string jwtToken = string.Format("{0}.{1}", unsignedToken, signature);

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