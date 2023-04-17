using DevelopmentHell.Hubba.AccountRecovery.Manager.Abstractions;
using DevelopmentHell.Hubba.AccountRecovery.Service.Abstractions;
using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword.Service.Abstractions;
using System.Security.Claims;

namespace DevelopmentHell.Hubba.AccountRecovery.Manager.Implementations
{

    public class AccountRecoveryManager : IAccountRecoveryManager
    {
        private IAccountRecoveryService _accountRecoveryService;
        private IOTPService _otpService;
        private IAuthenticationService _authenticationService;
        private IAuthorizationService _authorizationService;
        private ILoggerService _loggerService;

        public AccountRecoveryManager(IAccountRecoveryService accountRecoveryService, IOTPService otpService, IAuthenticationService authenticationService, IAuthorizationService authorizationService, ILoggerService loggerService)
        {
            _accountRecoveryService = accountRecoveryService;
            _otpService = otpService;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _loggerService = loggerService;
        }

        public async Task<Result<string>> EmailVerification(string email)
        {
            Result<string> result = new Result<string>();


            if (_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, user already logged in.";
                return result;
            }

            Result<int> userIdResult = await _accountRecoveryService.Verification(email).ConfigureAwait(false)!;
            if (!userIdResult.IsSuccessful || string.IsNullOrEmpty(userIdResult.Payload.ToString()))
            {
                result.IsSuccessful = false;
                result.ErrorMessage = userIdResult.ErrorMessage;
                return result;
            }
            int accountId = userIdResult.Payload;

            Result<string> otpResult = await _otpService.NewOTP(accountId).ConfigureAwait(false);
            string otp = otpResult.Payload!.ToString();

            Result sendOTPResult = _otpService.SendOTP(email, otp);
            if (!sendOTPResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = sendOTPResult.ErrorMessage;
                return result;
            }

            return await _authorizationService.GenerateAccessToken(accountId, true).ConfigureAwait(false); ;
        }

        public async Task<Result<bool>> AuthenticateOTP(string otp, string ipAddress)
        {

            Result<bool> result = new Result<bool>()
            {
                IsSuccessful = false,
            };

            if (_authorizationService.Authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                _loggerService.Log(LogLevel.INFO, Category.BUSINESS, $"{ipAddress} failed OTP authentication.");
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
                result.ErrorMessage = "Invalid or expired OTP, please try again.";
                return result;
            }
            result.IsSuccessful = true;
            result.Payload = true;
            return result;
        }

        public async Task<Result<Tuple<string, string>>> AccountAccess(string ipAddress)
        {
            Result<Tuple<string, string>> result = new Result<Tuple<string, string>>()
            {
                IsSuccessful = false,
            };

            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format.";
                return result;
            }
            var accountId = int.Parse(stringAccountId);

            Result<bool> ipAddressResult = await _accountRecoveryService.CompleteRecovery(accountId, ipAddress).ConfigureAwait(false);
            if (!ipAddressResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = ipAddressResult.ErrorMessage;
                return result;
            }

            if (ipAddressResult.Payload == false)
            {
                result.IsSuccessful = true;
                result.Payload = null;
                return result;
            }

            if (ipAddressResult.Payload == true)
            {
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

            return result;
        }

        public Result Logout()
        {
            return _authenticationService.Logout();
        }
    }
}
