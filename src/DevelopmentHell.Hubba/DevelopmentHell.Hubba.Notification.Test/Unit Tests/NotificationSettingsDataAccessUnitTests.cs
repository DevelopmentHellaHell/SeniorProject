using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Manager.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using System.Configuration;

namespace DevelopmentHell.Hubba.Notification.Test.Unit_Tests
{
    [TestClass]
    public class NotificationSettingsDataAccessUnitTests
    {
        private readonly INotificationSettingsDataAccess _notificationSettingsDataAccess;
        private readonly ITestingService _testingService;
        private readonly IUserAccountDataAccess _userAccountDataAccess;
        private readonly IRegistrationService _registrationService;

        private readonly string _userConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private readonly string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private readonly string notificationsConnectionString = ConfigurationManager.AppSettings["NotificationsConnectionString"]!;
        private readonly string notificationSettingsTable = ConfigurationManager.AppSettings["NotificationSettingsTable"]!;
        private readonly string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;

        private readonly string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;


        public NotificationSettingsDataAccessUnitTests()
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
            _notificationSettingsDataAccess = new NotificationSettingsDataAccess(notificationsConnectionString, notificationSettingsTable);
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
        public async Task CreateNotificationSettings()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;
            var expected = true;
            var beforeCreateNotification = await _notificationSettingsDataAccess.SelectUserNotificationSettings(id).ConfigureAwait(false);

            // Actual
            var actualResult = await _notificationSettingsDataAccess.CreateUserNotificationSettings(new NotificationSettings()
                {
                    UserId = id,
                    SiteNotifications = true,
                    EmailNotifications = false,
                    TextNotifications = false,
                    TypeScheduling = true,
                    TypeWorkspace = true,
                    TypeProjectShowcase = true,
                    TypeOther = true
                }
            ).ConfigureAwait(false);
            var afterCreateNotification = await _notificationSettingsDataAccess.SelectUserNotificationSettings(id).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(actualResult.IsSuccessful == expected);
            Assert.IsNull(beforeCreateNotification.Payload);
            Assert.IsNotNull(afterCreateNotification.Payload);
        }

        [TestMethod]
        public async Task SelectNotificationSettings()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;
            var expected = true;
            var createResult = await _notificationSettingsDataAccess.CreateUserNotificationSettings(new NotificationSettings()
                {
                    UserId = id,
                    SiteNotifications = true,
                    EmailNotifications = false,
                    TextNotifications = false,
                    TypeScheduling = true,
                    TypeWorkspace = true,
                    TypeProjectShowcase = true,
                    TypeOther = true
                }
            ).ConfigureAwait(false);

            // Actual
            var actualResult = await _notificationSettingsDataAccess.SelectUserNotificationSettings(id).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(actualResult.IsSuccessful == expected);
            Assert.IsNotNull(actualResult.Payload);

            Assert.AreEqual(actualResult.Payload.UserId, id);
            Assert.IsTrue(actualResult.Payload.SiteNotifications);
            Assert.IsFalse(actualResult.Payload.EmailNotifications);
            Assert.IsFalse(actualResult.Payload.TextNotifications);
            Assert.IsTrue(actualResult.Payload.TypeScheduling);
            Assert.IsTrue(actualResult.Payload.TypeWorkspace);
            Assert.IsTrue(actualResult.Payload.TypeProjectShowcase);
            Assert.IsTrue(actualResult.Payload.TypeOther);
        }

        [TestMethod]
        public async Task UpdateNotificationSettings()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var id = userIdResult.Payload;
            var expected = true;
            // Create new notification settings
            await _notificationSettingsDataAccess.CreateUserNotificationSettings(new NotificationSettings()
            {
                UserId = id,
                SiteNotifications = true,
                EmailNotifications = false,
                TextNotifications = false,
                TypeScheduling = true,
                TypeWorkspace = true,
                TypeProjectShowcase = true,
                TypeOther = true
            }).ConfigureAwait(false);

            // Actual
            // turn all settings off
            var actualResult = await _notificationSettingsDataAccess.UpdateUserNotificationSettings(new NotificationSettings()
            {
                UserId = id,
                SiteNotifications = false,
                EmailNotifications = false,
                TextNotifications = false,
                TypeScheduling = false,
                TypeWorkspace = false,
                TypeProjectShowcase = false,
                TypeOther = false
            }).ConfigureAwait(false);

            var recievedSettings = await _notificationSettingsDataAccess.SelectUserNotificationSettings(id).ConfigureAwait(false);
            var typeOther = recievedSettings.Payload!.TypeOther;

            //Assert
            Assert.AreEqual(typeOther!.Value, false);
            Assert.IsTrue(recievedSettings.IsSuccessful == expected);
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
            await _notificationSettingsDataAccess.CreateUserNotificationSettings(new NotificationSettings()
            {
                UserId = id,
                SiteNotifications = true,
                EmailNotifications = false,
                TextNotifications = false,
                TypeScheduling = true,
                TypeWorkspace = true,
                TypeProjectShowcase = true,
                TypeOther = true
            }).ConfigureAwait(false);
            var beforeDeleteNotificationSettings = await _notificationSettingsDataAccess.SelectUserNotificationSettings(id).ConfigureAwait(false);
            Assert.IsNotNull(beforeDeleteNotificationSettings.Payload);

            //Actual
            var actualResult = await _notificationSettingsDataAccess.DeleteNotificationSettings(id).ConfigureAwait(false);
            var afterDeleteNotificationSettings = await _notificationSettingsDataAccess.SelectUserNotificationSettings(id).ConfigureAwait(false);

            //Assert
            //check 
            Assert.IsTrue(actualResult.IsSuccessful == expected);
            Assert.IsNull(afterDeleteNotificationSettings.Payload);
        }


        [TestCleanup]
        public async Task Cleanup()
        {
            await _testingService.DeleteAllRecords().ConfigureAwait(false); //FIX: Need to delete Notification databases too
        }
    }
}
