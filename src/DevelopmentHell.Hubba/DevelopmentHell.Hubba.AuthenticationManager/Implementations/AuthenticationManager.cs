using DevelopmentHell.Hubba.Authentication.Manager.Abstractions;
using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword.Service.Abstractions;
using System.Configuration;
using System.Security.Claims;
using System.Security.Principal;

namespace DevelopmentHell.Hubba.Authentication.Manager.Implementations
{
    public class AuthenticationManager : IAuthenticationManager
	{
        private IAuthenticationService _authenticationService;
        private IOTPService _otpService;
        private IAuthorizationService _authorizationService;
        private ILoggerService _loggerService;

        public AuthenticationManager(IAuthenticationService authenticationService, IOTPService otpService, IAuthorizationService authorizationService, ILoggerService loggerService)
        {
            _authenticationService = authenticationService;
            _otpService = otpService;
            _authorizationService = authorizationService;
            _loggerService = loggerService;
        }
        
        public async Task<Result<string>> Login(string email, string password, string ipAddress, bool enabledSend = true)
        {
            Result<string> result = new();

            if (_authorizationService.authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
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

            Result sendOTPResult = _otpService.SendOTP(email, otp, enabledSend);
            if (!sendOTPResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = sendOTPResult.ErrorMessage;
                return result;
            }

            string userHashKey = ConfigurationManager.AppSettings["UserHashKey"]!;
            Result<HashData> userHashResult = HashService.HashString(email, userHashKey);
            if (!userHashResult.IsSuccessful || userHashResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, unexpected error. Please contact system administrator.";
                return result;
            }

            string userHash = Convert.ToBase64String(userHashResult.Payload.Hash!);
            _loggerService.Log(LogLevel.INFO, Category.BUSINESS, $"Successful login attempt from: {email}.", userHash);

			return await _authorizationService.GenerateToken(accountId, true).ConfigureAwait(false);
        }

        public async Task<Result<string>> AuthenticateOTP(string otp, string ipAddress)
        {

            Result<string> result = new();

            if (_authorizationService.authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, user already logged in.";
                return result;
            }

			var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
			var stringAccountId = claimsPrincipal?.FindFirstValue("accountId");
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

            return await _authorizationService.GenerateToken(accountId).ConfigureAwait(false);
        }
    }
}