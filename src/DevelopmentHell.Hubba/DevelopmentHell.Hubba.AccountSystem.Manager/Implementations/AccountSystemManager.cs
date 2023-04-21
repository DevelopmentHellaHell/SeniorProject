using DevelopmentHell.Hubba.AccountSystem.Abstractions;
using DevelopmentHell.Hubba.AccountSystem.Manager.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using System.Security.Claims;

namespace DevelopmentHell.Hubba.AccountSystem.Manager.Implementations
{
    public class AccountSystemManager : IAccountSystemManager
    {
        private IAccountSystemService _accountSystemService;
        private IOTPService _otpService;
        private IAuthorizationService _authorizationService;
        public AccountSystemManager(IAccountSystemService accountSystemService, IOTPService otpService, IAuthorizationService authorizationService)
        {
            _accountSystemService = accountSystemService;
            _otpService = otpService;
            _authorizationService = authorizationService;
        }
        public async Task<Result> NewOTP()
        {
            Result result = new Result();

            //Check prinicpal of user
            if (!_authorizationService.Authorize(new string[] { "AdminUser", "VerifiedUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized.";
                return result;
            }

            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format. ";
                return result;
            }
            //extract user ID from JWT Token
            var accountId = int.Parse(stringAccountId);

            Result generateOTPResult = await _otpService.NewOTP(accountId).ConfigureAwait(false);
            if (!generateOTPResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Could not generate OTP. Please try again later.";
                return result;
            }
            Result sendOTPResult

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> OTPVerification(string otpEntry)
        {

        }

        public async Task<Result> UpdateEmailInformation();
    }
}
