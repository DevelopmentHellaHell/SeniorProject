using Development.Hubba.JWTHandler.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Authentication.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.CellPhoneProvider.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Email.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Notification.Manager.Abstractions;
using DevelopmentHell.Hubba.Notification.Manager.Implementations;
using DevelopmentHell.Hubba.Notification.Service.Abstractions;
using DevelopmentHell.Hubba.Notification.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Manager.Abstractions;
using DevelopmentHell.Hubba.Registration.Manager.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using System.Configuration;

namespace DevelopmentHell.Hubba.Notification.Test
{
    [TestClass]
    public class ManagerIntegrationTests
    {
        private readonly INotificationService _notificationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserAccountDataAccess _userAccountDataAccess;
        private readonly INotificationManager _notificationManager;
        private readonly IRegistrationService _registrationService;
        private readonly IRegistrationManager _registrationManager;
        private readonly IAuthenticationService _authenticationService;
        private readonly ITestingService _testingService;

        public ManagerIntegrationTests()
        {
            LoggerService loggerService = new LoggerService(
                new LoggerDataAccess(
                    ConfigurationManager.AppSettings["LogsConnectionString"]!,
                    ConfigurationManager.AppSettings["UserNotificationsTable"]!
                )
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
            _userAccountDataAccess = new UserAccountDataAccess(
                ConfigurationManager.AppSettings["UsersConnectionString"]!,
                ConfigurationManager.AppSettings["UserAccountsTable"]!
            );
            ICryptographyService cryptographyService = new CryptographyService(
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

            _authenticationService = new AuthenticationService(
                _userAccountDataAccess,
                new UserLoginDataAccess(
                    ConfigurationManager.AppSettings["UsersConnectionString"]!,
                    ConfigurationManager.AppSettings["UserLogins"]!
                ),
                cryptographyService,
                jwtHandlerService,
                validationService,
                loggerService
            );

            _registrationService = new RegistrationService(
                _userAccountDataAccess,
                cryptographyService,
                validationService,
                loggerService
            );

            _notificationManager = new NotificationManager(
                _notificationService,
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

            _registrationManager = new RegistrationManager(
                _registrationService,
                _authorizationService,
                cryptographyService,
                _notificationService,
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
            Assert.IsNotNull(_notificationManager);
        }

        [TestMethod]
        public async Task UpdateNotificationSettingsToAllOff()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationManager.Register(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(id, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(id, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);

            }
            var expected = true;

            // Actual
            // Change all settings for Notifications to be false (off)
            var actualUpdateSettingsResult = await _notificationManager.UpdateNotificationSettings(new NotificationSettings()
            {
                UserId = id,
                TypeOther = false,
                TypeProjectShowcase = false,
                TypeScheduling = false,
                TypeWorkspace = false,
                TextNotifications = false,
                SiteNotifications = false,
                EmailNotifications = false,

            }).ConfigureAwait(false);

            var updatedSettings = await _notificationManager.GetNotificationSettings().ConfigureAwait(false);

            // Assert
            Assert.IsFalse(updatedSettings.Payload!.TypeOther);
            Assert.IsFalse(updatedSettings.Payload.TypeProjectShowcase);
            Assert.IsFalse(updatedSettings.Payload.TypeScheduling);
            Assert.IsFalse(updatedSettings.Payload.TypeWorkspace);
            Assert.IsFalse(updatedSettings.Payload.TextNotifications);
            Assert.IsFalse(updatedSettings.Payload.EmailNotifications);
            Assert.IsFalse(updatedSettings.Payload.SiteNotifications);
            Assert.IsTrue(actualUpdateSettingsResult.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task GetUserNotificationSettings()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationManager.Register(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(id, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(id, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);

            }
            var expected = true;

            // Actual
            var actualGetNotifications = await _notificationManager.GetNotificationSettings().ConfigureAwait(false);

            // Assert 
            Assert.IsNotNull(actualGetNotifications.Payload);
            Assert.IsTrue(actualGetNotifications.Payload!.TypeOther);
            Assert.IsTrue(actualGetNotifications.Payload.TypeProjectShowcase);
            Assert.IsTrue(actualGetNotifications.Payload.TypeScheduling);
            Assert.IsTrue(actualGetNotifications.Payload.TypeWorkspace);
            Assert.IsFalse(actualGetNotifications.Payload.TextNotifications);
            Assert.IsFalse(actualGetNotifications.Payload.EmailNotifications);
            Assert.IsTrue(actualGetNotifications.Payload.SiteNotifications);
            Assert.IsTrue(actualGetNotifications.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task GetNewUserPhoneDetails()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationManager.Register(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(id, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(id, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);

            }
            var expected = true;

            // Actual
            // Phone details default to NULL for new accounts
            var actualPhoneDetails = await _notificationManager.GetPhoneDetails().ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(actualPhoneDetails.Payload);
            Assert.IsNull(actualPhoneDetails.Payload.CellPhoneNumber);
            Assert.IsNull(actualPhoneDetails.Payload.CellPhoneProvider);
            Assert.IsTrue(actualPhoneDetails.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task CreateNotification()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationManager.Register(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(id, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(id, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);

            }
            var expected = true;

            // Actual 
            // Create new Notification for user
            var actualResult = await _notificationManager.CreateNewNotification(id, "this is a new notification", NotificationType.OTHER);
            var getNotification = await _notificationManager.GetNotifications().ConfigureAwait(false);

            // Assert
            Assert.AreEqual(getNotification.Payload![0]["Message"], "this is a new notification");
            Assert.IsTrue(actualResult.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task GetNotificationsFromNewUser()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationManager.Register(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(id, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(id, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);

            }
            var expected = true;

            // Actual
            // new account should have no notifications
            var actualNotifications = await _notificationManager.GetNotifications().ConfigureAwait(false);

            // Assert
            Assert.IsNull(actualNotifications.Payload);
            Assert.IsTrue(actualNotifications.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task GetNotifications()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationManager.Register(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(id, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(id, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);

            }
            var expected = true;

            // Actual 
            // Create three notifications of different types for user
            var n1 = await _notificationManager.CreateNewNotification(id, "This is message 1", NotificationType.SCHEDULING);
            var n2 = await _notificationManager.CreateNewNotification(id, "This is message 2", NotificationType.PROJECT_SHOWCASE);
            var n3 = await _notificationManager.CreateNewNotification(id, "This is message 3", NotificationType.WORKSPACE);
            var n4 = await _notificationManager.CreateNewNotification(id, "This is message 4", NotificationType.OTHER);

            var actualNotifications = await _notificationManager.GetNotifications().ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(actualNotifications.Payload);
            Assert.AreEqual(actualNotifications.Payload.Count, 4);
            Assert.IsTrue(actualNotifications.IsSuccessful == expected);
            Assert.IsTrue(n1.IsSuccessful == expected);
            Assert.IsTrue(n2.IsSuccessful == expected);
            Assert.IsTrue(n3.IsSuccessful == expected);
            Assert.IsTrue(n4.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task UpdatePhoneDetails()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationManager.Register(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(id, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(id, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);

            }
            var expected = true;

            // Actual
            // Change phone number and phone provider
            var actualResult = await _notificationManager.UpdatePhoneDetails("5101234567", CellPhoneProviders.T_MOBILE).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(actualResult.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task HideNotification()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationManager.Register(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(id, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(id, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);

            }
            var expected = true;

            // Create two Notifications. One will be hidden, one will not be
            var hiddenNotification = await _notificationManager.CreateNewNotification(id, "I will be hidden", NotificationType.OTHER).ConfigureAwait(false);
            var notHiddenNotification = await _notificationManager.CreateNewNotification(id, "I will not be hidden", NotificationType.OTHER).ConfigureAwait(false);

            // Get request to get Notification ids 
            var recievedNotification = await _notificationManager.GetNotifications().ConfigureAwait(false);
            List<int> notifications = new List<int>();
            Dictionary<string, object> firstNotification = recievedNotification.Payload![0]; //FIX: Create Notification error here too, sometimes result is null
            notifications.Add((int)firstNotification["NotificationId"]);

            // Actual
            // Hide first notification
            var resultHideNotification = await _notificationManager.HideIndividualNotifications(notifications).ConfigureAwait(false);
            var actualNotifications = await _notificationManager.GetNotifications().ConfigureAwait(false);

            // Assert
            Assert.IsTrue(resultHideNotification.IsSuccessful == expected);
            Assert.AreEqual(actualNotifications.Payload!.Count, 1);
        }

        [TestMethod]
        public async Task HideAllNotifications()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationManager.Register(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(id, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(id, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);

            }
            var expected = true;

            // Create Two Notifications
            await _notificationManager.CreateNewNotification(id, "notification 1", NotificationType.OTHER).ConfigureAwait(false);
            await _notificationManager.CreateNewNotification(id, "notification 2", NotificationType.SCHEDULING).ConfigureAwait(false);

            // Checking that two notifications are available to be seen
            var beforeHideAll = await _notificationManager.GetNotifications().ConfigureAwait(false);

            // Actual
            // Hides all notifications of User
            var actualHideAll = await _notificationManager.HideAllNotifications().ConfigureAwait(false);
            var afterHideAll = await _notificationManager.GetNotifications().ConfigureAwait(false);

            //Assert
            Assert.AreEqual(beforeHideAll.Payload!.Count, 2);
            Assert.IsTrue(actualHideAll.IsSuccessful == expected);
            Assert.IsNull(afterHideAll.Payload);
        }

        [TestMethod]
        public async Task EmailNotification()
        {
            // Arrange
            //  - Setup user and initial state
            // used personal email to recieve notification
            var credentialEmail = "kevin.lieu.dinh@gmail.com";
            var credentialPassword = "12345678";
            await _registrationManager.Register(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(id, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(id, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);

            }
            var expected = true;

            // Actual
            // Change settings to recieve email
            var updateSettingsResult = await _notificationManager.UpdateNotificationSettings(new NotificationSettings()
            {
                UserId = id,
                TypeOther = true,
                TypeProjectShowcase = true,
                TypeScheduling = true,
                TypeWorkspace = true,
                TextNotifications = false,
                SiteNotifications = false,
                EmailNotifications = true,

            }).ConfigureAwait(false);


            // Assert
            Assert.IsTrue(updateSettingsResult.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task TextNotification()
        {
            // Arrange
            //  - Setup user and initial state
            // used personal email to recieve notification
            var credentialEmail = "kevin.leiu.dinh@gmail.com";
            var credentialPassword = "12345678";
            await _registrationManager.Register(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(id, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(id, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);

            }
            var expected = true;

            // Actual
            // Change settings to personal number to recieve text message
            var updatePhoneDetails = await _notificationManager.UpdatePhoneDetails("5107755205", CellPhoneProviders.VERIZON);

            var updateSettingsResult = await _notificationManager.UpdateNotificationSettings(new NotificationSettings()
            {
                UserId = id,
                TypeOther = true,
                TypeProjectShowcase = true,
                TypeScheduling = true,
                TypeWorkspace = true,
                TextNotifications = true,
                SiteNotifications = false,
                EmailNotifications = false,

            }).ConfigureAwait(false);


            // Assert
            Assert.IsTrue(updatePhoneDetails.IsSuccessful == expected);
        }

        //TODO: DELETE EVERYTHING
        [TestMethod]
        public async Task DeleteNotificationDetails()
        {
            // Arrange
            //  - Setup user and initial state
            // used personal email to recieve notification
            var credentialEmail = "kevin.leiu.dinh@gmail.com";
            var credentialPassword = "12345678";
            await _registrationManager.Register(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(id, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(id, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);

            }
            await _notificationManager.CreateNewNotification(id, "notification 1", NotificationType.OTHER).ConfigureAwait(false);
            await _notificationManager.CreateNewNotification(id, "notification 2", NotificationType.SCHEDULING).ConfigureAwait(false);
            var beforeSettings = await _notificationManager.GetNotificationSettings().ConfigureAwait(false);
            var beforeNotifications = await _notificationManager.GetNotifications().ConfigureAwait(false);

            // Actual
            var actualResult = await _notificationManager.DeleteNotificationInformation(id).ConfigureAwait(false);
            var afterSettings = await _notificationManager.GetNotificationSettings().ConfigureAwait(false);
            var afterNotifications = await _notificationManager.GetNotifications().ConfigureAwait(false);

            // Assert
            // after should return null
            Assert.IsNotNull(beforeSettings.Payload);
            Assert.IsNotNull(beforeNotifications.Payload);
            Assert.IsNull(afterSettings.Payload);
            Assert.IsNull(afterNotifications.Payload);

        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await _testingService.DeleteAllRecords().ConfigureAwait(false); //FIX: Need to delete Notification databases too
        }
    }
}
