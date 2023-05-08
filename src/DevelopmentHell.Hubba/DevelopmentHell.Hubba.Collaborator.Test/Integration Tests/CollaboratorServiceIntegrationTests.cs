using System.Configuration;
using System.Security.Claims;
using Development.Hubba.JWTHandler.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.Authentication.Manager.Abstractions;
using DevelopmentHell.Hubba.Authentication.Manager.Implementations;
using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Authentication.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.Collaborator.Service.Abstractions;
using DevelopmentHell.Hubba.Collaborator.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Email.Service.Implementations;
using DevelopmentHell.Hubba.Files.Service.Abstractions;
using DevelopmentHell.Hubba.Files.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using Microsoft.AspNetCore.Http;

namespace DevelopmentHell.Hubba.Collaborator.Test.Integration_Tests
{

    [TestClass]
    public class CollaboratorServiceIntegrationTests
    {
        private string _usersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;

        private readonly IUserAccountDataAccess _userAccountDataAccess;
        private readonly IRegistrationService _registrationService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICollaboratorService _collaboratorService;
        private readonly ICollaboratorsDataAccess _collaboratorsDataAccess;
        private readonly ICollaboratorFileDataAccess _collaboratorFileDataAccess;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly ITestingService _testingService;
        private readonly IValidationService _validationService;
        private readonly ICryptographyService _cryptographyService;
        private readonly IFileService _fileService;
        private readonly IOTPDataAccess _otpDataAccess;
        private readonly string email = "dkoroni@gmail.com";
        private readonly string dummyIp = "127.0.0.1";
        private readonly string password = "12345678";

        public CollaboratorServiceIntegrationTests()
        {
            _otpDataAccess = new OTPDataAccess(ConfigurationManager.AppSettings["UsersConnectionString"]!, ConfigurationManager.AppSettings["UserOTPsTable"]!);
            _userAccountDataAccess = new UserAccountDataAccess(_usersConnectionString, _userAccountsTable);
            _cryptographyService = new CryptographyService(ConfigurationManager.AppSettings["CryptographyKey"]!);
            _validationService = new ValidationService();
            ICryptographyService cryptographyService = new CryptographyService(ConfigurationManager.AppSettings["CryptographyKey"]!);
            IJWTHandlerService jwtHandlerService = new JWTHandlerService(
                _jwtKey
            );
            ILoggerService loggerService = new LoggerService(
                new LoggerDataAccess(_logsConnectionString, _logsTable)
            );
            _testingService = new TestingService(
                _jwtKey,
                new TestsDataAccess()
            );
            _collaboratorsDataAccess = new CollaboratorsDataAccess(
                    ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
                    ConfigurationManager.AppSettings["CollaboratorsTable"]!);
            _collaboratorFileDataAccess = new CollaboratorFileDataAccess(
                    ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
                    ConfigurationManager.AppSettings["CollaboratorFilesTable"]!);
            _collaboratorService = new CollaboratorService(
                _collaboratorsDataAccess,
                _collaboratorFileDataAccess,
                new CollaboratorFileJunctionDataAccess(
                    ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
                    ConfigurationManager.AppSettings["CollaboratorFileJunctionTable"]!),
                new CollaboratorUserVoteDataAccess(
                    ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
                    ConfigurationManager.AppSettings["CollaboratorUserVotesTable"]!),
                new FTPFileService(
                            ConfigurationManager.AppSettings["FTPServer"]!,
                            ConfigurationManager.AppSettings["FTPUsername"]!,
                            ConfigurationManager.AppSettings["FTPPassword"]!,
                            loggerService
                        ),
                loggerService,
                _validationService
                );
            _registrationService = new RegistrationService(
                _userAccountDataAccess,
                cryptographyService,
                _validationService,
                loggerService
            );
            _authenticationService = new AuthenticationService
                (
                    new UserAccountDataAccess
                    (
                        ConfigurationManager.AppSettings["UsersConnectionString"]!,
                        _userAccountsTable
                    ),
                    new UserLoginDataAccess
                    (
                        ConfigurationManager.AppSettings["UsersConnectionString"]!,
                        ConfigurationManager.AppSettings["UserLoginsTable"]!
                    ),
                    new CryptographyService
                    (
                        ConfigurationManager.AppSettings["CryptographyKey"]!
                    ),
                    new JWTHandlerService
                    (
                        _jwtKey
                    ),
                    new ValidationService(),
                    loggerService
                );
            _authenticationManager = new AuthenticationManager(
                _authenticationService,
                new OTPService
                (
                    new OTPDataAccess(
                        ConfigurationManager.AppSettings["UsersConnectionString"]!,
                        ConfigurationManager.AppSettings["UserOTPsTable"]!
                    ),
                    new EmailService
                    (
                        ConfigurationManager.AppSettings["SENDGRID_USERNAME"]!,
                        ConfigurationManager.AppSettings["SENDGRID_API_KEY"]!,
                        ConfigurationManager.AppSettings["COMPANY_EMAIL"]!,
                        true
                    ),
                    new CryptographyService
                    (
                        ConfigurationManager.AppSettings["CryptographyKey"]!
                    )
                ),
                new AuthorizationService
                (
                     new UserAccountDataAccess
                    (
                        ConfigurationManager.AppSettings["UsersConnectionString"]!,
                        _userAccountsTable
                    ),
                      new JWTHandlerService(
                        _jwtKey
                    ),
                    loggerService
                ),
                new CryptographyService
                (
                    ConfigurationManager.AppSettings["CryptographyKey"]!
                ),
                loggerService
            );
            _fileService = new FTPFileService(
                ConfigurationManager.AppSettings["FTPServer"]!,
                ConfigurationManager.AppSettings["FTPUsername"]!,
                ConfigurationManager.AppSettings["FTPPassword"]!,
                loggerService);
            _registrationService = new RegistrationService(
                _userAccountDataAccess,
               _cryptographyService,
               _validationService,
               loggerService
            );

        }
        [TestInitialize]
        public async Task Setup()
        {
            await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.COLLABORATOR_PROFILES).ConfigureAwait(false);
            await _fileService.DeleteDir("/Collaborators").ConfigureAwait(false);
        }


        [TestMethod]
        public void ShouldInstansiateCtor()
        {
            Assert.IsNotNull(_collaboratorService);
        }

        [TestMethod]
        public async Task CreateCollaborator()
        {
            // register as user
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            //log in as user
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(accountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);
            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            // Arrange
            CollaboratorProfile collab = CreateMockCollaboratorProfile();
            IFormFile[] collabFiles = new IFormFile[]
            {
                CreateMockFormFile(),
                CreateMockFormFile()
            };
            IFormFile collabFile = CreateMockFormFile();

            // Act
            var actualResult = await _collaboratorService.CreateCollaborator(collab, collabFiles, collabFile);
            var actual = actualResult.IsSuccessful;
            var hasCollaboratorResult = await _collaboratorService.HasCollaborator(accountId);

            // Assert
            Assert.IsTrue(actual);
            Assert.IsTrue(hasCollaboratorResult.IsSuccessful);

        }

        [TestMethod]
        public async Task FailureCreateCollaboratorWithNoUploadedFiles()
        {
            // register as user
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            //log in as user
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(accountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);
            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            // Arrange
            CollaboratorProfile collab = CreateMockCollaboratorProfile();
            IFormFile[] collabFiles = new IFormFile[]
            {
            };
            IFormFile collabFile = CreateMockFormFile();

            // Act
            var actualResult = await _collaboratorService.CreateCollaborator(collab, collabFiles, collabFile);
            var actual = actualResult.IsSuccessful;
            var hasCollaboratorResult = await _collaboratorService.HasCollaborator(accountId);

            // Assert
            Assert.IsFalse(actual);
            Assert.IsFalse(hasCollaboratorResult.Payload);

        }

        [TestMethod]
        public async Task GetFilesFromCollaborator()
        {
            // register as user
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            //log in as user
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(accountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);
            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            // Arrange
            CollaboratorProfile collab = CreateMockCollaboratorProfile();
            IFormFile[] collabFiles = new IFormFile[]
            {
                CreateMockFormFile(),
                CreateMockFormFile()
            };
            IFormFile collabFile = CreateMockFormFile();
            var createCollaboratorResult = await _collaboratorService.CreateCollaborator(collab, collabFiles, collabFile);
            var getCollaboratorId = await _collaboratorService.GetCollaboratorId(accountId).ConfigureAwait(false);

            // Act
            var getFiles = await _collaboratorService.GetFileUrls((int)getCollaboratorId.Payload!).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(getFiles.IsSuccessful);
            Assert.IsTrue(getFiles.Payload!.Length == 2);
        }

        [TestMethod]
        public async Task DeleteFilesFromCollaborator()
        {
            // register as user
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            //log in as user
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(accountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);
            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            // Arrange
            CollaboratorProfile collab = CreateMockCollaboratorProfile();
            IFormFile[] collabFiles = new IFormFile[]
            {
                CreateMockFormFile(),
                CreateMockFormFile()
            };
            IFormFile collabFile = CreateMockFormFile();
            var createCollaboratorResult = await _collaboratorService.CreateCollaborator(collab, collabFiles, collabFile);
            var getCollaboratorId = await _collaboratorService.GetCollaboratorId(accountId).ConfigureAwait(false);

            // Act
            var deleteCollaborator = await _collaboratorService.DeleteCollaborator((int)getCollaboratorId.Payload!).ConfigureAwait(false);
            var getCollaboratorAfterDeletion = await _collaboratorService.GetCollaborator(accountId).ConfigureAwait(false);
            // Assert
            Assert.IsTrue(deleteCollaborator.IsSuccessful);
            Assert.IsFalse(getCollaboratorAfterDeletion.IsSuccessful);
            Assert.IsNull(getCollaboratorAfterDeletion.Payload);

        }

        [TestMethod]
        public async Task RemoveCollaborator()
        {
            // register as user
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            //log in as user
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(accountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);
            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            // Arrange
            CollaboratorProfile collab = CreateMockCollaboratorProfile();
            IFormFile[] collabFiles = new IFormFile[]
            {
                CreateMockFormFile(),
                CreateMockFormFile()
            };
            IFormFile collabFile = CreateMockFormFile();
            var createCollaboratorResult = await _collaboratorService.CreateCollaborator(collab, collabFiles, collabFile);
            var getCollaboratorId = await _collaboratorService.GetCollaboratorId(accountId).ConfigureAwait(false);

            // Act
            var removeCollaborator = await _collaboratorService.RemoveCollaborator((int)getCollaboratorId.Payload!).ConfigureAwait(false);
            var getCollaboratorAfterRemoval = await _collaboratorService.GetCollaborator(accountId).ConfigureAwait(false);
            var hasCollaboratorResult = await _collaboratorService.HasCollaborator(accountId).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(removeCollaborator.IsSuccessful);
            Assert.IsFalse(getCollaboratorAfterRemoval.IsSuccessful);
            Assert.IsNull(getCollaboratorAfterRemoval.Payload);
            Assert.IsTrue(hasCollaboratorResult.Payload);
        }

        [TestMethod]
        public async Task UpdatePublished()
        {
            // register as user
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            //log in as user
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(accountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);
            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            // Arrange
            CollaboratorProfile collab = CreateMockCollaboratorProfile();
            IFormFile[] collabFiles = new IFormFile[]
            {
                CreateMockFormFile(),
                CreateMockFormFile()
            };
            IFormFile collabFile = CreateMockFormFile();
            var createCollaboratorResult = await _collaboratorService.CreateCollaborator(collab, collabFiles, collabFile);
            var getCollaboratorId = await _collaboratorService.GetCollaboratorId(accountId).ConfigureAwait(false);
            var ownerGetCollaboratorBeforePrivate = await _collaboratorService.GetCollaborator((int)getCollaboratorId.Payload!).ConfigureAwait(false);

            // Act
            var updatePublishedResult = await _collaboratorService.UpdatePublished((int)getCollaboratorId.Payload!, false).ConfigureAwait(false);
            var ownerGetCollaboratorAfterPrivate = await _collaboratorService.GetCollaborator((int)getCollaboratorId.Payload!).ConfigureAwait(false);
            var hasCollaboratorResult = await _collaboratorService.HasCollaborator(accountId).ConfigureAwait(false);

            // Logging out of user
            var authenticationResult = _authenticationService.Logout();
            if (authenticationResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticationResult.Payload!, null);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            await _registrationService.RegisterAccount("noreply.hubba@gmail.com", password).ConfigureAwait(false);
            accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            accountId = accountIdResult.Payload;
            //log in as user
            loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            getOtp = await _otpDataAccess.GetOTP(accountId).ConfigureAwait(false);
            otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);
            actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }
            var otherGetCollaboratorAfterPrivate = await _collaboratorService.GetCollaborator((int)getCollaboratorId.Payload!).ConfigureAwait(false);


            // Assert
            Assert.IsTrue(updatePublishedResult.IsSuccessful);
            Assert.IsTrue(ownerGetCollaboratorBeforePrivate.IsSuccessful);
            Assert.IsNotNull(ownerGetCollaboratorBeforePrivate.Payload);
            Assert.IsTrue(ownerGetCollaboratorAfterPrivate.IsSuccessful);
            Assert.IsNotNull(ownerGetCollaboratorAfterPrivate.Payload);
            Assert.IsTrue(otherGetCollaboratorAfterPrivate.IsSuccessful);
            Assert.IsNotNull(otherGetCollaboratorAfterPrivate.Payload);
            Assert.IsTrue(hasCollaboratorResult.Payload);
        }


        [TestMethod]
        public async Task GetOwnerId()
        {
            // register as user
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            //log in as user
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(accountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);
            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            // Arrange
            CollaboratorProfile collab = CreateMockCollaboratorProfile();
            IFormFile[] collabFiles = new IFormFile[]
            {
                CreateMockFormFile(),
                CreateMockFormFile()
            };
            IFormFile collabFile = CreateMockFormFile();
            await _collaboratorService.CreateCollaborator(collab, collabFiles, collabFile);


            // Act
            var getCollaboratorIdResult = await _collaboratorService.GetCollaboratorId(accountId).ConfigureAwait(false);
            var getBeforeDelete = await _collaboratorService.GetCollaborator((int)getCollaboratorIdResult.Payload!).ConfigureAwait(false);
            await _collaboratorService.DeleteCollaborator((int)getCollaboratorIdResult.Payload!).ConfigureAwait(false);
            var getAfterDelete = await _collaboratorService.GetCollaborator((int)getCollaboratorIdResult.Payload!).ConfigureAwait(false);



            // Assert
            Assert.IsTrue(getCollaboratorIdResult.IsSuccessful);
            Assert.IsTrue(getBeforeDelete.IsSuccessful);
            Assert.IsFalse(getAfterDelete.IsSuccessful);

        }

        [TestMethod]
        public async Task GetFileIds()
        {
            // register as user
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            //log in as user
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(accountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);
            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            // Arrange
            CollaboratorProfile collab = CreateMockCollaboratorProfile();
            IFormFile[] collabFiles = new IFormFile[]
            {
                CreateMockFormFile(),
                CreateMockFormFile()
            };
            IFormFile collabFile = CreateMockFormFile();
            var actualResult = await _collaboratorService.CreateCollaborator(collab, collabFiles, collabFile);
            var actual = actualResult.IsSuccessful;
            var getCollaboratorIdResult = await _collaboratorService.GetCollaboratorId(accountId).ConfigureAwait(false);
            int expected = 2;

            // Act
            var getFileUrls = await _collaboratorService.GetFileUrls((int)getCollaboratorIdResult.Payload!).ConfigureAwait(false);


            // Assert
            Assert.IsTrue(actual);
            Assert.IsTrue(getFileUrls.IsSuccessful);
            Assert.AreEqual(expected, getFileUrls.Payload!.Length);

        }

        [TestMethod]
        public async Task CountFilesWithoutPfp()
        {
            // register as user
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            //log in as user
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(accountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);
            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            // Arrange
            CollaboratorProfile collab = CreateMockCollaboratorProfile();
            IFormFile[] collabFiles = new IFormFile[]
            {
                CreateMockFormFile(),
                CreateMockFormFile()
            };
            IFormFile collabFile = CreateMockFormFile();
            var actualResult = await _collaboratorService.CreateCollaborator(collab, collabFiles, collabFile);
            var actual = actualResult.IsSuccessful;
            var getCollaboratorIdResult = await _collaboratorService.GetCollaboratorId(accountId).ConfigureAwait(false);
            int expected = 2;

            // Act
            var getFileCount = await _collaboratorService.CountFilesWithoutPfp(accountId).ConfigureAwait(false);


            // Assert
            Assert.IsTrue(actual);
            Assert.IsTrue(getFileCount.IsSuccessful);
            Assert.AreEqual(expected, getFileCount.Payload!);

        }












        [TestCleanup]
        public async Task Cleanup()
        {
            await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.COLLABORATOR_PROFILES).ConfigureAwait(false);
            await _fileService.DeleteDir("/Collaborators").ConfigureAwait(false);
        }


        // creates a dummy iformfile
        public IFormFile CreateMockFormFile(string title = "mock")
        {
            var content = "This is a mock image file";
            var fileName = "mockimage.jpg";
            var name = title;
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            return new FormFile(stream, 0, stream.Length, name, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }

        public CollaboratorProfile CreateMockCollaboratorProfile()
        {
            return new CollaboratorProfile()
            {
                Name = "Test",
                ContactInfo = "1235 Two Street",
                Description = "Testing collaborator",
                Published = true,
            };

        }
    }
}