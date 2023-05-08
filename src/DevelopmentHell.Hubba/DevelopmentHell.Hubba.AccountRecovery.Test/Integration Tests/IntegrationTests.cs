using System.Configuration;
using System.Security.Claims;
using Development.Hubba.JWTHandler.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.AccountRecovery.Manager.Abstractions;
using DevelopmentHell.Hubba.AccountRecovery.Manager.Implementations;
using DevelopmentHell.Hubba.AccountRecovery.Service.Implementations;
using DevelopmentHell.Hubba.Authentication.Manager.Abstractions;
using DevelopmentHell.Hubba.Authentication.Manager.Implementations;
using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Authentication.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Email.Service.Abstractions;
using DevelopmentHell.Hubba.Email.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Notification.Service.Abstractions;
using DevelopmentHell.Hubba.Notification.Service.Implementations;
using DevelopmentHell.Hubba.OneTimePassword.Service.Abstractions;
using DevelopmentHell.Hubba.OneTimePassword.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Manager.Abstractions;
using DevelopmentHell.Hubba.Registration.Manager.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;

namespace DevelopmentHell.Hubba.AccountRecovery.Test.Integration_Tests
{
    [TestClass]
    public class IntegrationTests
    {
        private string _usersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private string _userOTPsTable = ConfigurationManager.AppSettings["UserOTPsTable"]!;
        private string _userLoginsTable = ConfigurationManager.AppSettings["UserLoginsTable"]!;
        private string _recoveryRequestsTable = ConfigurationManager.AppSettings["RecoveryRequestsTable"]!;

        private string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;

        private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;
        private string _cryptographyKey = ConfigurationManager.AppSettings["CryptographyKey"]!;

        // Class to test
        private readonly IAccountRecoveryManager _accountRecoveryManager;

        // Helper classes
        private readonly IRegistrationManager _registrationManager;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly INotificationService _notificationService;
        private readonly IOTPDataAccess _otpDataAccess;
        private readonly IUserAccountDataAccess _userAccountDataAccess;
        private readonly IUserLoginDataAccess _userLoginDataAccess;
        private readonly IRecoveryRequestDataAccess _recoveryRequestDataAccess;
        private readonly IRegistrationService _registrationService;
        private readonly IOTPService _otpService;
        private readonly ITestingService _testingService;
        private readonly ILoggerService _loggerService;
        private readonly ICryptographyService _cryptographyService;
        private readonly IValidationService _validationService;
        private readonly IEmailService _emailService;

        public IntegrationTests()
        {
            _loggerService = new LoggerService(
                new LoggerDataAccess(
                    _logsConnectionString,
                    _logsTable
                )
            );

            _userAccountDataAccess = new UserAccountDataAccess(
                _usersConnectionString,
                _userAccountsTable
            );

            _userLoginDataAccess = new UserLoginDataAccess(
                _usersConnectionString,
                _userLoginsTable
            );

            _recoveryRequestDataAccess = new RecoveryRequestDataAccess(
                _usersConnectionString,
                _recoveryRequestsTable
            );
            _otpDataAccess = new OTPDataAccess(
                _usersConnectionString,
                _userOTPsTable
            );
            _cryptographyService = new CryptographyService(
                _cryptographyKey
            );
            _emailService = new EmailService(
                    ConfigurationManager.AppSettings["SENDGRID_USERNAME"]!,
                    ConfigurationManager.AppSettings["SENDGRID_API_KEY"]!,
                    ConfigurationManager.AppSettings["COMPANY_EMAIL"]!,
                    false);

            _validationService = new ValidationService();
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
            IJWTHandlerService jwtHandlerService = new JWTHandlerService(
                _jwtKey
            );

            _authorizationService = new AuthorizationService(
                _userAccountDataAccess,
                jwtHandlerService,
                _loggerService
            );
            _authenticationService = new AuthenticationService(
                _userAccountDataAccess,
                _userLoginDataAccess,
                _cryptographyService,
                jwtHandlerService,
                _validationService,
                _loggerService
            );
            _accountRecoveryManager = new AccountRecoveryManager(
                new AccountRecoveryService(
                    _userAccountDataAccess,
                    _userLoginDataAccess,
                    _recoveryRequestDataAccess,
                    _validationService,
                    _loggerService
                ),
                new OTPService(
                    new OTPDataAccess(
                        _usersConnectionString,
                        _userOTPsTable
                    ),
                    _emailService,
                    _cryptographyService),
                    _authenticationService,
                    _authorizationService,
                    _loggerService
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
                    _userAccountDataAccess,
                    _loggerService
            );
            _registrationService = new RegistrationService(
                _userAccountDataAccess,
                _cryptographyService,
                _validationService,
                _loggerService
            );
            _registrationManager = new RegistrationManager(
                _registrationService,
                _authorizationService,
                _cryptographyService,
                _notificationService,
                _loggerService
            );
            _authenticationManager = new AuthenticationManager(
                _authenticationService,
                _otpService,
                _authorizationService,
                _cryptographyService,
                _loggerService
            );
            _testingService = new TestingService(
                _jwtKey,
                new TestsDataAccess()
            );
        }

        [TestInitialize]
        public async Task Setup()
        {
            await _testingService.DeleteAllRecords().ConfigureAwait(false);
        }

        /*
		 * Success Case
		 * Goal: Successfully add account to RecoveryRequest Datastore, user does not recevie authorization
		 * Process: Register Account Successfully, Attempt Account Recovery
		 */
        [TestMethod]
        public async Task ManualRecovery()
        {
            // Arrange
            string email = "accountrecovery-manualsuccess@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";

            var expected = true;
            var expectedRole = "DefaultUser";

            await _registrationManager.Register(email, password).ConfigureAwait(false);

            // Act
            var verificationResult = await _accountRecoveryManager.EmailVerification(email);

            _testingService.DecodeJWT(verificationResult.Payload!);

            var userIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            var id = userIdResult.Payload;

            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(id).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);

            await _accountRecoveryManager.AuthenticateOTP(otp, dummyIp);
            var actual = await _accountRecoveryManager.AccountAccess(dummyIp);


            ClaimsPrincipal? actualPrincipal = null;
            if (actual.IsSuccessful && actual.Payload is not null)
            {
                _testingService.DecodeJWT(actual.Payload!.Item1, actual.Payload!.Item2);

            }
            actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;


            //Arrange to check recoveryrequests has new request
            var recoveryRequestResult = await _recoveryRequestDataAccess.GetId(id);


            // Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload is null);
            Assert.IsTrue(recoveryRequestResult.IsSuccessful);
            Assert.IsTrue(recoveryRequestResult.Payload == id);
            Assert.IsTrue(actualPrincipal.FindFirstValue("azp")! == email);
            Assert.IsTrue(actualPrincipal.FindFirstValue("role")! == expectedRole);
            Assert.IsTrue(int.Parse(actualPrincipal.FindFirstValue("sub")!) == id);
        }


        /*
		 * Success Case
		 * Goal: Successfully recover user account,user recevies authorization
		 * Process: Register Account Successfully, Log into account, Attempt Account Recovery
		 */
        [TestMethod]
        public async Task AutomatedRecovery01()
        {
            // Arrange
            string email = "accountrecovery-automatedsuccess01@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";

            var expected = true;
            string expectedRole = "VerifiedUser";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            _authenticationManager.Logout();


            // Act
            var verificationResult = await _accountRecoveryManager.EmailVerification(email);

            _testingService.DecodeJWT(verificationResult.Payload!);

            var userIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            var id = userIdResult.Payload;

            getOtp = await _otpDataAccess.GetOTP(id).ConfigureAwait(false);
            otp = _cryptographyService.Decrypt(getOtp.Payload!);

            await _accountRecoveryManager.AuthenticateOTP(otp, dummyIp);
            var actual = await _accountRecoveryManager.AccountAccess(dummyIp);

            ClaimsPrincipal? actualPrincipal = null;
            if (actual.IsSuccessful && actual.Payload is not null)
            {
                _testingService.DecodeJWT(actual.Payload!.Item1, actual.Payload!.Item2);

            }
            actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;


            // Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload is not null);
            Assert.IsTrue(actualPrincipal.FindFirstValue("azp")! == email);
            Assert.IsTrue(actualPrincipal.FindFirstValue("role")! == expectedRole);
            Assert.IsTrue(int.Parse(actualPrincipal.FindFirstValue("sub")!) == id);
        }


        /*
   * Success Case
   * Goal: Successfully recover user account,user recevies authorization
   * Process: Register Account Successfully, Log into account, Fail 3 logins to disable account, Attempt Account Recovery
   */
        [TestMethod]
        public async Task AutomatedRecovery02()
        {
            // Arrange
            string email = "accountrecovery-automatedsuccess02@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";



            var expected = true;
            string expectedRole = "VerifiedUser";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            _authenticationManager.Logout();

            string wrongPassword = "whoops12345678";
            for (int i = 0; i < 3; i++)
            {
                await _authenticationManager.Login(email, wrongPassword, dummyIp);
            }
            var checkDisabledBefore = await _userAccountDataAccess.GetDisabled(newAccountId).ConfigureAwait(false);
            var checkLoginAttemptsBefore = await _userAccountDataAccess.GetAttempt(newAccountId).ConfigureAwait(false);


            // Act
            var verificationResult = await _accountRecoveryManager.EmailVerification(email);

            _testingService.DecodeJWT(verificationResult.Payload!);

            var userIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            var id = userIdResult.Payload;

            getOtp = await _otpDataAccess.GetOTP(id).ConfigureAwait(false);
            otp = _cryptographyService.Decrypt(getOtp.Payload!);

            await _accountRecoveryManager.AuthenticateOTP(otp, dummyIp);
            var actual = await _accountRecoveryManager.AccountAccess(dummyIp);

            ClaimsPrincipal? actualPrincipal = null;
            if (actual.IsSuccessful && actual.Payload is not null)
            {
                _testingService.DecodeJWT(actual.Payload!.Item1, actual.Payload!.Item2);

            }
            actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;


            var checkDisabledAfter = await _userAccountDataAccess.GetDisabled(newAccountId).ConfigureAwait(false);
            var checkLoginAttemptsAfter = await _userAccountDataAccess.GetAttempt(newAccountId).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload is not null);
            Assert.IsTrue(checkDisabledBefore.Payload == true);
            Assert.IsTrue(checkLoginAttemptsBefore.Payload!.LoginAttempts == 3);
            Assert.IsTrue(checkDisabledAfter.Payload == false);
            Assert.IsTrue(checkLoginAttemptsAfter.Payload!.LoginAttempts == 0);
            Assert.IsTrue(actualPrincipal.FindFirstValue("azp")! == email);
            Assert.IsTrue(actualPrincipal.FindFirstValue("role")! == expectedRole);
            Assert.IsTrue(int.Parse(actualPrincipal.FindFirstValue("sub")!) == id);
        }


        /*
        * Failure Case
        * Goal: Attempt to recover account with invalid format email
        * Process: Attempt Account Recovery
        */
        [TestMethod]
        public async Task InvalidUsername()
        {
            // Arrange
            string email = "accountrecoveryfailure";

            var expected = false;
            var expectedErrorMessage = "Invalid email. Retry again or contact system admin";

            // Act
            var actual = await _accountRecoveryManager.EmailVerification(email);

            // Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }

        /*
        * Failure Case
        * Goal: Attempt to recover account with a new account not in the datastore
        * Process: Attempt Account Recovery
        */
        [TestMethod]
        public async Task NewAccount()
        {
            // Arrange
            string email = "accountrecoveryfailure@gmail.com";

            var expected = false;
            var expectedErrorMessage = "Invalid email. Retry again or contact system admin";

            // Act
            var actual = await _accountRecoveryManager.EmailVerification(email);

            // Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }


        /*
        * Failure Case
        * Goal: Attempt to recover account with an invalid OTP
        * Process: Register account, attempt Account Recovery
        */
        [TestMethod]
        public async Task InvalidOTP()
        {
            // Arrange
            string email = "accountrecoveryfailure@gmail.com";
            string dummyIp = "192.0.2.0";
            var password = "12345678";
            var invalidOTP = "whoops";

            var expected = false;
            var expectedErrorMessage = "Invalid or expired OTP, please try again.";
            var expectedRole = "DefaultUser";

            await _registrationManager.Register(email, password).ConfigureAwait(false);

            // Act
            var verificationResult = await _accountRecoveryManager.EmailVerification(email);
            ClaimsPrincipal? actualPrincipal = null;
            if (verificationResult.IsSuccessful)
            {
                _testingService.DecodeJWT(verificationResult.Payload!);

            }
            actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;

            var actual = await _accountRecoveryManager.AuthenticateOTP(invalidOTP, dummyIp);

            // Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
            Assert.IsTrue(actualPrincipal.FindFirstValue("azp")! == email);
            Assert.IsTrue(actualPrincipal.FindFirstValue("role")! == expectedRole);
        }


        /*
        * Failure Case
        * Goal: Attempt to recover account without a token
        * Process: Attempt Account Recovery
        */
        [TestMethod]
        public async Task InvalidTokenFormat01()
        {
            // Arrange
            string dummyIp = "192.0.2.0";
            var invalidOTP = "whoops";

            var expected = false;
            var expectedErrorMessage = "Error, invalid access token format.";

            // Act
            var actual = await _accountRecoveryManager.AuthenticateOTP(invalidOTP, dummyIp);

            // Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }

        /*
        * Failure Case
        * Goal: Attempt to recover account without a token
        * Process: Attempt Account Recovery
        */
        [TestMethod]
        public async Task InvalidTokenFormat02()
        {
            // Arrange
            string dummyIp = "192.0.2.0";
            var invalidOTP = "whoops";

            var expected = false;
            var expectedErrorMessage = "Error, invalid access token format.";

            // Act
            var actual = await _accountRecoveryManager.AccountAccess(dummyIp);

            // Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }

        /*
        * Failure Case
        * Goal: Attempt to recover account as an authenticated user
        * Process: Register account, log in, attempt Account Recovery
        */
        [TestMethod]
        public async Task UserLoggedInAlready1()
        {
            // Arrange
            string email = "accountrecovery-automatedsuccess01@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";

            var expected = false;
            var expectedErrorMessage = "Error, user already logged in.";
            var expectedRole = "VerifiedUser";


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

            // Act
            var actual = await _accountRecoveryManager.EmailVerification(email);

            // Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
            Assert.IsTrue(actualPrincipal.FindFirstValue("azp")! == email);
            Assert.IsTrue(actualPrincipal.FindFirstValue("role")! == expectedRole);
        }


        /*
        * Failure Case
        * Goal: Attempt to recover account as an authenticated user
        * Process: Register account, log in, attempt Account Recovery
        */
        [TestMethod]
        public async Task UserLoggedInAlready2()
        {
            // Arrange
            string email = "accountrecovery-automatedsuccess01@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";
            var invalidOTP = "whoops";

            var expected = false;
            var expectedErrorMessage = "Error, user already logged in.";
            var expectedRole = "VerifiedUser";


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
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            // Act
            var actual = await _accountRecoveryManager.AuthenticateOTP(invalidOTP, dummyIp);

            // Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
            Assert.IsTrue(actualPrincipal.FindFirstValue("azp")! == email);
            Assert.IsTrue(actualPrincipal.FindFirstValue("role")! == expectedRole);
        }
    }
}
