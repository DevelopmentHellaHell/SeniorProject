using Development.Hubba.JWTHandler.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.AccountDeletion.Service.Abstractions;
using DevelopmentHell.Hubba.AccountDeletion.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.CellPhoneProvider.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Email.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Notification.Manager.Abstractions;
using DevelopmentHell.Hubba.Notification.Manager.Implementations;
using DevelopmentHell.Hubba.Notification.Service.Abstractions;
using DevelopmentHell.Hubba.Notification.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using System.Configuration;

namespace DevelopmentHell.Hubba.AccountDeletion.Test.Integration_Tests
{
    [TestClass]
    public class ServiceIntegrationTests
    {
        private string _usersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;

        private readonly IUserAccountDataAccess _userAccountDataAccess;
        private readonly IAccountDeletionService _accountDeletionService;
        private readonly IRegistrationService _registrationService;
        private readonly ITestingService _testingService;
        private readonly INotificationManager _notificationManager;

        public ServiceIntegrationTests()
        {
            ILoggerService loggerService = new LoggerService(
                new LoggerDataAccess(_logsConnectionString, _logsTable)
            );
            IValidationService validationService = new ValidationService();
            IJWTHandlerService jwtHandlerService = new JWTHandlerService(
                _jwtKey
            );
            ICryptographyService cryptographyService = new CryptographyService(ConfigurationManager.AppSettings["CryptographyKey"]!);
            INotificationService notificationService = new NotificationService(
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
            _userAccountDataAccess = new UserAccountDataAccess(_usersConnectionString, _userAccountsTable);
            _notificationManager = new NotificationManager(
                        notificationService,
                        new CellPhoneProviderService(),
                        new EmailService(
                            ConfigurationManager.AppSettings["SENDGRID_USERNAME"]!,
                            ConfigurationManager.AppSettings["SENDGRID_API_KEY"]!,
                            ConfigurationManager.AppSettings["COMPANY_EMAIL"]!,
                            false
                        ),
                        new AuthorizationService(
                            _userAccountDataAccess,
                            jwtHandlerService,
                            loggerService
                        ),
                        new ValidationService(),
                        loggerService
                );
            _accountDeletionService = new AccountDeletionService(
                _userAccountDataAccess,
                _notificationManager,
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
        public void ShouldInstansiateCtor()
        {
            Assert.IsNotNull(_accountDeletionService);
        }

        [TestMethod]
        public async Task DeleteVerifiedAccount()
        {
            // Arrange

            // generate user account
            string email = "test@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            var expected = new Result { IsSuccessful = true };
            // proving the account was created
            var getUser = await _userAccountDataAccess.GetUser(accountId).ConfigureAwait(false);
            Assert.IsNotNull(getUser.Payload);

            // Act
            Result actual = await _accountDeletionService.DeleteAccount(accountId).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(expected.IsSuccessful == actual.IsSuccessful);
            getUser = await _userAccountDataAccess.GetUser(accountId).ConfigureAwait(false);
            Assert.IsNull(getUser.Payload);
        }

        [TestMethod]
        public async Task DeleteAdminAccount()
        {
            // Arrange

            // generate user account
            string email = "test@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            var expected = new Result { IsSuccessful = true };

            // promoting account to admin
            var getUser = await _userAccountDataAccess.GetUser(accountId).ConfigureAwait(false);
            if (getUser.Payload is not null)
            {
                getUser.Payload.Role = "AdminUser";
                await _userAccountDataAccess.Update(getUser.Payload).ConfigureAwait(false);
            }

            // Act
            Result actual = await _accountDeletionService.DeleteAccount(accountId).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(expected.IsSuccessful == actual.IsSuccessful);
            getUser = await _userAccountDataAccess.GetUser(accountId).ConfigureAwait(false);
            Assert.IsNull(getUser.Payload);
        }

        [TestMethod]
        public async Task CountingAdminAccounts()
        {
            // Arrange

            // generate first admin
            string email1 = "test1@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult1 = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId1 = accountIdResult1.Payload;
            var userAccount1 = await _userAccountDataAccess.GetUser(accountId1).ConfigureAwait(false);
            if (userAccount1.Payload is not null)
            {
                userAccount1.Payload.Role = "AdminUser";
                await _userAccountDataAccess.Update(userAccount1.Payload).ConfigureAwait(false);
            }
            int expectedResult1 = 1;
            int expectedResult2 = 2;

            // Act
            Result<int> countAdmin1 = await _accountDeletionService.CountAdmin().ConfigureAwait(false);

            // Arrange
            // generate 2nd admin
            string email2 = "test2@gmail.com";
            await _registrationService.RegisterAccount(email2, password).ConfigureAwait(false);
            var accountIdResult2 = await _userAccountDataAccess.GetId(email2).ConfigureAwait(false);
            int accountId2 = accountIdResult2.Payload;
            var userAccount2 = await _userAccountDataAccess.GetUser(accountId2).ConfigureAwait(false);
            if (userAccount2.Payload is not null)
            {
                userAccount2.Payload.Role = "AdminUser";
                await _userAccountDataAccess.Update(userAccount2.Payload).ConfigureAwait(false);
            }

            // Act
            Result<int> countAdmin2 = await _accountDeletionService.CountAdmin().ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(countAdmin1);
            Assert.IsNotNull(countAdmin2);
            Assert.IsTrue(countAdmin1.IsSuccessful && countAdmin2.IsSuccessful);
            Assert.AreEqual(expectedResult1, countAdmin1.Payload);
            Assert.AreEqual(expectedResult2, countAdmin2.Payload);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await _testingService.DeleteAllRecords().ConfigureAwait(false);
        }
    }
}
