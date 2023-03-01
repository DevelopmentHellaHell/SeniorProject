using System.Security.Principal;
using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword.Service.Abstractions;
using DevelopmentHell.Hubba.AccountRecovery.Service.Abstractions;
using DevelopmentHell.Hubba.AccountRecovery.Manager.Abstractions;

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

        public async Task<Result<int>> Verification(string email, IPrincipal? principal = null, bool enabledSend = true)
        {
            Result<int> result = new();


            if (_authorizationService.authorize(principal, new string[] { "VerifiedUser", "Admin" }).IsSuccessful)
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
            result.IsSuccessful = true;
            result.Payload = accountId;
            return result;
        }

        public async Task<Result<bool>> AuthenticateOTP(int accountId, string otp, string ipAddress, IPrincipal? principal = null)
        {

            Result<bool> result = new Result<bool>()
            {
                IsSuccessful = false,
            };

            if (_authorizationService.authorize(principal, new string[] { "VerifiedUser", "Admin" }).IsSuccessful)
            {
                _loggerService.Log(LogLevel.INFO, Category.BUSINESS, $"{ipAddress} failed OTP authentication.");
                result.ErrorMessage = "Error, user already logged in.";
                return result;
            }

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

        public async Task<Result<string>> AccountAccess(int accountId, string ipAddress)
        {
            Result<string> result = new Result<string>()
            {
                IsSuccessful = false,
            };

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
