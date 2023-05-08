using System.Configuration;
using Development.Hubba.JWTHandler.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.Analytics.Service.Abstractions;
using DevelopmentHell.Hubba.Analytics.Service.Implementations;
using DevelopmentHell.Hubba.Authentication.Manager.Abstractions;
using DevelopmentHell.Hubba.Authentication.Manager.Implementations;
using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Authentication.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
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

namespace DevelopmentHell.Hubba.Analytics.Test
{
    [TestClass]
    public class IntegrationTests
    {
        private string _usersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private string _userOTPsTable = ConfigurationManager.AppSettings["UserOTPsTable"]!;
        private string _userLoginsTable = ConfigurationManager.AppSettings["UserLoginsTable"]!;

        private string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;

        private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;
        private string _cryptographyKey = ConfigurationManager.AppSettings["CryptographyKey"]!;

        // Class to test
        private readonly IAnalyticsService _analyticsService;

        // Helper Classes
        private readonly IRegistrationManager _registrationManager;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly INotificationService _notificationService;
        private readonly IOTPDataAccess _otpDataAccess;
        private readonly IUserAccountDataAccess _userAccountDataAccess;
        private readonly IUserLoginDataAccess _userLoginDataAccess;
        private readonly IRegistrationService _registrationService;
        private readonly IOTPService _otpService;
        private readonly ITestingService _testingService;
        private readonly ILoggerService _loggerService;
        private readonly ICryptographyService _cryptographyService;
        private readonly IValidationService _validationService;

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

            _otpDataAccess = new OTPDataAccess(
                _usersConnectionString,
                _userOTPsTable
            );
            _cryptographyService = new CryptographyService(
                _cryptographyKey
            );

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
            _analyticsService = new AnalyticsService(
                new AnalyticsDataAccess(_logsConnectionString, _logsTable),
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

        [TestMethod]
        public async Task AttemptToAccessWithoutAdmin()
        {
            // Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";

            bool expected = false;

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            // Act
            var result = await _analyticsService.GetData(DateTime.Now).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task AttemptToAccessWithoutAuth()
        {
            // Arrange
            bool expected = false;

            // Act
            var result = await _analyticsService.GetData(DateTime.Now).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.IsSuccessful == expected);
        }
    }
}