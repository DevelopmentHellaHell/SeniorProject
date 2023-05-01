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
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Collaborator.Test.Unit_Tests
{
    [TestClass]
    public class CollaboratorDataAccessUnitTests
    {
        private readonly ITestingService _testingService;
        private readonly IFileService _fileService;

        private readonly string _fileName = "FileName";
        private readonly string _fileType = "FileType";
        private readonly string _fileSize = "FileSize";
        private readonly string _fileUrl = "FileUrl";
        private readonly string _ownerId = "OwnerId";
        private readonly string _lastModifiedUser = "LastModifiedUser";
        private readonly string _createDate = "CreateDate";
        private readonly string _fileId = "FileId";
        private readonly string _pfpFile = "PfpFile";
        private readonly string _updateDate = "UpdateDate";
        private readonly string _usersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private readonly string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private readonly string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private readonly string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        private readonly string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;
        private ICollaboratorsDataAccess _collaboratorsDataAccess;
        private ICollaboratorFileDataAccess _collaboratorFileDataAccess;
        private ICollaboratorFileJunctionDataAccess _collaboratorFileJunctionDataAccess;
        private ICollaboratorUserVoteDataAccess _collaboratorUserVoteDataAccess;
        private ICollaboratorService _collaboratorService;
        private readonly IUserAccountDataAccess _userAccountDataAccess;
        private readonly IRegistrationService _registrationService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IValidationService _validationService;
        private readonly ICryptographyService _cryptographyService;
        private readonly IOTPDataAccess _otpDataAccess;
        private readonly string email = "dkoroni@gmail.com";
        private readonly string dummyIp = "127.0.0.1";
        private readonly string password = "12345678";

        public CollaboratorDataAccessUnitTests()
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
            _registrationService = new RegistrationService(
                _userAccountDataAccess,
               _cryptographyService,
               _validationService,
               loggerService
            );
            ValidationService validationService = new ValidationService();
            _collaboratorFileJunctionDataAccess = new CollaboratorFileJunctionDataAccess(
                ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
                ConfigurationManager.AppSettings["CollaboratorFileJunctionTable"]!
            );
            _collaboratorUserVoteDataAccess = new CollaboratorUserVoteDataAccess(
                ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
                ConfigurationManager.AppSettings["CollaboratorUserVotesTable"]!
            );
            _testingService = new TestingService(
                _jwtKey,
                new TestsDataAccess()
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
            _collaboratorsDataAccess = new CollaboratorsDataAccess(
                    ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
                    ConfigurationManager.AppSettings["CollaboratorsTable"]!);
        }

        [TestInitialize]
        public async Task Setup()
        {
            await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.COLLABORATOR_PROFILES).ConfigureAwait(false);
            await _fileService.DeleteDir("/Collaborators").ConfigureAwait(false);
        }


        [TestMethod]
        public async Task InsertCollab()
        {
            //Arrange
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
            var collabProfile = createMockProfile();


            //Act
            var createResult = await _collaboratorsDataAccess.CreateCollaborator(collabProfile).ConfigureAwait(false);
            var createdId = createResult.Payload;
            var hasCollaboratorResult = await _collaboratorsDataAccess.Exists(createdId);

            //Assert
            Assert.IsNotNull(createdId);
            Assert.IsTrue(hasCollaboratorResult.IsSuccessful);

        }

        [TestMethod]
        public async Task GetOwnerId()
        {
            //Arrange
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
            var collabProfile = createMockProfile();
            var createResult = await _collaboratorsDataAccess.CreateCollaborator(collabProfile).ConfigureAwait(false);
            var createdId = createResult.Payload;

            //Act
            var ownerIdResult = await _collaboratorsDataAccess.GetOwnerId(createdId).ConfigureAwait(false);
            var ownerId = ownerIdResult.Payload;


            //Assert
            Assert.IsTrue(ownerIdResult.IsSuccessful);
            Assert.AreEqual(ownerId, accountId);
            Assert.IsNotNull(ownerId);

        }

        [TestMethod]
        public async Task UpdateProfilePictureId()
        {
            //Arrange
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
            var collabProfile = createMockProfile();
            var createResult = await _collaboratorsDataAccess.CreateCollaborator(collabProfile).ConfigureAwait(false);
            var createdId = createResult.Payload;

            //Act
            var updateIdResult = await _collaboratorsDataAccess.UpdatePfpFileId(createdId, null).ConfigureAwait(false);
            var collaboratorResult = await _collaboratorsDataAccess.GetCollaborator(createdId).ConfigureAwait(false);




            //Assert
            Assert.IsNull(collaboratorResult.Payload!.PfpUrl);
            Assert.IsTrue(updateIdResult.IsSuccessful);

        }


        [TestMethod]
        public async Task CollaboratorExists()
        {
            //Arrange
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
            var collabProfile = createMockProfile();


            //Act
            var createResult = await _collaboratorsDataAccess.CreateCollaborator(collabProfile).ConfigureAwait(false);
            var createdId = createResult.Payload;
            var hasCollaboratorResult = await _collaboratorsDataAccess.Exists(createdId);

            //Assert
            Assert.IsTrue(hasCollaboratorResult.IsSuccessful);
            Assert.IsTrue(hasCollaboratorResult.Payload);

        }

        [TestMethod]
        public async Task GetCollaboratorId()
        {
            //Arrange
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
            var collabProfile = createMockProfile();
            var createResult = await _collaboratorsDataAccess.CreateCollaborator(collabProfile).ConfigureAwait(false);
            var createdId = createResult.Payload;


            //Act
            var getCollabId = await _collaboratorsDataAccess.SelectCollaboratorId(accountId).ConfigureAwait(false);
            var collabId = getCollabId.Payload;


            //Assert
            Assert.IsTrue(getCollabId.IsSuccessful);
            Assert.IsNotNull(getCollabId.Payload);
            Assert.AreEqual(createdId, collabId);

        }



        public CollaboratorProfile createMockProfile()
        {
            return new CollaboratorProfile()
            {
                Name="Test",
                ContactInfo="Test street",
                Description="Testing",
                CollabUrls=new List<string>() { "www.example.com"},
                Published=true,
            };
        }



        [TestCleanup]
        public async Task Cleanup()
        {
            await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.COLLABORATOR_PROFILES).ConfigureAwait(false);
            await _fileService.DeleteDir("/Collaborators").ConfigureAwait(false);
        }




    }
}
