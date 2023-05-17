using System.Configuration;
using Development.Hubba.JWTHandler.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Authentication.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Notification.Service.Abstractions;
using DevelopmentHell.Hubba.Notification.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;


namespace DevelopmentHell.Hubba.Notification.Test.Integration_Tests
{
    [TestClass]
    public class ServiceIntegrationTests
    {
        private readonly INotificationService _notificationService;
        private readonly IUserAccountDataAccess _userAccountDataAccess;
        private readonly IRegistrationService _registrationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ITestingService _testingService;

        private readonly string _userConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private readonly string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private readonly string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private readonly string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        private readonly string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;
        private readonly string _notificationConnectionString = ConfigurationManager.AppSettings["NotificationsConnectionString"]!;
        private readonly string _userNotificationTable = ConfigurationManager.AppSettings["UserNotificationsTable"]!;
        private readonly string _notificationSettingsTable = ConfigurationManager.AppSettings["NotificationSettingsTable"]!;

        public ServiceIntegrationTests()
        {
            LoggerService loggerService = new LoggerService(
                new LoggerDataAccess(
                    _logsConnectionString,
                    _logsTable
                )
            );
            ICryptographyService cryptographyService = new CryptographyService(
                ConfigurationManager.AppSettings["CryptographyKey"]!
            );
            IValidationService validationService = new ValidationService();
            IJWTHandlerService jwtHandlerService = new JWTHandlerService(
                _jwtKey
            );
            _notificationService = new NotificationService(
                new NotificationDataAccess(
                    _notificationConnectionString,
                    _userNotificationTable
                ),
                new NotificationSettingsDataAccess(
                    _notificationConnectionString,
                    _notificationSettingsTable
                ),
                new UserAccountDataAccess(
                    _userConnectionString,
                    _userAccountsTable
                ),
                loggerService
            );
            _userAccountDataAccess = new UserAccountDataAccess(
                _userConnectionString,
                _userAccountsTable
            );
            _authorizationService = new AuthorizationService(
                _userAccountDataAccess,
                jwtHandlerService,
                loggerService
            );
            _authenticationService = new AuthenticationService(
                _userAccountDataAccess,
                new UserLoginDataAccess(
                    _userConnectionString,
                    ConfigurationManager.AppSettings["UserLoginsTable"]!
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
        public async Task CreateNewNotificationSettings()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;
            var expected = true;

            // Actual
            // Call function to create default notification settings for new user
            var actualResult = await _notificationService.CreateNewNotificationSettings(id).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(actualResult.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task AddNotification()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            var result = await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;
            var expected = true;
            await _notificationService.CreateNewNotificationSettings(id).ConfigureAwait(false);

            // Actual
            //Create a new notification for user
            var actualResult = await _notificationService.AddNotification(id, "test!", NotificationType.OTHER);

            //Assert 
            Assert.IsTrue(actualResult.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task GetUserNotifications()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            var result = await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;
            var expected = true;
            var actualResult = await _notificationService.CreateNewNotificationSettings(id).ConfigureAwait(false);

            // Create three notification
            await _notificationService.AddNotification(id, "message 1", NotificationType.SCHEDULING).ConfigureAwait(false);
            await _notificationService.AddNotification(id, "message 2", NotificationType.OTHER).ConfigureAwait(false);
            await _notificationService.AddNotification(id, "message 3", NotificationType.PROJECT_SHOWCASE).ConfigureAwait(false);

            // Actual
            var actualGetNotifications = await _notificationService.GetNotifications(id).ConfigureAwait(false);

            var notification1 = actualGetNotifications.Payload![0];
            var notification2 = actualGetNotifications.Payload![1];
            var notification3 = actualGetNotifications.Payload![2];

            // Assert
            // check message contents
            Assert.AreEqual("message 1", notification1["Message"]);
            Assert.AreEqual("message 2", notification2["Message"]);
            Assert.AreEqual("message 3", notification3["Message"]);

            Assert.IsTrue(actualGetNotifications.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task GetNotificationSettings()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            var result = await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;
            var expected = true;
            await _notificationService.CreateNewNotificationSettings(id).ConfigureAwait(false);

            // Actual 
            var actualSettings = await _notificationService.SelectUserNotificationSettings(id).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(actualSettings.Payload!.TypeProjectShowcase, true);
            Assert.AreEqual(actualSettings.Payload.TypeOther, true);
            Assert.AreEqual(actualSettings.Payload.TypeScheduling, true);
            Assert.AreEqual(actualSettings.Payload.TypeWorkspace, true);
            Assert.AreEqual(actualSettings.Payload.SiteNotifications, true);
            Assert.AreEqual(actualSettings.Payload.EmailNotifications, false);
            Assert.AreEqual(actualSettings.Payload.TextNotifications, false);
            Assert.IsTrue(actualSettings.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task UpdateUserNotificationSettings()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            var result = await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;
            var expected = true;
            await _notificationService.CreateNewNotificationSettings(id).ConfigureAwait(false);

            // Actual
            // change notification settings to all false
            var actualResult = _notificationService.UpdateNotificationSettings(new NotificationSettings
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
            var recievedSettings = await _notificationService.SelectUserNotificationSettings(id).ConfigureAwait(false);
            var typeOther = recievedSettings.Payload!.TypeOther;

            // Assert
            Assert.AreEqual(typeOther!.Value, false);
            Assert.IsTrue(recievedSettings.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task HideIndividualNotifications()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            var result = await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;
            var expected = true;
            await _notificationService.CreateNewNotificationSettings(id).ConfigureAwait(false);

            var hiddenNotification = await _notificationService.AddNotification(id, "I will be hidden", NotificationType.OTHER).ConfigureAwait(false);
            var notHiddenNotification = await _notificationService.AddNotification(id, "I will not be hidden", NotificationType.OTHER).ConfigureAwait(false);

            // Get request to get Notification ids 
            var recievedNotification = await _notificationService.GetNotifications(id).ConfigureAwait(false);
            List<int> notifications = new List<int>();
            Dictionary<string, object> firstNotification = recievedNotification.Payload![0];
            notifications.Add((int)firstNotification["NotificationId"]);

            // Actual
            // Hide first notification
            var resultHideNotification = await _notificationService.HideIndividualNotifications(notifications).ConfigureAwait(false);
            var actualNotifications = await _notificationService.GetNotifications(id).ConfigureAwait(false);

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
            var result = await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;
            var expected = true;
            await _notificationService.CreateNewNotificationSettings(id).ConfigureAwait(false);

            await _notificationService.AddNotification(id, "notification 1", NotificationType.OTHER).ConfigureAwait(false);
            await _notificationService.AddNotification(id, "notification 2", NotificationType.OTHER).ConfigureAwait(false);
            await _notificationService.AddNotification(id, "notification 3", NotificationType.OTHER).ConfigureAwait(false);

            var beforeNotifications = await _notificationService.GetNotifications(id).ConfigureAwait(false);

            //Actual
            //Hide All Notifications
            var actualResult = await _notificationService.HideAllNotifications(id).ConfigureAwait(false);
            var afterNotifications = await _notificationService.GetNotifications(id).ConfigureAwait(false);

            //Assert
            Assert.AreEqual(beforeNotifications.Payload!.Count, 3);
            Assert.IsNull(afterNotifications.Payload);
            Assert.IsTrue(actualResult.IsSuccessful == expected);

        }

        [TestMethod]
        public async Task DeleteNotifications()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            var result = await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;
            var expected = true;
            await _notificationService.CreateNewNotificationSettings(id).ConfigureAwait(false);

            // Create three notification
            await _notificationService.AddNotification(id, "message 1", NotificationType.SCHEDULING).ConfigureAwait(false);
            await _notificationService.AddNotification(id, "message 2", NotificationType.OTHER).ConfigureAwait(false);
            await _notificationService.AddNotification(id, "message 3", NotificationType.PROJECT_SHOWCASE).ConfigureAwait(false);
            var beforeDeleteNotifications = await _notificationService.GetNotifications(id).ConfigureAwait(false);
            Assert.IsNotNull(beforeDeleteNotifications.Payload);

            //Actual
            var actualResult = await _notificationService.DeleteAllNotifications(id).ConfigureAwait(false);
            var afterDeleteNotifications = await _notificationService.GetNotifications(id).ConfigureAwait(false);

            //Assert
            //check 
            Assert.IsTrue(actualResult.IsSuccessful == expected);
            Assert.IsNull(afterDeleteNotifications.Payload);
        }

        [TestMethod]
        public async Task DeleteNotificationSettings()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            var result = await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;
            var expected = true;
            await _notificationService.CreateNewNotificationSettings(id).ConfigureAwait(false);
            var beforeDeleteNotificationSettings = await _notificationService.SelectUserNotificationSettings(id).ConfigureAwait(false);
            Assert.IsNotNull(beforeDeleteNotificationSettings.Payload);

            // Actual
            var actualResult = await _notificationService.DeleteNotificationSettings(id).ConfigureAwait(false);
            var afterDeleteNotificationSettings = await _notificationService.SelectUserNotificationSettings(id).ConfigureAwait(false);

            // Assert
            // check 
            Assert.IsTrue(actualResult.IsSuccessful == expected);
            Assert.IsNull(afterDeleteNotificationSettings.Payload);
        }

        [TestMethod]
        public async Task GetUser()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            var result = await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;
            var getUserWithDAO = await _userAccountDataAccess.GetUser(id).ConfigureAwait(false);
            Assert.IsNotNull(getUserWithDAO);

            // Actual
            var getUserWithNotification = await _notificationService.GetUser(id).ConfigureAwait(false);

            // Assert
            // check 
            Assert.IsNotNull(getUserWithNotification);
            Assert.IsTrue(getUserWithNotification.IsSuccessful);

            Assert.IsNotNull(getUserWithNotification.Payload);
            Assert.IsNotNull(getUserWithDAO.Payload);


            Assert.AreEqual(getUserWithNotification.Payload.Id, getUserWithDAO.Payload.Id);
            Assert.AreEqual(getUserWithNotification.Payload.Email, getUserWithDAO.Payload.Email);
            Assert.AreEqual(getUserWithNotification.Payload.PasswordHash, getUserWithDAO.Payload.PasswordHash);
            Assert.AreEqual(getUserWithNotification.Payload.PasswordSalt, getUserWithDAO.Payload.PasswordSalt);
            Assert.AreEqual(getUserWithNotification.Payload.LoginAttempts, getUserWithDAO.Payload.LoginAttempts);
            Assert.AreEqual(getUserWithNotification.Payload.FailureTime, getUserWithDAO.Payload.FailureTime);
            Assert.AreEqual(getUserWithNotification.Payload.Disabled, getUserWithDAO.Payload.Disabled);
            Assert.AreEqual(getUserWithNotification.Payload.Role, getUserWithDAO.Payload.Role);
            Assert.AreEqual(getUserWithNotification.Payload.CellPhoneNumber, getUserWithDAO.Payload.CellPhoneNumber);
            Assert.AreEqual(getUserWithNotification.Payload.CellPhoneProvider, getUserWithDAO.Payload.CellPhoneProvider);
        }

        [TestMethod]
        public async Task GetId()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            var result = await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var getIdWithDAO = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            // Actual
            var getIdWithNotification = await _notificationService.GetId(credentialEmail).ConfigureAwait(false);

            // Assert
            // check 
            Assert.IsNotNull(getIdWithNotification);
            Assert.IsTrue(getIdWithNotification.IsSuccessful);
            Assert.AreEqual(getIdWithNotification.Payload, getIdWithDAO.Payload);
        }

        [TestMethod]
        public async Task UpdatePhoneNumber()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;

            // Actual
            // change number and provider
            var actualResult = _notificationService.UpdatePhoneDetails(new UserAccount
            {
                Id = id,
                CellPhoneNumber = "5101234567",
                CellPhoneProvider = CellPhoneProviders.VERIZON
            }
            ).ConfigureAwait(false);
            // get settings again
            var userInformation = await _notificationService.GetUser(id).ConfigureAwait(false);

            // Assert 
            Assert.AreEqual(userInformation.Payload!.CellPhoneNumber, "5101234567");
            Assert.AreEqual(userInformation.Payload.CellPhoneProvider, CellPhoneProviders.VERIZON);
        }


        [TestCleanup]
        public async Task Cleanup()
        {
            var cleanup = await _testingService.DeleteAllRecords().ConfigureAwait(false);
            //Console.WriteLine(cleanup.ErrorMessage);
        }
    }
}
