using DevelopmentHell.Hubba.AccountSystem.Abstractions;
using DevelopmentHell.Hubba.AccountSystem.Manager.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using System.Security.Claims;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using Microsoft.Identity.Client;
using DevelopmentHell.Hubba.Notification.Manager.Abstractions;

namespace DevelopmentHell.Hubba.AccountSystem.Manager.Implementations
{
    public class AccountSystemManager : IAccountSystemManager
    {
        private IAccountSystemService _accountSystemService;
        private IOTPService _otpService;
        private IAuthorizationService _authorizationService;
        private ICryptographyService _cryptographyService;
        private INotificationManager _notificationManager;
        public AccountSystemManager(IAccountSystemService accountSystemService, IOTPService otpService, IAuthorizationService authorizationService, ICryptographyService cryptographyService, INotificationManager notificationManager)
        {
            _accountSystemService = accountSystemService;
            _otpService = otpService;
            _authorizationService = authorizationService;
            _cryptographyService = cryptographyService;
            _notificationManager = notificationManager;
        }
        //TODO: Check this function
        public async Task<Result> VerifyAccount()
        {
            Result result = new Result();

            //TODO: sanitation check for email

            //Check prinicpal of user
            if (!_authorizationService.Authorize(new string[] { "AdminUser", "VerifiedUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized.";
                return result;
            }

            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            var stringAccountEmail = claimsPrincipal?.FindFirstValue("azp");

            //Check to ensure strings are taken from token
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format. ";
                return result;
            }
            if (stringAccountEmail is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format. ";
                return result;
            }

            //extract user ID from JWT Token
            var accountId = int.Parse(stringAccountId); //TODO: do I need parser for email

            Result<string> generateOTPResult = await _otpService.NewOTP(accountId).ConfigureAwait(false);
            if (!generateOTPResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = generateOTPResult.ErrorMessage;
                return result;
            }
            string otp = generateOTPResult.Payload!.ToString();

            //Taken from OTP service
            Result sendOTPResult = _otpService.SendOTP(stringAccountEmail, otp);
            if (!sendOTPResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = sendOTPResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> VerifyNewEmail(string newEmail)
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

            //Check to ensure strings are taken from token
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format. ";
                return result;
            }

            //extract user ID from JWT Token
            var accountId = int.Parse(stringAccountId); 

            Result<string> generateOTPResult = await _otpService.NewOTP(accountId).ConfigureAwait(false);
            if (!generateOTPResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = generateOTPResult.ErrorMessage;
                return result;
            }
            string otp = generateOTPResult.Payload!.ToString();

            //Taken from OTP service
            Result sendOTPResult = _otpService.SendOTP(newEmail, otp);
            if (!sendOTPResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = sendOTPResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> OTPVerification(string otpEntry)
        {
            Result result = new Result();

            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format. ";
                return result;
            }
            var accountId = int.Parse(stringAccountId);

            Result checkOTPResult = await _otpService.CheckOTP(accountId, otpEntry);
            if (!checkOTPResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = checkOTPResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> UpdateEmailInformation(string newEmail, string password)
        {
            Result result = new Result();

            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            var stringAccountEmail = claimsPrincipal?.FindFirstValue("azp");

            //Check to ensure strings are taken from token
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format. ";
                return result;
            }
            if (stringAccountEmail is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format. ";
                return result;
            }

            //extract user ID from JWT Token
            var accountId = int.Parse(stringAccountId);

            //check to see if email is different
            if (newEmail == stringAccountEmail)     
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Your new email is the same as the already registered email. Please enter a new email or exit view. ";
                return result;
            }

            Result checkResult = await CheckPassword(accountId, stringAccountEmail, password).ConfigureAwait(false);
            if (!checkResult.IsSuccessful) 
            {
                result.IsSuccessful = false;
                result.ErrorMessage = checkResult.ErrorMessage;
                return result;
            }

            Result updateResult = await _accountSystemService.UpdateEmailInformation(stringAccountEmail, newEmail);
            if(!updateResult.IsSuccessful) 
            {
                result.IsSuccessful = false;
                result.ErrorMessage = updateResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            await _notificationManager.CreateNewNotification(accountId, "Your email has sucessfully changed. ", 0, true);
            return result;
        }

        public async Task<Result> UpdatePassword(string oldPassword, string newPassword, string newPasswordDupe)
        {
            Result result = new Result();

            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            var stringAccountEmail = claimsPrincipal?.FindFirstValue("azp");

            //Check to ensure strings are taken from token
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format. ";
                return result;
            }
            if (stringAccountEmail is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format. ";
                return result;
            }
            var accountId = int.Parse(stringAccountId);

            //Check if new passwords are matching
            if (newPassword != newPasswordDupe)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Your passwords do not match. Please try again. ";
            }
            
            //TODO: validation check for password

            Result checkResult = await CheckPassword(accountId, stringAccountEmail, oldPassword).ConfigureAwait(false);
            if (!checkResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = checkResult.ErrorMessage;
                return result;
            }

            Result<List<Dictionary<string, object>>> getCredResult = await _accountSystemService.GetPasswordData(accountId).ConfigureAwait(false);
            var credentials = getCredResult.Payload;
            Result<HashData> hashPassword = _cryptographyService.HashString(newPassword, (string)credentials![0]["PasswordSalt"]);
            string newHashPassword = Convert.ToBase64String(hashPassword.Payload!.Hash!);

            Result updatePasswordResult = await _accountSystemService.UpdatePassword(newHashPassword, stringAccountEmail).ConfigureAwait(false);
            if (!updatePasswordResult.IsSuccessful) 
            {
                result.IsSuccessful = false;
                result.ErrorMessage = updatePasswordResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            await _notificationManager.CreateNewNotification(accountId, "You've successfully chaned your password. ", 0, true);
            return result;
        }

        public async Task<Result> UpdateUserName(string firstName, string lastName)
        {
            Result result = new Result();

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

            var accountId = int.Parse(stringAccountId);

            Result updateResult = await _accountSystemService.UpdateUserName(accountId, firstName, lastName).ConfigureAwait(false);
            if (!updateResult.IsSuccessful) 
            {
                result.IsSuccessful = false;
                result.ErrorMessage = updateResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            await _notificationManager.CreateNewNotification(accountId, "You've successfully changed your Account Settings. " +
                "If this was not you, please contact administration for further assistance. ", 0, true);
            return result;
        }

        public async Task<Result<List<Dictionary<string, object>>>> GetAccountSettings()
        {
            Result<List<Dictionary<string, object>>> result = new Result<List<Dictionary<string, object>>>();

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

            var accountId = int.Parse(stringAccountId);

            Result<List<Dictionary<string, object>>> getResult = await _accountSystemService.GetAccountSetting(accountId).ConfigureAwait(false);
            if (!getResult.IsSuccessful) 
            {
                result.IsSuccessful = false;
                result.ErrorMessage = getResult.ErrorMessage;
                return result;
            }
            if (getResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error retrieving account settings. ";
                return result;
            }

            return getResult;

        }

        //Check proper credentials of user
        private async Task<Result> CheckPassword(int accountId, string email, string password)
        {
            Result result = new Result();

            Result<List<Dictionary<string, object>>> getCredResult = await _accountSystemService.GetPasswordData(accountId).ConfigureAwait(false);
            var credentials = getCredResult.Payload;
            Result<HashData> hashData = _cryptographyService.HashString(password, (string)credentials![0]["PasswordSalt"]);
            var newHash = Convert.ToBase64String(hashData.Payload!.Hash!);
            var oldHash = credentials![0]["PasswordHash"];
            if (oldHash != newHash)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Incorrect Password. ";
            }

            result.IsSuccessful = true;
            return result;
        }
    }
}
