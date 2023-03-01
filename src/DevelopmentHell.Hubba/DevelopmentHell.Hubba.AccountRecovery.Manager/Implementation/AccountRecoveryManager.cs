using System.Security.Principal;
using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword.Service.Abstractions;
using DevelopmentHell.Hubba.AccountRecovery.Service.Abstractions;
using DevelopmentHell.Hubba.AccountRecovery.Manager.Abstractions;
using System.Security.Claims;

namespace DevelopmentHell.Hubba.AccountRecovery.Manager.Implementation
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

        public async Task<Result<string>> Verification(string email, bool enabledSend = true)
        {
            Result<string> result = new();


            if (_authorizationService.authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, user already logged in.";
                return result;
            }

            Result<int> userIdResult = await _accountRecoveryService.Verification(email).ConfigureAwait(false)!;
            if (!userIdResult.IsSuccessful && string.IsNullOrEmpty(userIdResult.Payload.ToString()))
            {
                result.IsSuccessful = false;
                result.ErrorMessage = userIdResult.ErrorMessage;
                return result;
            }
            int accountId = userIdResult.Payload;

            Result<string> otpResult = await _otpService.NewOTP(accountId).ConfigureAwait(false);
            string otp = otpResult.Payload!.ToString();

            Result sendOTPResult = _otpService.SendOTP(email, otp, enabledSend);
            if (!sendOTPResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = sendOTPResult.ErrorMessage;
                return result;
            }
            return await _authorizationService.GenerateToken(accountId, true).ConfigureAwait(false); ;
        }

        public async Task<Result<bool>> AuthenticateOTP(string otp, string ipAddress)
        {

            Result<bool> result = new Result<bool>()
            {
                IsSuccessful = false,
            };

            if (_authorizationService.authorize(new string[] { "VerifiedUser", "AdminUser" }).IsSuccessful)
            {
                _loggerService.Log(LogLevel.INFO, Category.BUSINESS, $"{ipAddress} failed OTP authentication.");
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
                result.ErrorMessage = "Invalid username or OTP provided. Retry again or contact system admin";
                return result;
            }
            result.IsSuccessful = true;
            result.Payload = true;
            return result;
        }

        public async Task<Result<string>> AccountAccess(string ipAddress)
        {
            Result<string> result = new Result<string>()
            {
                IsSuccessful = false,
            };

            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("accountId");
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
                return await _authorizationService.GenerateToken(accountId).ConfigureAwait(false);
            }

            return result;

        }
    }
}
