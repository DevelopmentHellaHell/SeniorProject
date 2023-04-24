using Development.Hubba.JWTHandler.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.AccountSystem.Abstractions;
using DevelopmentHell.Hubba.AccountSystem.Implementations;
using DevelopmentHell.Hubba.AccountSystem.Manager.Implementations;
using DevelopmentHell.Hubba.Authentication.Manager.Implementations;
using DevelopmentHell.Hubba.Authentication.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.CellPhoneProvider.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Email.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Notification.Manager.Implementations;
using DevelopmentHell.Hubba.Notification.Service.Implementations;
using DevelopmentHell.Hubba.OneTimePassword.Service.Abstractions;
using DevelopmentHell.Hubba.OneTimePassword.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Manager.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Security.Claims;

namespace DevelopmentHell.Hubba.AccountSystem.Test.Integration_Tests
{
    [TestClass]
    public class ManagerIntegrationTests
    {
        private readonly UserAccountDataAccess _userAccountDataAccess;
        private readonly IAccountSystemService _accountSystemService;
        private readonly AuthorizationService _authorizationService;
        private readonly AuthenticationService _authenticationService;
        private readonly RegistrationService _registrationService;
        private readonly NotificationManager _notificationManager;
        private readonly AccountSystemManager _accountSystemManager;
        private readonly RegistrationManager _registrationManager;
        private readonly AuthenticationManager _authenticationManager;
        private readonly TestingService _testingService;
        private readonly NotificationService _notificationService;
        private readonly ICryptographyService _cryptographyService;
        private readonly OTPService _otpService;
        private readonly OTPDataAccess _otpDataAccess;

        public ManagerIntegrationTests() 
        {
            _otpDataAccess = new OTPDataAccess(
                    ConfigurationManager.AppSettings["UsersConnectionString"]!,
                    ConfigurationManager.AppSettings["UserOTPsTable"]!
            );

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
                loggerService
            );
            _cryptographyService = new CryptographyService(
                 ConfigurationManager.AppSettings["CryptographyKey"]!
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
            _authorizationService = new AuthorizationService(
                _userAccountDataAccess,
                jwtHandlerService,
                loggerService
            );
            _registrationService = new RegistrationService(
                _userAccountDataAccess,
                _cryptographyService,
                validationService,
                loggerService
            );
            _notificationManager = new NotificationManager(
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
                ),
                new CellPhoneProviderService(),
                new EmailService(
                    ConfigurationManager.AppSettings["SENDGRID_USERNAME"]!,
                    ConfigurationManager.AppSettings["SENDGRID_API_KEY"]!,
                    ConfigurationManager.AppSettings["COMPANY_EMAIL"]!,
                    false
                ),
                _authorizationService,
                new ValidationService(),
                loggerService
            );

            _accountSystemManager = new AccountSystemManager(
                _accountSystemService,
                _otpService,
                _authorizationService,
                _cryptographyService,
                _notificationManager,
                new ValidationService(),
                loggerService
            );
            _registrationManager = new RegistrationManager(
                _registrationService,
                _authorizationService,
                _cryptographyService,
                _notificationService,
                loggerService
            );

            _authenticationManager = new AuthenticationManager(
                _authenticationService,
                _otpService,
                _authorizationService,
                _cryptographyService,
                loggerService
            );

            _testingService = new TestingService(
                ConfigurationManager.AppSettings["JwtKey"]!,
                new TestsDataAccess()
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
            Assert.IsNotNull(_accountSystemManager);
        }

        [TestMethod]
        public async Task GetUserNameFromFreshAccount()
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
            // Get first name and last name. Results should be null
            var getAccountSettingsResult = await _accountSystemManager.GetAccountSettings().ConfigureAwait(false);

            // Assert 
            Assert.IsTrue(getAccountSettingsResult.IsSuccessful);
            Assert.IsNull(getAccountSettingsResult.Payload!.FirstName);
            Assert.IsNull(getAccountSettingsResult.Payload!.LastName);
        }

        [TestMethod]
        public async Task VerifyAccount()
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
            var verifyResult = await _accountSystemManager.VerifyAccount().ConfigureAwait(false);

            //check past OTP and new OTP to ensure process went off
            Result<byte[]> getNewOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string newOtp = _cryptographyService.Decrypt(getNewOtp.Payload!);

            // Assert
            Assert.IsTrue(verifyResult.IsSuccessful);
            Assert.AreNotEqual(otp, newOtp);
        }

        [TestMethod]
        public async Task VerifyNewEmail()
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
            var verifyResult = await _accountSystemManager.VerifyNewEmail("test2@gmail.com");

            //check past OTP and new OTP to ensure process went off
            Result<byte[]> getNewOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string newOtp = _cryptographyService.Decrypt(getNewOtp.Payload!);

            // Assert 
            Assert.IsTrue(verifyResult.IsSuccessful);
            Assert.AreNotEqual(otp, newOtp);
        }

        [TestMethod]
        public async Task NewEmailFailure()
        {
            // Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";

            // Create a new account with email meant to be our new email
            var secondEmailResult = await _registrationManager.Register("test2@gmail.com", password).ConfigureAwait(false);
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
            var verifyResult = await _accountSystemManager.VerifyNewEmail("test2@gmail.com");

            // Assert
            Assert.IsFalse(verifyResult.IsSuccessful);

        }


        [TestMethod]
        public async Task DuplicateEmailFailure()
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
            var verifyResult = await _accountSystemManager.VerifyNewEmail("test@gmail.com").ConfigureAwait(false);

            // Assert
            Assert.IsFalse(verifyResult.IsSuccessful);

        }

        [TestMethod]
        public async Task OTPVerification_VerifyAccount() 
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
            await _accountSystemManager.VerifyAccount().ConfigureAwait(false);
            Result<byte[]> getNewOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string newOtp = _cryptographyService.Decrypt(getNewOtp.Payload!);
            var verificationResult = await _accountSystemManager.OTPVerification(newOtp).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(verificationResult.IsSuccessful);
            Assert.AreNotEqual(otp, newOtp);
        }

        [TestMethod]
        public async Task OTPVerification_VerifyNewEmail()
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
            await _accountSystemManager.VerifyNewEmail("test1@gmail.com").ConfigureAwait(false);
            Result<byte[]> getNewOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string newOtp = _cryptographyService.Decrypt(getNewOtp.Payload!);
            var verificationResult = await _accountSystemManager.OTPVerification(newOtp).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(verificationResult.IsSuccessful);
            Assert.AreNotEqual(otp, newOtp);
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
            await _accountSystemManager.VerifyNewEmail(newEmail).ConfigureAwait(false);
            Result<byte[]> getNewOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string newOtp = _cryptographyService.Decrypt(getNewOtp.Payload!);
            var verificationResult = await _accountSystemManager.OTPVerification(newOtp).ConfigureAwait(false);

            // Actual 
            var updateResult = await _accountSystemManager.UpdateEmailInformation(newEmail, password).ConfigureAwait(false);
            Result<int> getUpdatedEmailId = await _userAccountDataAccess.GetId(newEmail).ConfigureAwait(false);
            int updatedEmailId = getUpdatedEmailId.Payload;

            // Assert
            Assert.IsTrue(updateResult.IsSuccessful);
            Assert.AreEqual(newAccountId, updatedEmailId);

        }

        [TestMethod]
        public async Task UpdateEmailFailure()
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
            await _accountSystemManager.VerifyNewEmail(newEmail).ConfigureAwait(false);
            Result<byte[]> getNewOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string newOtp = _cryptographyService.Decrypt(getNewOtp.Payload!);
            var verificationResult = await _accountSystemManager.OTPVerification(newOtp).ConfigureAwait(false);

            // Actual 
            var updateResult = await _accountSystemManager.UpdateEmailInformation(newEmail, "password").ConfigureAwait(false);
            Result<int> getUpdatedEmailId = await _userAccountDataAccess.GetId(newEmail).ConfigureAwait(false);
            int updatedEmailId = getUpdatedEmailId.Payload;

            // Assert
            Assert.IsFalse(updateResult.IsSuccessful);

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

            string newPassword = "abcdefghi";
            Result<byte[]> getNewOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string newOtp = _cryptographyService.Decrypt(getNewOtp.Payload!);
            var verificationResult = await _accountSystemManager.OTPVerification(newOtp).ConfigureAwait(false);

            // Actual 
            var updateResult = await _accountSystemManager.UpdatePassword(password, newPassword, newPassword).ConfigureAwait(false);


            // Assert
            Assert.IsTrue(updateResult.IsSuccessful);

        }

        [TestMethod]
        public async Task PasswordDupeFail()
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

            string newPassword = "abcdefghi";
            Result<byte[]> getNewOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string newOtp = _cryptographyService.Decrypt(getNewOtp.Payload!);
            var verificationResult = await _accountSystemManager.OTPVerification(newOtp).ConfigureAwait(false);

            // Actual 
            var updateResult = await _accountSystemManager.UpdatePassword(password, newPassword, "adcdefghij").ConfigureAwait(false);


            // Assert
            Assert.IsFalse(updateResult.IsSuccessful);

        }

        [TestMethod]
        public async Task NewPasswordRequirementFailure()
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

            string newPassword = "abc";
            Result<byte[]> getNewOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string newOtp = _cryptographyService.Decrypt(getNewOtp.Payload!);
            var verificationResult = await _accountSystemManager.OTPVerification(newOtp).ConfigureAwait(false);

            // Actual 
            var updateResult = await _accountSystemManager.UpdatePassword(password, newPassword, newPassword).ConfigureAwait(false);


            // Assert
            Assert.IsFalse(updateResult.IsSuccessful);

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
            var updateResult = await _accountSystemManager.UpdateUserName("Kevin", "Dinh").ConfigureAwait(false);
            var getName = await _accountSystemManager.GetAccountSettings();
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
            var updateResult1 = await _accountSystemManager.UpdateUserName(null, "Dinh").ConfigureAwait(false);
            var updateResult2 = await _accountSystemManager.UpdateUserName("Kevin", null).ConfigureAwait(false);
            var getName = await _accountSystemManager.GetAccountSettings();
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
            var updateResult = await _accountSystemManager.UpdateUserName(null, null).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(updateResult.IsSuccessful);
        }

        [TestMethod]
        public async Task GetUserName()
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

            var updateResult = await _accountSystemManager.UpdateUserName("Kevin", "Dinh").ConfigureAwait(false);

            // Actual
            var getAccountSettingsResult = await _accountSystemManager.GetAccountSettings().ConfigureAwait(false);

            // Assert 
            Assert.IsTrue(getAccountSettingsResult.IsSuccessful);
            Assert.AreEqual(getAccountSettingsResult.Payload!.FirstName, "Kevin");
            Assert.AreEqual(getAccountSettingsResult.Payload!.LastName, "Dinh");
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await _testingService.DeleteAllRecords().ConfigureAwait(false); 
        }
    }
}

