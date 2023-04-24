
using Development.Hubba.JWTHandler.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.Authentication.Manager.Abstractions;
using DevelopmentHell.Hubba.Authentication.Manager.Implementations;
using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Authentication.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.Collaborator.Manager.Abstractions;
using DevelopmentHell.Hubba.Collaborator.Manager.Implementations;
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
using DevelopmentHell.Hubba.WebAPI.DTO.Collaborator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Identity.Client;
using System.Configuration;
using System.Security.Claims;
using System.Security.Policy;
using AuthenticationService = DevelopmentHell.Hubba.Authentication.Service.Implementations.AuthenticationService;

namespace DevelopmentHell.Hubba.Collaborator.Test.Integration_Tests
{
    [TestClass]
    public class CollaboratorManagerIntegrationTests
    {
        private string _usersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;

        private readonly IUserAccountDataAccess _userAccountDataAccess;
        private readonly IRegistrationService _registrationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly Authentication.Service.Abstractions.IAuthenticationService _authenticationService;
        private readonly ICollaboratorManager _collaboratorManager;
        private readonly ICollaboratorService _collaboratorService;
        private readonly ICollaboratorsDataAccess _collaboratorsDataAccess;
        private readonly ICollaboratorFileDataAccess _collaboratorFileDataAccess;
        private readonly ITestingService _testingService;
        private readonly IValidationService _validationService;
        private readonly IFileService _fileService;
        private readonly ICryptographyService _cryptographyService;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IOTPDataAccess _otpDataAccess;
        private readonly string realEmail = "dkoroni@gmail.com";
        private readonly string dummyIp = "127.0.0.1";

          


        public CollaboratorManagerIntegrationTests()
        {
            _userAccountDataAccess = new UserAccountDataAccess(_usersConnectionString, _userAccountsTable);
            _validationService = new ValidationService();
            _cryptographyService = new CryptographyService(ConfigurationManager.AppSettings["CryptographyKey"]!);
            IJWTHandlerService jwtHandlerService = new JWTHandlerService(
                _jwtKey
            );
            ILoggerService loggerService = new LoggerService(
                new LoggerDataAccess(_logsConnectionString, _logsTable)
            );
            _authorizationService = new AuthorizationService(
                _userAccountDataAccess,
                jwtHandlerService,
                loggerService
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
            _collaboratorManager = new CollaboratorManager(_collaboratorService,_authorizationService,loggerService, _validationService);
            _authenticationService = new AuthenticationService(
                _userAccountDataAccess,
                new UserLoginDataAccess(
                    _usersConnectionString,
                    ConfigurationManager.AppSettings["UserLoginsTable"]!
                ),
                _cryptographyService,
                jwtHandlerService,
                _validationService,
                loggerService
            );
            _registrationService = new RegistrationService(
                _userAccountDataAccess,
               _cryptographyService,
               _validationService,
               loggerService
            );
            _fileService = new FTPFileService(
                ConfigurationManager.AppSettings["FTPServer"]!,
                ConfigurationManager.AppSettings["FTPUsername"]!,
                ConfigurationManager.AppSettings["FTPPassword"]!,
                loggerService);
            _authenticationManager = new AuthenticationManager(
                new AuthenticationService
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
                ),
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
            _otpDataAccess = new OTPDataAccess(ConfigurationManager.AppSettings["UsersConnectionString"]!, ConfigurationManager.AppSettings["UserOTPsTable"]!);

        }
        [TestInitialize]
        public async Task Setup()
        {
            await _testingService.DeleteAllRecords().ConfigureAwait(false);
        }

        [TestMethod]
        public void ShouldInstansiateCtor()
        {
            Assert.IsNotNull(_collaboratorManager);
        }

        [TestMethod]
        public async Task CreateCollaboratorProfileNoPfp()
        {
            //Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            // log in as user

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

            CreateCollaboratorDTO collab = MockCreateCollaboratorDTO();



            //Act
            Result actual = await _collaboratorManager.CreateCollaborator(collab).ConfigureAwait(false);

            //Assert
            Console.WriteLine(actual.ErrorMessage);

            Assert.IsTrue(actual.IsSuccessful);
        }

        [TestMethod]
        public async Task CreateCollaboratorProfileWithPfp()
        {
            //Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            // log in as user

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

            var collab = MockCreateCollaboratorDTO();
            collab.PfpFile= CreateMockFormFile();


            //Act
            Result actual = await _collaboratorManager.CreateCollaborator(collab).ConfigureAwait(false);

            //Assert
            Console.WriteLine(actual.ErrorMessage);
            Assert.IsTrue(actual.IsSuccessful);
            
        }

        [TestMethod]
        public async Task CreateCollaboratorProfileWithPfpAndMultipleFiles()
        {
            //Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            // log in as user

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

            CreateCollaboratorDTO collab = MockCreateCollaboratorDTO();
            collab.UploadedFiles = new IFormFile[]
            {
                CreateMockFormFile(),
                CreateMockFormFile()
            };
            collab.PfpFile = CreateMockFormFile();


            //Act
            Result actual = await _collaboratorManager.CreateCollaborator(collab).ConfigureAwait(false);

            //Assert
            Console.WriteLine(actual.ErrorMessage);
            Assert.IsTrue(actual.IsSuccessful);
        }

        [TestMethod]
        public async Task GetCollaboratorWithAuthorization()
        {
            //Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            // log in as user

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

            CreateCollaboratorDTO collab = MockCreateCollaboratorDTO();
            collab.UploadedFiles = new IFormFile[]
            {
                CreateMockFormFile(),
                CreateMockFormFile()
            };
            collab.PfpFile = CreateMockFormFile();

            await _collaboratorManager.CreateCollaborator(collab).ConfigureAwait(false);
            var collabIdResult = await _collaboratorsDataAccess.GetCollaboratorId(accountId).ConfigureAwait(false);
            int collabId = (int)collabIdResult.Payload!;

            //Act
            var actual = await _collaboratorManager.GetCollaborator(collabId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful);
            Assert.IsTrue(_validationService.ValidateCollaborator(actual.Payload!).IsSuccessful);
        }

        [TestMethod]
        public async Task UpdateCollaboratorRemovePfp()
        {
            //Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            // log in as user

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

            // create new collaborator
            IFormFile changedFile = CreateMockFormFile("changed");
            CreateCollaboratorDTO collab = MockCreateCollaboratorDTO();
            collab.PfpFile = CreateMockFormFile();
            await _collaboratorManager.CreateCollaborator(collab).ConfigureAwait(false); 

            // create new editing DTO
            EditCollaboratorDTO changedCollab = MockEditCollaboratorDTO();
            changedCollab.UploadedFiles = new IFormFile[] { CreateMockFormFile() };

            // get the newly uploaded collaborator
            var collabIdResult = await _collaboratorsDataAccess.GetCollaboratorId(accountId).ConfigureAwait(false);
            int collabId = (int)collabIdResult.Payload!;
            var getCollabResult = await _collaboratorManager.GetCollaborator(collabId).ConfigureAwait(false);

            // remove the pfp from collaborator
            changedCollab.RemovedFiles = new string[] { getCollabResult.Payload!.PfpUrl! };
            
            //Act
            var actual = await _collaboratorManager.EditCollaborator(changedCollab).ConfigureAwait(false);
            getCollabResult = await _collaboratorManager.GetCollaborator(collabId).ConfigureAwait(false);
            var actualPfpUrl = getCollabResult.Payload?.PfpUrl;

            //Assert
            Assert.IsTrue(actual.IsSuccessful);
            Assert.IsNull(actualPfpUrl);
        }

        [TestMethod]
        public async Task UpdateCollaboratorAddNewPfp()
        {
            //Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            // log in as user

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

            CreateCollaboratorDTO collab = MockCreateCollaboratorDTO();
            collab.PfpFile = CreateMockFormFile();

            await _collaboratorManager.CreateCollaborator(collab).ConfigureAwait(false);

            // create new editing DTO
            EditCollaboratorDTO changedCollab = MockEditCollaboratorDTO();
            changedCollab.PfpFile =  CreateMockFormFile();

            // get the newly uploaded collaborator
            var collabIdResult = await _collaboratorsDataAccess.GetCollaboratorId(accountId).ConfigureAwait(false);
            int collabId = (int)collabIdResult.Payload!;
            var getCollabResult = await _collaboratorManager.GetCollaborator(collabId).ConfigureAwait(false);
            var beforeChangeUrl = getCollabResult.Payload!.PfpUrl;

            // remove the pfp from collaborator
            changedCollab.RemovedFiles = new string[] { getCollabResult.Payload!.PfpUrl! };

            //Act
            var actual = await _collaboratorManager.EditCollaborator(changedCollab).ConfigureAwait(false);
            getCollabResult = await _collaboratorManager.GetCollaborator(collabId).ConfigureAwait(false);
            var actualPfpUrl = getCollabResult.Payload?.PfpUrl;

            //Assert
            Assert.IsNotNull(getCollabResult.Payload!.PfpUrl);
            Assert.IsNotNull(actualPfpUrl);
            Assert.AreNotEqual(beforeChangeUrl, actualPfpUrl);
            Assert.IsTrue(actual.IsSuccessful);
        }


        [TestMethod]
        public async Task UpdateVisibilityPublished()
        {
            //Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            /// log in as user

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


            CreateCollaboratorDTO collab = MockCreateCollaboratorDTO();

            await _collaboratorManager.CreateCollaborator(collab).ConfigureAwait(false);
            var collabIdResult = await _collaboratorsDataAccess.GetCollaboratorId(accountId).ConfigureAwait(false);
            int collabId = (int)collabIdResult.Payload!;
            bool expected = false;

            //Act
            var actualResult = await _collaboratorManager.ChangeVisibility(collabId, false).ConfigureAwait(false);
            var actualPublished = await _collaboratorsDataAccess.GetPublished(collabId).ConfigureAwait(false);
            var actual = actualPublished.Payload!;

            //Assert
            Assert.IsTrue(actualPublished.IsSuccessful);
            Assert.AreEqual(expected, actual);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public async Task RemoveCollaboratorWithNoChangeToOtherCollaborators()
        {
            //Arrange
            // first user
            string email = "test@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            // second user
            string email2 = "test2@gmail.com";
            await _registrationService.RegisterAccount(email2, password).ConfigureAwait(false);
            var accountIdResult2 = await _userAccountDataAccess.GetId(email2).ConfigureAwait(false);
            int accountId2 = accountIdResult2.Payload;

            // log in as user

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

            CreateCollaboratorDTO collab = MockCreateCollaboratorDTO();
            // creating first collaborator profile
            await _collaboratorManager.CreateCollaborator(collab).ConfigureAwait(false);
            _authenticationService.Logout();

            // log in as user 2
            var accessTokenResult2 = await _authorizationService.GenerateAccessToken(accountId2, false).ConfigureAwait(false);
            var idTokenResult2 = _authenticationService.GenerateIdToken(accountId2, accessTokenResult2.Payload!);
            if (accessTokenResult2.IsSuccessful && idTokenResult2.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult2.Payload!, idTokenResult2.Payload!);
            }

            //creating second collaborator profile
            var secondCollabResult = await _collaboratorManager.CreateCollaborator(collab).ConfigureAwait(false);


            // get the second account's id
            var collabIdResult = await _collaboratorsDataAccess.GetCollaboratorId(accountId2).ConfigureAwait(false);
            int collabId = (int)collabIdResult.Payload!;
            var fileIdsResult = await _collaboratorFileDataAccess.SelectFileIdsFromOwner(accountId2).ConfigureAwait(false);
            List<int> fileIds = fileIdsResult.Payload!;

            //Act
            // second account is removing its collaborator profile
            var actual = await _collaboratorManager.RemoveCollaborator(collabId).ConfigureAwait(false);
            var collabIdRemovedResult = await _collaboratorsDataAccess.GetCollaboratorId(accountId2).ConfigureAwait(false);
            var newFileIdResults = await _collaboratorFileDataAccess.SelectFileIdsFromOwner(accountId2).ConfigureAwait(false);
            List<int> newFileId = newFileIdResults.Payload!;

            //Assert
            Assert.IsTrue(actual.IsSuccessful);
            Assert.IsNotNull(collabId);
            Assert.IsTrue(collabIdRemovedResult.IsSuccessful);
            Assert.IsNotNull(fileIds);
            Assert.IsTrue(newFileId.Count == 0);
        }

        [TestMethod]
        public async Task UpvoteAndDownvote()
        {
            //Arrange
            // second user
            string email = "test@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            // second user
            string email2 = "test2@gmail.com";
            await _registrationService.RegisterAccount(email2, password).ConfigureAwait(false);
            var accountIdResult2 = await _userAccountDataAccess.GetId(email2).ConfigureAwait(false);
            int accountId2 = accountIdResult2.Payload;

            // log in as user

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

            CreateCollaboratorDTO collab = MockCreateCollaboratorDTO();
            // get the collaborator id before logging off
            await _collaboratorManager.CreateCollaborator(collab).ConfigureAwait(false);
            var collabIdResult = await _collaboratorsDataAccess.GetCollaboratorId(accountId).ConfigureAwait(false);
            int collabId = (int)collabIdResult.Payload!;
            // Logging out of user
            var authenticationResult = _authenticationService.Logout();
            if (authenticationResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticationResult.Payload!, null);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            // log in as user 2

            loginResult = await _authenticationManager.Login(email2, password, dummyIp).ConfigureAwait(false);
            getOtp = await _otpDataAccess.GetOTP(accountId2).ConfigureAwait(false);
            otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);
            actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }



            // Act
            var getCollabResult = await _collaboratorManager.GetCollaborator(collabId).ConfigureAwait(false);
            var votesBefore = getCollabResult.Payload!.Votes;
            // upvote
            var actualUpvote = await _collaboratorManager.Vote(collabId, true);
            getCollabResult = await _collaboratorManager.GetCollaborator(collabId).ConfigureAwait(false);
            var votesDuring = getCollabResult.Payload!.Votes;
            // downvote
            var actualDownvote = await _collaboratorManager.Vote(collabId, false);
            getCollabResult = await _collaboratorManager.GetCollaborator(collabId).ConfigureAwait(false);
            var votesAfter = getCollabResult.Payload!.Votes;


            // Assert
            Assert.IsTrue(actualUpvote.IsSuccessful);
            Assert.IsTrue(actualDownvote.IsSuccessful);
            Assert.AreEqual(votesBefore, votesAfter);
            Assert.IsTrue(votesBefore == 0);
            Assert.IsTrue(votesDuring == 1);
            Assert.IsTrue(votesAfter == 0);
        }

        [TestMethod]
        public async Task RemoveOwnCollaborator()
        {
            //Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            // log in as user

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

            CreateCollaboratorDTO collab = MockCreateCollaboratorDTO();
            collab.UploadedFiles = new IFormFile[]
            {
                CreateMockFormFile(),
                CreateMockFormFile()
            };
            collab.PfpFile = CreateMockFormFile();

            await _collaboratorManager.CreateCollaborator(collab).ConfigureAwait(false);
            var collabIdResult = await _collaboratorsDataAccess.GetCollaboratorId(accountId).ConfigureAwait(false);
            int collabId = (int)collabIdResult.Payload!;

            //Act
            var actual = await _collaboratorManager.RemoveOwnCollaborator().ConfigureAwait(false);
            var actualCollab = await _collaboratorsDataAccess.GetCollaborator(collabId).ConfigureAwait(false);


            //Assert
            Assert.IsTrue(actual.IsSuccessful);
            Assert.IsFalse(_validationService.ValidateCollaborator(actualCollab.Payload!).IsSuccessful);
            Assert.IsTrue(string.IsNullOrEmpty(actualCollab.Payload!.PfpUrl));
            Assert.IsTrue(string.IsNullOrEmpty(actualCollab.Payload!.Name));
            Assert.IsTrue(string.IsNullOrEmpty(actualCollab.Payload!.ContactInfo));
            Assert.IsTrue(string.IsNullOrEmpty(actualCollab.Payload!.Availability));
            Assert.IsTrue(string.IsNullOrEmpty(actualCollab.Payload!.Description));
            Assert.IsFalse(actualCollab.Payload!.Published);
        }

        [TestMethod]
        public async Task DeleteCollaborator()
        {
            //Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            // log in as user

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

            CreateCollaboratorDTO collab = MockCreateCollaboratorDTO();
            collab.UploadedFiles = new IFormFile[]
            {
                CreateMockFormFile(),
                CreateMockFormFile()
            };
            collab.PfpFile = CreateMockFormFile();

            await _collaboratorManager.CreateCollaborator(collab).ConfigureAwait(false);
            var collabIdResult = await _collaboratorsDataAccess.GetCollaboratorId(accountId).ConfigureAwait(false);
            int collabId = (int)collabIdResult.Payload!;

            //Act
            var actual = await _collaboratorManager.DeleteCollaborator(collabId).ConfigureAwait(false);
            var actualCollab = await _collaboratorsDataAccess.GetCollaborator(collabId).ConfigureAwait(false);


            //Assert
            Assert.IsTrue(actual.IsSuccessful);
            Assert.IsFalse(actualCollab.IsSuccessful);
            Assert.IsTrue(actualCollab.Payload == null);
        }

        [TestMethod]
        public async Task DeleteCollaboratorWithNoChangeToOtherCollaborators()
        {
            //Arrange
            // first user
            string email = "test@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            // second user
            string email2 = "test2@gmail.com";
            await _registrationService.RegisterAccount(email2, password).ConfigureAwait(false);
            var accountIdResult2 = await _userAccountDataAccess.GetId(email2).ConfigureAwait(false);
            int accountId2 = accountIdResult2.Payload;

            // log in as user

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

            IFormFile file = CreateFormFileFromFilePath("C:\\Users\\NZXT ASRock\\Documents\\Senior Project\\SeniorProject\\src\\DevelopmentHell.Hubba\\Images\\rayquaza6.png");
            IFormFile[] uploadedFile = new IFormFile[] { file, file };

            CreateCollaboratorDTO collab = MockCreateCollaboratorDTO();
            collab.PfpFile = CreateMockFormFile();
            // creating first collaborator profile
            await _collaboratorManager.CreateCollaborator(collab).ConfigureAwait(false);
            // Logging out of user
            var authenticationResult = _authenticationService.Logout();
            if (authenticationResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticationResult.Payload!, null);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            // log in as user

            loginResult = await _authenticationManager.Login(email2, password, dummyIp).ConfigureAwait(false);
            getOtp = await _otpDataAccess.GetOTP(accountId2).ConfigureAwait(false);
             otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);
            actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            //creating second collaborator profile
            var secondCollabResult = await _collaboratorManager.CreateCollaborator(collab).ConfigureAwait(false);


            // get the second account's id
            var collabIdResult = await _collaboratorsDataAccess.GetCollaboratorId(accountId2).ConfigureAwait(false);
            int collabId = (int)collabIdResult.Payload!;
            var fileIdsResult = await _collaboratorFileDataAccess.SelectFileIdsFromOwner(accountId2).ConfigureAwait(false);
            List<int> fileIds = fileIdsResult.Payload!;

            //Act
            // second account is deleting its collaborator profile
            var actual = await _collaboratorManager.DeleteCollaborator(collabId).ConfigureAwait(false);
            var collabIdDeletedResult = await _collaboratorsDataAccess.GetCollaboratorId(accountId2).ConfigureAwait(false);
            var fileIdsResultDeleted = await _collaboratorFileDataAccess.SelectFileIdsFromOwner(accountId2).ConfigureAwait(false);
            List<int> fileIdsDeleted = fileIdsResultDeleted.Payload!;

            //Assert
            Assert.IsTrue(actual.IsSuccessful);
            Assert.IsNotNull(collabId);
            Assert.IsFalse(collabIdDeletedResult.IsSuccessful);
            Assert.IsNotNull(fileIds);
            Assert.IsTrue(fileIdsDeleted.Count == 0);
        }

        [TestMethod]
        public async Task FailureDeleteCollaboratorWithoutCollaborator()
        {
            //Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            // log in as user

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

            CreateCollaboratorDTO collab = MockCreateCollaboratorDTO();
            collab.UploadedFiles = new IFormFile[]
            {
                CreateMockFormFile(),
                CreateMockFormFile()
            };
            collab.PfpFile = CreateMockFormFile();

            await _collaboratorManager.CreateCollaborator(collab).ConfigureAwait(false);
            var collabIdResult = await _collaboratorsDataAccess.GetCollaboratorId(accountId).ConfigureAwait(false);
            int collabId = (int)collabIdResult.Payload!;

            await _collaboratorManager.DeleteCollaborator(collabId).ConfigureAwait(false);
            await _collaboratorsDataAccess.GetCollaborator(collabId).ConfigureAwait(false);


            //Act
            var actual = await _collaboratorManager.DeleteCollaborator(collabId).ConfigureAwait(false);
            var actualCollab = await _collaboratorsDataAccess.GetCollaborator(collabId).ConfigureAwait(false);


            //Assert
            Assert.IsTrue(collabIdResult.IsSuccessful);
            Assert.IsFalse(actual.IsSuccessful);
            Assert.IsFalse(actualCollab.IsSuccessful);
            Assert.IsTrue(actualCollab.Payload == null);
        }

        [TestMethod]
        public async Task FailureGetCollaboratorWithoutCollaborator()
        {
            //Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            // log in as user

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

            CreateCollaboratorDTO collab = MockCreateCollaboratorDTO();
            collab.UploadedFiles = new IFormFile[]
            {
                CreateMockFormFile(),
                CreateMockFormFile()
            };
            collab.PfpFile = CreateMockFormFile();

            await _collaboratorManager.CreateCollaborator(collab).ConfigureAwait(false);
            var collabIdResult = await _collaboratorsDataAccess.GetCollaboratorId(accountId).ConfigureAwait(false);
            int collabId = (int)collabIdResult.Payload!;

            await _collaboratorManager.DeleteCollaborator(collabId).ConfigureAwait(false);


            //Act
            var actual = await _collaboratorsDataAccess.GetCollaborator(collabId).ConfigureAwait(false);


            //Assert
            Assert.IsTrue(collabIdResult.IsSuccessful);
            Assert.IsFalse(actual.IsSuccessful);
            Assert.IsNull(actual.Payload);
        }

        [TestMethod]
        public async Task FailureVoteWithoutLoggingIn()
        {
            //Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            // log in as user
            
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

            CreateCollaboratorDTO collab = MockCreateCollaboratorDTO();
            collab.UploadedFiles = new IFormFile[]
            {
                CreateMockFormFile(),
                CreateMockFormFile()
            };
            collab.PfpFile = CreateMockFormFile();

            await _collaboratorManager.CreateCollaborator(collab).ConfigureAwait(false);
            var collabIdResult = await _collaboratorsDataAccess.GetCollaboratorId(accountId).ConfigureAwait(false);
            int collabId = (int)collabIdResult.Payload!;


            // Logging out of user
            var authenticationResult = _authenticationService.Logout();
            if (authenticationResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticationResult.Payload!, null);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }




            //Act
            var actual = await _collaboratorManager.Vote(collabId, true).ConfigureAwait(false);


            //Assert
            Assert.IsFalse(actual.IsSuccessful);
            Assert.AreEqual(actual.StatusCode, StatusCodes.Status401Unauthorized);
        }














        [TestCleanup]
        public async Task Cleanup()
        {
            await _testingService.DeleteAllRecords().ConfigureAwait(false);
            await _fileService.DeleteDir("/Collaborators").ConfigureAwait(false);
        }


        // Making an IFormFile for testing, it streams the location of the file into a file object
        public IFormFile CreateFormFileFromFilePath(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var stream = new FileStream(filePath, FileMode.Open);
            var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = GetContentType(fileName)
            };

            return formFile;
        }
        private string GetContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }

        // creates a dummy iformfile
        public  IFormFile CreateMockFormFile(string title = "mock")
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

        // Create dummy collaborator for testing
        public CreateCollaboratorDTO MockCreateCollaboratorDTO()
        {
            IFormFile[] uploadedFile = new IFormFile[] { CreateMockFormFile() };
            return new CreateCollaboratorDTO()
            {
                Name = "Bestest  woodworker",
                PfpFile = null,
                ContactInfo = "123 Avenue Wood Village, CA",
                Tags = "woodie, super strong, carpenter",
                Description = "I can build big old tables and chairs",
                Availability = "I'm free whenever",
                UploadedFiles = uploadedFile,
                Published = true
            };
        }

        // Edit collaborator 
        public EditCollaboratorDTO MockEditCollaboratorDTO()
        {
            IFormFile[] uploadedFile = new IFormFile[] { CreateMockFormFile() };
            return new EditCollaboratorDTO()
            {
                Name = "Bestest  woodworker",
                ContactInfo = "123 Avenue Wood Village, CA",
                Tags = "woodie, super strong, carpenter",
                Description = "I can build big old tables and chairs",
                Availability = "I'm free whenever",
                Published = true,
                PfpFile = null,
                UploadedFiles = uploadedFile,
            };
        }
    }
}
