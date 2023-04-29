using DevelopmentHell.Hubba.Authentication.Manager.Abstractions;
using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword.Service.Abstractions;
using System.Configuration;
using System.Security.Claims;

namespace DevelopmentHell.Hubba.Authentication.Manager.Implementations
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private IAuthenticationService _authenticationService;
        private IOTPService _otpService;
        private IAuthorizationService _authorizationService;
        private ICryptographyService _cryptographyService;
        private ILoggerService _loggerService;

        public AuthenticationManager(IAuthenticationService authenticationService, IOTPService otpService, IAuthorizationService authorizationService, ICryptographyService cryptographyService, ILoggerService loggerService)
        {
            _authenticationService = authenticationService;
            _otpService = otpService;
            _authorizationService = authorizationService;
            _cryptographyService = cryptographyService;
            _loggerService = loggerService;
        }

        public async Task<Result<string>> Login(string email, string password, string ipAddress)
        {
            Result<string> result = new();

            if (_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, user already logged in.";
                return result;
            }

            Result<int> authenticateResult = await _authenticationService.AuthenticateCredentials(email, password, ipAddress).ConfigureAwait(false);
            if (!authenticateResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = authenticateResult.ErrorMessage;
                return result;
            }

            int accountId = authenticateResult.Payload;
            Result<string> otpResult = await _otpService.NewOTP(accountId).ConfigureAwait(false);
            string otp = otpResult.Payload!.ToString();

            Result sendOTPResult = _otpService.SendOTP(email, otp);
            if (!sendOTPResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = sendOTPResult.ErrorMessage;
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
            _loggerService.Log(LogLevel.INFO, Category.BUSINESS, $"Successful login attempt from: {email}.", userHash);

            return await _authorizationService.GenerateAccessToken(accountId, true).ConfigureAwait(false);
        }

        public async Task<Result<Tuple<string, string>>> AuthenticateOTP(string otp, string ipAddress)
        {

            Result<Tuple<string, string>> result = new();

            if (_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, user already logged in.";
                return result;
            }

            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format.";
                return result;
            }
            var accountId = int.Parse(stringAccountId);

            Result resultCheck = await _otpService.CheckOTP(accountId, otp).ConfigureAwait(false);
            if (!resultCheck.IsSuccessful)
            {
                _loggerService.Log(LogLevel.INFO, Category.BUSINESS, $"{ipAddress} failed OTP authentication.");
                result.IsSuccessful = false;
                result.ErrorMessage = "Invalid or expired OTP, please try again.";
                return result;
            }

            Result registerIpAddress = await _authenticationService.RegisterIpAddress(accountId, ipAddress).ConfigureAwait(false);
            if (!registerIpAddress.IsSuccessful)
            {
                // do nothing
            }

            Result<string> authorizationTokenResult = await _authorizationService.GenerateAccessToken(accountId).ConfigureAwait(false);
            string? accessToken = authorizationTokenResult.Payload;
            if (!authorizationTokenResult.IsSuccessful || accessToken is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error during the authentication process.";
                return result;
            }

            Result<string> authenticationTokenResult = _authenticationService.GenerateIdToken(accountId, accessToken);
            string? idToken = authenticationTokenResult.Payload;
            if (!authenticationTokenResult.IsSuccessful || idToken is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error during the authentication process.";
                return result;
            }

            result.IsSuccessful = true;
            result.Payload = new Tuple<string, string>(accessToken, idToken);
            return result;
        }

        public Result<string> Logout()
        {
            Result<string> result = new Result<string>();

            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var email = principal.FindFirstValue("azp");
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

            return _authenticationService.Logout();
        }
    }
}