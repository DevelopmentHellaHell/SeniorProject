using System.Configuration;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.AccountDeletion.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.CellPhoneProvider.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Email.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Notification.Manager.Implementations;
using DevelopmentHell.Hubba.Notification.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.UserManagement.Manager.Implementations;
using DevelopmentHell.Hubba.UserManagement.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Implementations;

namespace DevelopmentHell.Hubba.UserManagement.Test
{
    [TestClass]
    public class UserManagementIntegrationTests
    {
        private string _usersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private string _userNamesTable = ConfigurationManager.AppSettings["UserNamesTable"]!;
        private string _notificationsConnectionString = ConfigurationManager.AppSettings["NotificationsConnectionString"]!;
        private string _notificationsTable = ConfigurationManager.AppSettings["UserNotificationsTable"]!;
        private string _notificationSettingsTable = ConfigurationManager.AppSettings["NotificationSettingsTable"]!;
        private string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;
        private string _cryptographyKey = ConfigurationManager.AppSettings["CryptographyKey"]!;
        private string _sendgridApi = ConfigurationManager.AppSettings["SENDGRID_API_KEY"]!;
        private string _sendgridUser = ConfigurationManager.AppSettings["SENDGRID_USERNAME"]!;
        private string _email = ConfigurationManager.AppSettings["COMPANY_EMAIL"]!;

        private LoggerDataAccess _loggerDataAccess;
        private LoggerService _loggerService;
        private UserAccountDataAccess _userAccountDataAccess;
        private JWTHandlerService _jwtHandlerService;
        private AuthorizationService _authorizationService;
        private CryptographyService _cryptographyService;
        private ValidationService _validationService;
        private RegistrationService _registrationService;
        private UserNamesDataAccess _userNamesDataAccess;
        private UserManagementService _userManagementService;
        private NotificationDataAccess _notificationDataAccess;
        private NotificationSettingsDataAccess _notificationSettingsDataAccess;
        private NotificationService _notificationService;
        private CellPhoneProviderService _cellPhoneProviderService;
        private EmailService _emailService;
        private NotificationManager _notificationManager;
        private AccountDeletionService _accountDeletionService;
        private UserManagementManager _userManagementManager;
        private TestsDataAccess _testsDataAccess;
        private TestingService _testingService;

        string adminEmail = "admin@gmail.com";
        string userEmail = "user@gmail.com";
        string password = "12345678";

        int? adminId;
        int? userId;
        string? adminToken;
        string? userToken;

        System.Diagnostics.Stopwatch _stopWatch;



        public UserManagementIntegrationTests()
        {
            _loggerDataAccess = new(_logsConnectionString, _logsTable);
            _loggerService = new(_loggerDataAccess);
            _userAccountDataAccess = new(_usersConnectionString, _userAccountsTable);
            _jwtHandlerService = new(_jwtKey);
            _authorizationService = new(_userAccountDataAccess, _jwtHandlerService, _loggerService);
            _cryptographyService = new(_cryptographyKey);
            _validationService = new();
            _registrationService = new(_userAccountDataAccess, _cryptographyService, _validationService, _loggerService);
            _userNamesDataAccess = new(_usersConnectionString, "UserNames");
            _userManagementService = new(_loggerService, _userAccountDataAccess, _userNamesDataAccess);
            _notificationDataAccess = new(_notificationsConnectionString, _notificationsTable);
            _notificationSettingsDataAccess = new(_notificationsConnectionString, _notificationSettingsTable);
            _notificationService = new(_notificationDataAccess, _notificationSettingsDataAccess, _userAccountDataAccess, _loggerService);
            _cellPhoneProviderService = new();
            _emailService = new(_sendgridUser, _sendgridApi, _email);
            _notificationManager = new(_notificationService, _cellPhoneProviderService, _emailService, _authorizationService, _validationService, _loggerService);
            _accountDeletionService = new(_userAccountDataAccess, _notificationManager, _loggerService);
            _userManagementManager = new(_authorizationService, _loggerService, _registrationService, _userManagementService, _validationService, _accountDeletionService, _userAccountDataAccess);
            _testsDataAccess = new();
            _testingService = new(_jwtKey, _testsDataAccess);
            _stopWatch = new();
        }

        [TestInitialize]
        public async Task Initialize()
        {
            Assert.IsNotNull(_usersConnectionString);
            Assert.IsNotNull(_userAccountsTable);
            Assert.IsNotNull(_logsConnectionString);
            Assert.IsNotNull(_logsTable);
            Assert.IsNotNull(_jwtKey);
            Assert.IsNotNull(_cryptographyKey);

            Result result;
            await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.USERS).ConfigureAwait(false);
            result = await _registrationService.RegisterAccount(adminEmail, password, "AdminUser").ConfigureAwait(false);
            Assert.IsTrue(result.IsSuccessful, result.ErrorMessage);
            result = await _registrationService.RegisterAccount(userEmail, password, "VerifiedUser").ConfigureAwait(false);
            Assert.IsTrue(result.IsSuccessful, result.ErrorMessage);

            var accountIdResult = await _userAccountDataAccess.GetId(adminEmail).ConfigureAwait(false);
            adminId = accountIdResult.Payload;
            accountIdResult = await _userAccountDataAccess.GetId(userEmail).ConfigureAwait(false);
            userId = accountIdResult.Payload;

            adminToken = (await _authorizationService.GenerateAccessToken((int)adminId).ConfigureAwait(false)).Payload;
            userToken = (await _authorizationService.GenerateAccessToken((int)userId).ConfigureAwait(false)).Payload;

            _stopWatch.Start();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Assert.IsTrue(_stopWatch.ElapsedMilliseconds <= 5000);
            _stopWatch.Reset();
        }

        // Make sure Initialize Func and if planned interactions are functional
        [TestMethod]
        public void TestInitialization()
        {
            Assert.IsNotNull(adminToken);
            Assert.IsNotNull(adminId);
            Assert.IsNotNull(userToken);
            Assert.IsNotNull(userId);

            _testingService.DecodeJWT(adminToken);
            Assert.IsNotNull(Thread.CurrentPrincipal);
            Assert.IsTrue(_authorizationService.Authorize(new[] { "AdminUser" }).IsSuccessful);

            _testingService.DecodeJWT(userToken);
            Assert.IsNotNull(Thread.CurrentPrincipal);
            Assert.IsTrue(_authorizationService.Authorize(new[] { "VerifiedUser" }).IsSuccessful);

            Thread.CurrentPrincipal = null;
        }

        [TestMethod]
        public async Task SuccessBackendAdminCreateAccount()
        {
            Assert.IsNotNull(adminToken);
            _testingService.DecodeJWT(adminToken);
            Assert.IsNotNull(Thread.CurrentPrincipal);

            var testResult = await _userManagementManager.ElevatedCreateAccount("test@gmail.com", password, "VerifiedUser", "firstName", "lastName", "userName").ConfigureAwait(false);
            _stopWatch.Stop();
            Assert.IsTrue(testResult.IsSuccessful, testResult.ErrorMessage);

            var getUser = await _userAccountDataAccess.GetUser("test@gmail.com").ConfigureAwait(false);
            Assert.IsNotNull(getUser.Payload);
            int getUserId = getUser.Payload.Id;
            var getNames = await _userNamesDataAccess.GetData(getUserId);
            Assert.IsNotNull(getNames.Payload);
            Assert.IsTrue((string)getNames.Payload!["FirstName"] == "firstName");
            Assert.IsTrue((string)getNames.Payload!["LastName"] == "lastName");
            Assert.IsTrue((string)getNames.Payload!["UserName"] == "userName");
            Assert.IsTrue((string)getUser.Payload!.Role! == "VerifiedUser");
        }

        [TestMethod]
        public async Task FailureBackendAdminCreateAccountUsedEmail()
        {
            Assert.IsNotNull(adminToken);
            _testingService.DecodeJWT(adminToken);
            Assert.IsNotNull(Thread.CurrentPrincipal);

            var testResult = await _userManagementManager.ElevatedCreateAccount(userEmail, password, "VerifiedUser", "firstName", "lastName", "userName");
            _stopWatch.Stop();
            Assert.IsFalse(testResult.IsSuccessful);
        }

        [TestMethod]
        public async Task FailureBackendAdminCreateAccountNotAdmin()
        {
            Assert.IsNotNull(userToken);
            _testingService.DecodeJWT(userToken);
            Assert.IsNotNull(Thread.CurrentPrincipal);

            var testResult = await _userManagementManager.ElevatedCreateAccount("test@gmail.com", password, "VerifiedUser", "firstName", "lastName", "userName");
            _stopWatch.Stop();
            Assert.IsFalse(testResult.IsSuccessful);
        }

        [TestMethod]
        public async Task FailureBackendAdminCreateAccountBadPassword()
        {
            Assert.IsNotNull(userToken);
            _testingService.DecodeJWT(userToken);
            Assert.IsNotNull(Thread.CurrentPrincipal);

            var testResult = await _userManagementManager.ElevatedCreateAccount("test@gmail.com", "asdf", "VerifiedUser", "firstName", "lastName", "userName");
            _stopWatch.Stop();
            Assert.IsFalse(testResult.IsSuccessful);
        }

        [TestMethod]
        public async Task SuccessBackendAdminUpdateAccount()
        {
            Assert.IsNotNull(adminToken);
            _testingService.DecodeJWT(adminToken);
            Assert.IsNotNull(Thread.CurrentPrincipal);

            Dictionary<string, object> updateDict = new() {
                { "FirstName","changedFirstName" },
                { "LastName","changedLastName" }
            };
            var setResult = await _userManagementService.SetNames(userEmail, "firstName", "lastName", "userName");
            Assert.IsTrue(setResult.IsSuccessful);

            var testResult = await _userManagementManager.ElevatedUpdateAccount(userEmail, updateDict);
            _stopWatch.Stop();

            Assert.IsTrue(testResult.IsSuccessful);

            var getUser = await _userAccountDataAccess.GetUser(userEmail).ConfigureAwait(false);
            Assert.IsNotNull(getUser.Payload);
            int getUserId = getUser.Payload.Id;
            var getNames = await _userNamesDataAccess.GetData(getUserId);
            Assert.IsNotNull(getNames.Payload);
            Assert.IsTrue((string)getNames.Payload["FirstName"] == "changedFirstName");
            Assert.IsTrue((string)getNames.Payload["LastName"] == "changedLastName");
            Assert.IsTrue((string)getNames.Payload["UserName"] == "userName");
        }

        public async Task FailureBackendAdminUpdateAccountNotAdmin()
        {
            Assert.IsNotNull(userToken);
            _testingService.DecodeJWT(userToken);
            Assert.IsNotNull(Thread.CurrentPrincipal);

            Dictionary<string, object> updateDict = new() {
                { "FirstName","changedFirstName" },
                { "LastName","changedLastName" }
            };

            var testResult = await _userManagementManager.ElevatedUpdateAccount(userEmail, updateDict);
            _stopWatch.Stop();

            Assert.IsFalse(testResult.IsSuccessful);
        }

        [TestMethod]
        public async Task SuccessBackendAdminDeleteAccount()
        {
            Assert.IsNotNull(adminToken);
            _testingService.DecodeJWT(adminToken);
            Assert.IsNotNull(Thread.CurrentPrincipal);

            var testResult = await _userManagementManager.ElevatedDeleteAccount(userEmail);
            _stopWatch.Stop();

            Assert.IsTrue(testResult.IsSuccessful);

            var getUser = await _userAccountDataAccess.GetUser(userEmail);
            Assert.IsTrue(getUser.IsSuccessful);
            Assert.IsNull(getUser.Payload);
        }
        [TestMethod]
        public async Task FailureBackendAdminDeleteAccountNotAdmin()
        {
            Assert.IsNotNull(userToken);
            _testingService.DecodeJWT(userToken);
            Assert.IsNotNull(Thread.CurrentPrincipal);

            var testResult = await _userManagementManager.ElevatedDeleteAccount(userEmail);
            _stopWatch.Stop();

            Assert.IsFalse(testResult.IsSuccessful);
        }
        [TestMethod]
        public async Task SuccessBackendAdminDisableAccount()
        {
            Assert.IsNotNull(adminToken);
            _testingService.DecodeJWT(adminToken);
            Assert.IsNotNull(Thread.CurrentPrincipal);

            var testResult = await _userManagementManager.ElevatedDisableAccount(userEmail);
            _stopWatch.Stop();

            Assert.IsTrue(testResult.IsSuccessful);

            var getResult = await _userAccountDataAccess.GetUser(userEmail);
            Assert.IsTrue(getResult.IsSuccessful);
            Assert.IsNotNull(getResult.Payload);
            Assert.IsTrue(getResult.Payload.Disabled);
        }
        [TestMethod]
        public async Task FailureBackendAdminDisableAccountNotAdmin()
        {
            Assert.IsNotNull(userToken);
            _testingService.DecodeJWT(userToken);
            Assert.IsNotNull(Thread.CurrentPrincipal);

            var testResult = await _userManagementManager.ElevatedDisableAccount(userEmail);
            _stopWatch.Stop();

            Assert.IsFalse(testResult.IsSuccessful);
        }
        [TestMethod]
        public async Task SuccessBackendAdminEnableAccount()
        {
            Assert.IsNotNull(adminToken);
            _testingService.DecodeJWT(adminToken);
            Assert.IsNotNull(Thread.CurrentPrincipal);

            var updateResult = await _userAccountDataAccess.Update(new()
            {
                Id = (int)userId!,
                Disabled = true
            });
            Assert.IsTrue(updateResult.IsSuccessful);

            var testResult = await _userManagementManager.ElevatedEnableAccount(userEmail);
            _stopWatch.Stop();

            Assert.IsTrue(testResult.IsSuccessful);

            var getResult = await _userAccountDataAccess.GetUser(userEmail);
            Assert.IsTrue(getResult.IsSuccessful);
            Assert.IsNotNull(getResult.Payload);
            Assert.IsFalse(getResult.Payload.Disabled);
        }
        [TestMethod]
        public async Task FailureBackendAdminEnableAccountNotAdmin()
        {
            Assert.IsNotNull(userToken);
            _testingService.DecodeJWT(userToken);
            Assert.IsNotNull(Thread.CurrentPrincipal);

            var updateResult = await _userAccountDataAccess.Update(new()
            {
                Id = (int)userId!,
                Disabled = true
            });
            Assert.IsTrue(updateResult.IsSuccessful);

            var testResult = await _userManagementManager.ElevatedEnableAccount(userEmail);
            _stopWatch.Stop();

            Assert.IsFalse(testResult.IsSuccessful);
        }
        [TestMethod]
        public async Task BackendAdminViewAccount()
        {
            Assert.IsNotNull(adminToken);
            _testingService.DecodeJWT(adminToken);
            Assert.IsNotNull(Thread.CurrentPrincipal);
            //TODO: need listing implementation to test
            Assert.IsTrue(false);
        }
    }
}