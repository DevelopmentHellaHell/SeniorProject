using System.Configuration;
using System.Security.Claims;
using Development.Hubba.JWTHandler.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.AccountSystem.Implementations;
using DevelopmentHell.Hubba.Authentication.Manager.Implementations;
using DevelopmentHell.Hubba.Authentication.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Email.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Notification.Service.Implementations;
using DevelopmentHell.Hubba.OneTimePassword.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Manager.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevelopmentHell.Hubba.AccountSystem.Test.Integration_Tests
{
    [TestClass]
    public class ServiceIntegrationTests
    {
        private readonly UserAccountDataAccess _userAccountDataAccess;
        private readonly AccountSystemService _accountSystemService;
        private readonly CryptographyService _cryptographyService;
        private readonly AuthorizationService _authorizationService;
        private readonly RegistrationService _registrationService;
        private readonly NotificationService _notificationService;
        private readonly RegistrationManager _registrationManager;
        private readonly TestingService _testingService;
        private readonly OTPDataAccess _otpDataAccess;
        private readonly AuthenticationService _authenticationService;
        private readonly OTPService _otpService;
        private readonly AuthenticationManager _authenticationManager;

        public ServiceIntegrationTests()
        {
            LoggerService loggerService = new LoggerService(
                new LoggerDataAccess(
                    ConfigurationManager.AppSettings["LogsConnectionString"]!,
                    ConfigurationManager.AppSettings["UserNotificationsTable"]!
                )
            );
            _userAccountDataAccess = new UserAccountDataAccess(
                ConfigurationManager.AppSettings["UsersConnectionString"]!,
                ConfigurationManager.AppSettings["UserAccountsTable"]!
            );
            _accountSystemService = new AccountSystemService(
                _userAccountDataAccess,
            new BookingsDataAccess(
                ConfigurationManager.AppSettings["SchedulingsConnectionString"]!,
                ConfigurationManager.AppSettings["BookingsTable"]!
            ),
            new ListingHistoryDataAccess
            (
                ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!,
                ConfigurationManager.AppSettings["ListingHistoryTable"]!
            ),
           new ListingsDataAccess(
                ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!,
                ConfigurationManager.AppSettings["ListingsTable"]!
            ),
                loggerService
            );
            _cryptographyService = new CryptographyService(
                 ConfigurationManager.AppSettings["CryptographyKey"]!
            );
            IValidationService validationService = new ValidationService();
            IJWTHandlerService jwtHandlerService = new JWTHandlerService(
                ConfigurationManager.AppSettings["JwtKey"]!
            );
            _authorizationService = new AuthorizationService(
                _userAccountDataAccess,
                new JWTHandlerService(
                    ConfigurationManager.AppSettings["JwtKey"]!
                ),
                loggerService
            );
            _registrationService = new RegistrationService(
                _userAccountDataAccess,
                _cryptographyService,
                validationService,
                loggerService
            );
            _notificationService = new NotificationService(
                 new NotificationDataAccess(
                     ConfigurationManager.AppSettings["NotificationsConnectionString"]!,
                     ConfigurationManager.AppSettings["UserNotificationsTable"]!
                 ),
                 new NotificationSettingsDataAccess(
                     ConfigurationManager.AppSettings["NotificationsConnectionString"]!,
                     ConfigurationManager.AppSettings["NotificationSettingsTable"]!
                 ),
                 new UserAccountDataAccess(
                     ConfigurationManager.AppSettings["UsersConnectionString"]!,
                     ConfigurationManager.AppSettings["UserAccountsTable"]!
                 ),
                 loggerService
            );
            _registrationManager = new RegistrationManager(
                _registrationService,
                _authorizationService,
                _cryptographyService,
                _notificationService,
                loggerService
            );

            _testingService = new TestingService(
                ConfigurationManager.AppSettings["JwtKey"]!,
                new TestsDataAccess()
            );
            _otpDataAccess = new OTPDataAccess(
                    ConfigurationManager.AppSettings["UsersConnectionString"]!,
                    ConfigurationManager.AppSettings["UserOTPsTable"]!
            );
            _authenticationService = new AuthenticationService(
                _userAccountDataAccess,
                new UserLoginDataAccess(
                    ConfigurationManager.AppSettings["UsersConnectionString"]!,
                    ConfigurationManager.AppSettings["UserLogins"]!
                ),
                _cryptographyService,
                jwtHandlerService,
                validationService,
                loggerService
            );
            _otpService = new OTPService(
                _otpDataAccess,
                new EmailService(
                    ConfigurationManager.AppSettings["SENDGRID_USERNAME"]!,
                    ConfigurationManager.AppSettings["SENDGRID_API_KEY"]!,
                    ConfigurationManager.AppSettings["COMPANY_EMAIL"]!,
                    true
                ),
                _cryptographyService
            );
            _authenticationManager = new AuthenticationManager(
                _authenticationService,
                _otpService,
                _authorizationService,
                _cryptographyService,
                loggerService
            );
        }

        [TestInitialize]
        public async Task Setup()
        {
            await _testingService.DeleteAllRecords().ConfigureAwait(false);
        }

        [TestMethod]
        public void ShouldInstansiateCtor()
        {
            Assert.IsNotNull(_accountSystemService);
        }

        [TestMethod]
        public async Task CheckingEmail()
        {
            // Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);

            }
            actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;

            string newEmail = "test1@gmail.com";

            // Actual 
            var checkResult = await _accountSystemService.CheckNewEmail(newEmail).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(checkResult.IsSuccessful);
        }


        [TestMethod]
        public async Task CheckingEmailFailure()
        {
            // Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";
            string newEmail = "test1@gmail.com";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            await _registrationManager.Register(newEmail, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);

            }
            actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;

            // Actual 
            var checkResult = await _accountSystemService.CheckNewEmail(newEmail).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(checkResult.IsSuccessful);
        }

        [TestMethod]
        public async Task UpdateEmail()
        {
            // Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);

            }
            actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;

            string newEmail = "test1@gmail.com";
            await _accountSystemService.CheckNewEmail(newEmail).ConfigureAwait(false);

            // Actual 
            var updateResult = await _accountSystemService.UpdateEmailInformation(newAccountId, newEmail).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(updateResult.IsSuccessful);
        }

        [TestMethod]
        public async Task GetPasswordData()
        {
            // Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);

            }
            actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;

            // Actual
            var getResult = await _accountSystemService.GetPasswordData(newAccountId).ConfigureAwait(false);
            PasswordInformation passwordInfo = getResult.Payload!;
            string salt = passwordInfo.PasswordSalt!;
            string hash = passwordInfo.PasswordHash!;
            Result<HashData> hashData = _cryptographyService.HashString(password, salt);
            var newHash = Convert.ToBase64String(hashData.Payload!.Hash!);

            // Assert
            Assert.IsTrue(getResult.IsSuccessful);
            Assert.IsNotNull(getResult.Payload);
            Assert.AreEqual(newHash, hash);
        }

        [TestMethod]
        public async Task UpdatePassword()
        {
            // Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);

            }
            actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;

            var getResult = await _accountSystemService.GetPasswordData(newAccountId).ConfigureAwait(false);
            PasswordInformation passwordInfo = getResult.Payload!;
            string salt = passwordInfo.PasswordSalt!;
            string originalHash = passwordInfo.PasswordHash!;

            // Actual
            string newPassword = "abcdefghi";
            Result<HashData> hashData = _cryptographyService.HashString(newPassword, salt);
            var newHash = Convert.ToBase64String(hashData.Payload!.Hash!);

            var updateResult = await _accountSystemService.UpdatePassword(newHash, email).ConfigureAwait(false);
            var getNewResult = await _accountSystemService.GetPasswordData(newAccountId).ConfigureAwait(false);
            PasswordInformation passwordNewInfo = getNewResult.Payload!;
            string saltDupe = passwordNewInfo.PasswordSalt!;
            string retrieveNewHash = passwordNewInfo.PasswordHash!;

            // Assert 
            Assert.AreEqual(newHash, retrieveNewHash);
            Assert.AreNotEqual(retrieveNewHash, originalHash);
            Assert.AreEqual(salt, saltDupe);
            Assert.IsTrue(updateResult.IsSuccessful);
        }

        [TestMethod]
        public async Task GetNameFreshAccount()
        {
            // Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);

            }
            actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;

            // Actual
            var getResult = await _accountSystemService.GetAccountSettings(newAccountId);
            AccountSystemSettings setting = getResult.Payload!;

            // Assert
            Assert.IsNull(setting.FirstName);
            Assert.IsNull(setting.LastName);
            Assert.IsTrue(getResult.IsSuccessful);
        }

        [TestMethod]
        public async Task ChangeName()
        {

            // Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);

            }
            actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;

            // Actual 
            var updateResult = await _accountSystemService.UpdateUserName(newAccountId, "Kevin", "Dinh").ConfigureAwait(false);
            var getName = await _accountSystemService.GetAccountSettings(newAccountId);
            AccountSystemSettings settings = getName.Payload!;
            string firstName = settings.FirstName!;
            string lastName = settings.LastName!;

            // Assert
            Assert.IsTrue(updateResult.IsSuccessful);
            Assert.AreEqual(firstName, "Kevin");
            Assert.AreEqual(lastName, "Dinh");
        }

        [TestMethod]
        public async Task ChangeIndividualName()
        {

            // Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);

            }
            actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;

            // Actual 
            var updateResult1 = await _accountSystemService.UpdateUserName(newAccountId, null, "Dinh").ConfigureAwait(false);
            var updateResult2 = await _accountSystemService.UpdateUserName(newAccountId, "Kevin", null).ConfigureAwait(false);
            var getName = await _accountSystemService.GetAccountSettings(newAccountId);
            AccountSystemSettings settings = getName.Payload!;
            string firstName = settings.FirstName!;
            string lastName = settings.LastName!;

            // Assert
            Assert.IsTrue(updateResult1.IsSuccessful);
            Assert.IsTrue(updateResult2.IsSuccessful);
            Assert.AreEqual(firstName, "Kevin");
            Assert.AreEqual(lastName, "Dinh");
        }

        [TestMethod]
        public async Task nullNameFailure()
        {
            // Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);

            }
            actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;

            // Actual 
            var updateResult = await _accountSystemService.UpdateUserName(newAccountId, null, null).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(updateResult.IsSuccessful);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await _testingService.DeleteAllRecords().ConfigureAwait(false);
        }

    }
}
