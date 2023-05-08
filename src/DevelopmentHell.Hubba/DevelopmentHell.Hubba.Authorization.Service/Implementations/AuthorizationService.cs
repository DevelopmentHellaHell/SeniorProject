using System.Security.Claims;
using Development.Hubba.JWTHandler.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;

namespace DevelopmentHell.Hubba.Authorization.Service.Implementations
{
    public class AuthorizationService : IAuthorizationService
    {
        private IUserAccountDataAccess _userAccountDataAccess;
        private IJWTHandlerService _jwtHandlerService;
        private ILoggerService _loggerService;

        public AuthorizationService(IUserAccountDataAccess userAccountDataAccess, IJWTHandlerService jwtHandlerService, ILoggerService loggerService)
        {
            _userAccountDataAccess = userAccountDataAccess;
            _jwtHandlerService = jwtHandlerService;
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
                if (roles.Contains(principal.FindFirstValue("role")))
                {
                    result.IsSuccessful = true;
                    return result;
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
                    { "role", defaultUser ? "DefaultUser" : role }
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