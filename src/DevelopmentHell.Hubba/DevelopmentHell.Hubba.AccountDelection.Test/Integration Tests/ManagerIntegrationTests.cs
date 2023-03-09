using DevelopmentHell.Hubba.AccountDeletion.Manager;
using DevelopmentHell.Hubba.AccountDeletion.Manager.Abstraction;
using DevelopmentHell.Hubba.AccountDeletion.Manager.Implementations;
using DevelopmentHell.Hubba.AccountDeletion.Service.Abstractions;
using DevelopmentHell.Hubba.AccountDeletion.Service.Implementations;
using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Authentication.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using System.Configuration;

namespace DevelopmentHell.Hubba.AccountDeletion.Test
{
    [TestClass]
    public class ManagerIntegrationTests
    {
        private string _usersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;

        private readonly IAuthorizationService _authorizationService;
        private readonly IUserAccountDataAccess _userAccountDataAccess;
        private readonly IAccountDeletionManager _accountDeletionManager;
        private readonly IRegistrationService _registrationService;
        private readonly ITestingService _testingService;

        public ManagerIntegrationTests()
        {
            _userAccountDataAccess = new UserAccountDataAccess(_usersConnectionString, _userAccountsTable);
            IValidationService _validationService = new ValidationService();
            ICryptographyService _cryptographyService = new CryptographyService(ConfigurationManager.AppSettings["CryptographyKey"]!);
            ILoggerService _loggerService = new LoggerService(
                new LoggerDataAccess(_logsConnectionString, _logsTable)
            );
            IAccountDeletionService _accountDeletionService = new AccountDeletionService(
                _userAccountDataAccess, 
                _loggerService
            );
            IAuthenticationService _authenticationService = new AuthenticationService(
                _userAccountDataAccess,
                new UserLoginDataAccess(
                    _usersConnectionString,
                    ConfigurationManager.AppSettings["UserLoginsTable"]!
                ),
                _cryptographyService,
                _validationService,
                _loggerService
            );
            _authorizationService = new AuthorizationService(
                ConfigurationManager.AppSettings["JwtKey"]!,
                _userAccountDataAccess,
                _loggerService
            );
            _accountDeletionManager = new AccountDeletionManager(
                _accountDeletionService, 
                _authenticationService, 
                _authorizationService, 
                _loggerService
            );
            _registrationService = new RegistrationService(
                _userAccountDataAccess,
               _cryptographyService,
                _validationService,
                _loggerService
            );
            _testingService = new TestingService(
                new TestsDataAccess()
            );
        }

        [TestMethod]
        public async Task DeleteVerifiedUserAccount()
        {
            // Arrange
            var result = await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.USERS).ConfigureAwait(false);
            
            // generate user account
            string email = "test@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            // log in as user
            var tokenResult = await _authorizationService.GenerateToken(accountId, false).ConfigureAwait(false);
            if (tokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(tokenResult.Payload!);
            }
            var expected = new Result { IsSuccessful = true};

            // Act
            Result actual = await _accountDeletionManager.DeleteAccount(accountId);

            // Assert
            Assert.IsTrue(expected.IsSuccessful == actual.IsSuccessful);
            var getUser = await _userAccountDataAccess.GetUser(accountId).ConfigureAwait(false);
            Assert.IsNull(getUser.Payload);
        }

        [TestMethod]
        public async Task VerifiedUserUnauthorizedDeletion()
        {
            // Arrange
            var result = await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.USERS).ConfigureAwait(false);

            // generate 2 user accounts
            string email1 = "test1@gmail.com";
            string email2 = "test2@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            await _registrationService.RegisterAccount(email2, password).ConfigureAwait(false);
            var accountIdResult1 = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            var accountIdResult2 = await _userAccountDataAccess.GetId(email2).ConfigureAwait(false);
            int accountId1 = accountIdResult1.Payload;
            int accountId2 = accountIdResult2.Payload;

            // log in as first user
            var tokenResult = await _authorizationService.GenerateToken(accountId1, false).ConfigureAwait(false);
            if (tokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(tokenResult.Payload!);
            }
            var expected = new Result { IsSuccessful = true };

            // Act
            Result actual = await _accountDeletionManager.DeleteAccount(accountId2);

            // Assert
            Assert.IsFalse(expected.IsSuccessful == actual.IsSuccessful);
            var getUser = await _userAccountDataAccess.GetUser(accountId2).ConfigureAwait(false);
            Assert.IsNotNull(getUser.Payload);
        }

        [TestMethod]
        public async Task AdminUserDeleteOther()
        {
            // Arrange
            var result = await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.USERS).ConfigureAwait(false);
            
            // generate 1 user account and 1 admin account
            string email1 = "test1@gmail.com";
            string email2 = "test2@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            await _registrationService.RegisterAccount(email2, password).ConfigureAwait(false);
            var accountIdResult1 = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            var accountIdResult2 = await _userAccountDataAccess.GetId(email2).ConfigureAwait(false);
            int accountId1 = accountIdResult1.Payload;
            int accountId2 = accountIdResult2.Payload;
            var userAccount = await _userAccountDataAccess.GetUser(accountId1).ConfigureAwait(false);
            if (userAccount.Payload is not null)
            {
                userAccount.Payload.Role = "AdminUser";
                await _userAccountDataAccess.Update(userAccount.Payload).ConfigureAwait(false);
            }

            // log in as admin
            var tokenResult = await _authorizationService.GenerateToken(accountId1, false).ConfigureAwait(false);
            if (tokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(tokenResult.Payload!);
            }
            var expected = new Result { IsSuccessful = true };

            // Act
            Result actual = await _accountDeletionManager.DeleteAccount(accountId2);

            // Assert
            Assert.IsTrue(expected.IsSuccessful == actual.IsSuccessful);
            var getUser = await _userAccountDataAccess.GetUser(accountId2).ConfigureAwait(false);
            Assert.IsNull(getUser.Payload);
        }

        [TestMethod]
        public async Task AdminUserDeleteSelfAsLastAdmin()
        {
            // Arrange
            var result = await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.USERS).ConfigureAwait(false);
            
            // generate admin
            string email = "test@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            var userAccount = await _userAccountDataAccess.GetUser(accountId).ConfigureAwait(false);
            if (userAccount.Payload is not null)
            {
                userAccount.Payload.Role = "AdminUser";
                await _userAccountDataAccess.Update(userAccount.Payload).ConfigureAwait(false);
            }

            // log in as admin
            var tokenResult = await _authorizationService.GenerateToken(accountId, false).ConfigureAwait(false);
            if (tokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(tokenResult.Payload!);
            }
            var expected = new Result { IsSuccessful = false };

            // Act
            Result actual = await _accountDeletionManager.DeleteAccount(accountId);

            // Assert
            Assert.IsTrue(expected.IsSuccessful == actual.IsSuccessful);
            var getUser = await _userAccountDataAccess.GetUser(accountId).ConfigureAwait(false);
            Assert.IsNotNull(getUser.Payload);
        }

        [TestMethod]
        public async Task AdminUserDeleteSelfWithOtherAdmin()
        {
            // Arrange
            var result = await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.USERS).ConfigureAwait(false);
            
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

            // log in as first admin
            var tokenResult = await _authorizationService.GenerateToken(accountId1, false).ConfigureAwait(false);
            if (tokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(tokenResult.Payload!);
            }
            var expected = new Result { IsSuccessful = true };

            // Act
            // delete self
            Result actual = await _accountDeletionManager.DeleteAccount(accountId1);

            // Assert
            Assert.IsTrue(expected.IsSuccessful == actual.IsSuccessful);
            var getUser = await _userAccountDataAccess.GetUser(accountId1).ConfigureAwait(false);
            Assert.IsNull(getUser.Payload);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await _testingService.DeleteAllRecords().ConfigureAwait(false);
        }
    }
}