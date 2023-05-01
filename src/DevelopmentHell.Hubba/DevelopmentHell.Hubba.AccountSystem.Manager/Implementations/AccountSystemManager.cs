using DevelopmentHell.Hubba.AccountSystem.Abstractions;
using DevelopmentHell.Hubba.AccountSystem.Manager.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using System.Security.Claims;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Notification.Manager.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Scheduling.Service.Abstractions;
using DevelopmentHell.Hubba.Scheduling.Manager.Abstraction;

namespace DevelopmentHell.Hubba.AccountSystem.Manager.Implementations
{
    public class AccountSystemManager : IAccountSystemManager
    {
        private IAccountSystemService _accountSystemService;
        private IOTPService _otpService;
        private IAuthorizationService _authorizationService;
        private ICryptographyService _cryptographyService;
        private INotificationManager _notificationManager;
        private IValidationService _validationService;
        private ISchedulingManager _schedulingManager;
        private ILoggerService _loggerService;
        public AccountSystemManager(IAccountSystemService accountSystemService, IOTPService otpService, IAuthorizationService authorizationService, ICryptographyService cryptographyService,
            INotificationManager notificationManager, IValidationService validationService, ISchedulingManager schedulingManager, ILoggerService loggerService)
        {
            _accountSystemService = accountSystemService;
            _otpService = otpService;
            _authorizationService = authorizationService;
            _cryptographyService = cryptographyService;
            _notificationManager = notificationManager;
            _validationService = validationService;
            _schedulingManager = schedulingManager;
            _loggerService = loggerService;
        }
        //TODO: Check this function
        public async Task<Result> VerifyAccount()
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

            Result checkerResult =  _validationService.ValidateEmail(newEmail);
            if (!checkerResult.IsSuccessful) 
            {
                result.IsSuccessful = false;
                result.ErrorMessage = checkerResult.ErrorMessage;
                return result;
            }

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

            //Check if email is associated with another account
            Result checkEmailResult = await _accountSystemService.CheckNewEmail(newEmail).ConfigureAwait(false);
            if (!checkEmailResult.IsSuccessful) 
            {
                result.IsSuccessful = false;
                result.ErrorMessage = checkEmailResult.ErrorMessage;
                return result;
            }

            //check to see if email is different
            if (newEmail == stringAccountEmail)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "This email is already registered as your account email. Please enter a new email or exit view. ";
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

            //MOVE
            Result checkResult = await CheckPassword(accountId, stringAccountEmail, password).ConfigureAwait(false);
            if (!checkResult.IsSuccessful) 
            {
                result.IsSuccessful = false;
                result.ErrorMessage = checkResult.ErrorMessage;
                return result;
            }

            Result updateResult = await _accountSystemService.UpdateEmailInformation(accountId, newEmail);
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

            //Validating new password
            Result validationResult = _validationService.ValidatePassword(newPassword); 
            if (!validationResult.IsSuccessful) 
            {
                result.IsSuccessful = false;
                result.ErrorMessage = validationResult.ErrorMessage;
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
            var accountId = int.Parse(stringAccountId);

            if (newPassword == oldPassword || newPasswordDupe == oldPassword) 
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "You have entered the same password. Please try again. ";
                return result;
            }

            //Check if new passwords are matching
            if (newPassword != newPasswordDupe) 
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Your passwords do not match. Please try again. ";
                return result;
            }

            //MOVE
            Result checkResult = await CheckPassword(accountId, stringAccountEmail, oldPassword).ConfigureAwait(false);
            if (!checkResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = checkResult.ErrorMessage;
                return result;
            }

            Result<PasswordInformation> getCredResult = await _accountSystemService.GetPasswordData(accountId).ConfigureAwait(false);
            var credentials = getCredResult.Payload;
            Result<HashData> hashPassword = _cryptographyService.HashString(newPassword, credentials!.PasswordSalt!);
            string newHashPassword = Convert.ToBase64String(hashPassword.Payload!.Hash!);

            Result updatePasswordResult = await _accountSystemService.UpdatePassword(newHashPassword, stringAccountEmail).ConfigureAwait(false);
            if (!updatePasswordResult.IsSuccessful) 
            {
                result.IsSuccessful = false;
                result.ErrorMessage = updatePasswordResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            await _notificationManager.CreateNewNotification(accountId, "You've successfully changed your password. ", 0, true);
            return result;
        }

        public async Task<Result> UpdateUserName(string? firstName, string? lastName)
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

            Result updateResult = await _accountSystemService.UpdateUserName(accountId, firstName!, lastName!).ConfigureAwait(false);
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

        public async Task<Result<AccountSystemSettings>> GetAccountSettings()
        {
            Result<AccountSystemSettings> result = new Result<AccountSystemSettings>();

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

            Result<AccountSystemSettings> getResult = await _accountSystemService.GetAccountSettings(accountId).ConfigureAwait(false);
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

        public async Task<Result> CancelBooking(int bookingId)
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

            Result<bool> cancelResult = await _schedulingManager.CancelBooking(accountId, bookingId).ConfigureAwait(false);
            if(!cancelResult.IsSuccessful || cancelResult.Payload is false) 
            {
                result.IsSuccessful = false;
                result.ErrorMessage = cancelResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            return result;

        }

        public async Task<Result<List<BookingHistory>>> GetBookingHistory()
        {
            Result<List<BookingHistory>> result = new Result<List<BookingHistory>>();
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format. ";
                return result;
            }

            var accountId = int.Parse(stringAccountId);

            var getResult = await _accountSystemService.GetBookingHistory(accountId).ConfigureAwait(false);
            return getResult;
        }


        //Check proper credentials of user
        private async Task<Result> CheckPassword(int accountId, string email, string password) //MOVE
        {
            Result result = new Result();

            Result<PasswordInformation> getCredResult = await _accountSystemService.GetPasswordData(accountId).ConfigureAwait(false);
            var credentials = getCredResult.Payload;
            Result<HashData> hashData = _cryptographyService.HashString(password, credentials!.PasswordSalt!);
            var newHash = Convert.ToBase64String(hashData.Payload!.Hash!);
            var oldHash = credentials.PasswordHash;
            if (oldHash != newHash)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Incorrect Password. ";
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }


    }
}
