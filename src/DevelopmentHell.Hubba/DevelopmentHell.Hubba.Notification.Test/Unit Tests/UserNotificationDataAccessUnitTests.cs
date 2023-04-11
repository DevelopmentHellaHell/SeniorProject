using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using System.Configuration;

namespace DevelopmentHell.Hubba.Notification.Test.Unit_Tests
{
    [TestClass]
    public class UserNotificationDataAccessUnitTests
    {
        private readonly INotificationDataAccess _notificationDataAccess;
        private readonly ITestingService _testingService;
        private readonly IUserAccountDataAccess _userAccountDataAccess;
        private readonly IRegistrationService _registrationService;

        private readonly string _userConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private readonly string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private readonly string notificationsConnectionString = ConfigurationManager.AppSettings["NotificationsConnectionString"]!;
        private readonly string userNotificationsTable = ConfigurationManager.AppSettings["UserNotificationsTable"]!;
        private readonly string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;

        private readonly string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;


        public UserNotificationDataAccessUnitTests()
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
            _notificationDataAccess = new NotificationDataAccess(notificationsConnectionString, userNotificationsTable);
            _testingService = new TestingService(_jwtKey, new TestsDataAccess());
            _userAccountDataAccess = new UserAccountDataAccess(
                _userConnectionString,
                _userAccountsTable
            );
            _registrationService = new RegistrationService(
                _userAccountDataAccess,
                cryptographyService,
                validationService,
                loggerService
            );
        }

        [TestInitialize]
        public async Task Setup()
        {
            await _testingService.DeleteAllRecords().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task AddNotifications()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;
            string expectedMessage = "New notification created!";


            // Actual
            var actual = await _notificationDataAccess.AddNotification(id, expectedMessage, NotificationType.OTHER).ConfigureAwait(false);
            var actualMessage = await _notificationDataAccess.GetNotifications(id).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(actualMessage.Payload!.First()["Message"], expectedMessage);


        }

        [TestMethod]
        public async Task GetNotifications()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;
            var expected = true;

            await _notificationDataAccess.AddNotification(id, "notification 1", NotificationType.OTHER).ConfigureAwait(false);
            await _notificationDataAccess.AddNotification(id, "notification 2", NotificationType.OTHER).ConfigureAwait(false);
            await _notificationDataAccess.AddNotification(id, "notification 3", NotificationType.OTHER).ConfigureAwait(false);
            // Actual
            // Get Notifications 
            var actualResult = await _notificationDataAccess.GetNotifications(id).ConfigureAwait(false);

            //Assert
            Assert.AreEqual(actualResult.Payload!.Count, 3);
            Assert.IsTrue(actualResult.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task HideIndividualNotifications()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;
            var expected = true;

            var hiddenNotification = await _notificationDataAccess.AddNotification(id, "I will be hidden", NotificationType.OTHER).ConfigureAwait(false);
            var notHiddenNotification = await _notificationDataAccess.AddNotification(id, "I will not be hidden", NotificationType.OTHER).ConfigureAwait(false);

            // Get request to get Notification ids 
            var recievedNotification = await _notificationDataAccess.GetNotifications(id).ConfigureAwait(false);
            List<int> notifications = new List<int>();
            Dictionary<string, object> firstNotification = recievedNotification.Payload![0];
            notifications.Add((int)firstNotification["NotificationId"]);

            // Actual
            var resultHideNotification = await _notificationDataAccess.HideIndividualNotifications(notifications).ConfigureAwait(false);
            var actualNotifications = await _notificationDataAccess.GetNotifications(id).ConfigureAwait(false);

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
            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;
            var expected = true;

            await _notificationDataAccess.AddNotification(id, "notification 1", NotificationType.OTHER).ConfigureAwait(false);
            await _notificationDataAccess.AddNotification(id, "notification 2", NotificationType.OTHER).ConfigureAwait(false);
            await _notificationDataAccess.AddNotification(id, "notification 3", NotificationType.OTHER).ConfigureAwait(false);

            var beforeNotifications = await _notificationDataAccess.GetNotifications(id).ConfigureAwait(false);

            //Actual
            //Hide All Notifications
            var actualResult = await _notificationDataAccess.HideAllNotifications(id).ConfigureAwait(false);
            var afterNotifications = await _notificationDataAccess.GetNotifications(id).ConfigureAwait(false);

            //Assert
            Assert.AreEqual(beforeNotifications.Payload!.Count, 3);
            Assert.IsTrue(actualResult.IsSuccessful == expected);
            Assert.IsNull(afterNotifications.Payload);
        }

        [TestMethod]
        public async Task DeleteAllNotifications()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;

            await _notificationDataAccess.AddNotification(id, "notification 1", NotificationType.OTHER).ConfigureAwait(false);
            await _notificationDataAccess.AddNotification(id, "notification 2", NotificationType.OTHER).ConfigureAwait(false);
            await _notificationDataAccess.AddNotification(id, "notification 3", NotificationType.OTHER).ConfigureAwait(false);
            var beforeDeleteNotifications = await _notificationDataAccess.GetNotifications(id).ConfigureAwait(false);

            // Actual
            // Delete Notifications 
            var deleteResult = await _notificationDataAccess.DeleteAllNotifications(id).ConfigureAwait(false);
            var afterDeleteNotifications = await _notificationDataAccess.GetNotifications(id).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(deleteResult.IsSuccessful);
            Assert.IsNotNull(beforeDeleteNotifications.Payload);
            Assert.AreEqual(beforeDeleteNotifications.Payload!.Count, 3);
            Assert.IsNull(afterDeleteNotifications.Payload);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await _testingService.DeleteAllRecords().ConfigureAwait(false); //FIX: Need to delete Notification databases too
        }
    }
}
