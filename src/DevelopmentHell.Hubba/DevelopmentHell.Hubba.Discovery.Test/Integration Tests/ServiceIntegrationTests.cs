using System.Configuration;
using Development.Hubba.JWTHandler.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Discovery.Service.Abstractions;
using DevelopmentHell.Hubba.Discovery.Service.Implemenatations;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Notification.Service.Abstractions;
using DevelopmentHell.Hubba.Notification.Service.Implementations;
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

namespace DevelopmentHell.Hubba.Discovery.Test
{
    [TestClass]
    public class ServiceIntegrationTests
    {
        private string _usersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private string _userOTPsTable = ConfigurationManager.AppSettings["UserOTPsTable"]!;
        private string _userLoginsTable = ConfigurationManager.AppSettings["UserLoginsTable"]!;

        private string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;

        private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;
        private string _cryptographyKey = ConfigurationManager.AppSettings["CryptographyKey"]!;

        private readonly string _listingProfileConnectionString = ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!;
        private readonly string _listingsTable = ConfigurationManager.AppSettings["ListingsTable"]!;

        private readonly string _showcaseConnectionString = ConfigurationManager.AppSettings["ProjectShowcasesConnectionString"]!;
        private readonly string _showcasesTable = ConfigurationManager.AppSettings["ShowcasesTable"]!;
        private readonly string _showcaseCommentsTable = ConfigurationManager.AppSettings["ShowcaseCommentsTable"]!;
        private readonly string _showcaseVotesTable = ConfigurationManager.AppSettings["ShowcaseVotesTable"]!;
        private readonly string _showcaseCommentVotesTable = ConfigurationManager.AppSettings["ShowcaseCommentVotesTable"]!;
        private readonly string _showcaseReportsTable = ConfigurationManager.AppSettings["ShowcaseReportsTable"]!;
        private readonly string _showcaseCommentReportsTable = ConfigurationManager.AppSettings["ShowcaseCommentReportsTable"]!;

        // Class to test
        private readonly IDiscoveryService _discoveryService;

        // Helper Classes
        private readonly IRegistrationManager _registrationManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly INotificationService _notificationService;
        private readonly IOTPDataAccess _otpDataAccess;
        private readonly IUserAccountDataAccess _userAccountDataAccess;
        private readonly IUserLoginDataAccess _userLoginDataAccess;
        private readonly IRegistrationService _registrationService;
        private readonly ITestingService _testingService;
        private readonly ILoggerService _loggerService;
        private readonly ICryptographyService _cryptographyService;
        private readonly IValidationService _validationService;

        public ServiceIntegrationTests()
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
            IJWTHandlerService jwtHandlerService = new JWTHandlerService(
                _jwtKey
            );

            _authorizationService = new AuthorizationService(
                _userAccountDataAccess,
                jwtHandlerService,
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
            _discoveryService = new DiscoveryService(
                new ListingsDataAccess(_listingProfileConnectionString, _listingsTable),
                new CollaboratorsDataAccess(
                    ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
                    ConfigurationManager.AppSettings["CollaboratorsTable"]!
                ),
                new ProjectShowcaseDataAccess(
                    _showcaseConnectionString,
                    _showcasesTable,
                    _showcaseCommentsTable,
                    _showcaseVotesTable,
                    _showcaseCommentVotesTable,
                    _showcaseReportsTable,
                    _showcaseCommentReportsTable
                ),
                _loggerService
            );
            _testingService = new TestingService(
                _jwtKey,
                new TestsDataAccess()
            );
        }

        [TestMethod]
        public void ShouldInstansiateCtor()
        {
            Assert.IsNotNull(_discoveryService);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(100)]
        public async Task GetCuratedWithValidOffset(int offset)
        {
            // Arrange
            var expectedSuccess = true;

            // Act
            var result = await _discoveryService.GetCurated(offset);
            //Console.WriteLine(result.ErrorMessage);
            // Assert
            Assert.IsTrue(result.IsSuccessful == expectedSuccess);
            Assert.IsNotNull(result.Payload);
        }

        [DataTestMethod]
        [DataRow("woodworking")]
        [DataRow("pottery")]
        public async Task GetSearchWithValidQuery(string query)
        {
            // Arrange
            var category = "listings";
            var filter = "none";
            var offset = 0;
            var expectedSuccess = true;

            // Act
            var result = await _discoveryService.GetSearch(query, category, filter, offset).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.IsSuccessful == expectedSuccess);
            Assert.IsNotNull(result.Payload);
        }

        [DataTestMethod]
        [DataRow("listings")]
        [DataRow("collaborators")]
        [DataRow("showcases")]
        public async Task GetSearchWithValidCategory(string category)
        {
            var filter = "none";
            var query = "woodworking";
            var offset = 0;

            var expectedSuccess = true;

            // Act
            var result = await _discoveryService.GetSearch(query, category, filter, offset).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.IsSuccessful == expectedSuccess);
            Assert.IsNotNull(result.Payload);
        }

        [DataTestMethod]
        [DataRow("none")]
        [DataRow("popular")]
        public async Task GetSearchWithValidFilter(string filter)
        {
            var category = "listings";
            var query = "woodworking";
            var offset = 0;

            var expectedSuccess = true;

            // Act
            var result = await _discoveryService.GetSearch(query, category, filter, offset).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.IsSuccessful == expectedSuccess);
            Assert.IsNotNull(result.Payload);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(100)]
        public async Task GetSearchWithValidOffset(int offset)
        {
            var category = "listings";
            var filter = "none";
            var query = "woodworking";

            var expectedSuccess = true;

            // Act
            var result = await _discoveryService.GetSearch(query, category, filter, offset).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.IsSuccessful == expectedSuccess);
            Assert.IsNotNull(result.Payload);
        }
    }
}