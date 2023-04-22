
using Development.Hubba.JWTHandler.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Implementations;
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
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Identity.Client;
using System.Configuration;


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
        private readonly IAuthenticationService _authenticationService;
        private readonly ICollaboratorManager _collaboratorManager;
        private readonly ICollaboratorService _collaboratorService;
        private readonly ICollaboratorsDataAccess _collaboratorsDataAccess;
        private readonly ICollaboratorFileDataAccess _collaboratorFileDataAccess;
        private readonly ITestingService _testingService;
        private readonly IValidationService _validationService;

        public CollaboratorManagerIntegrationTests()
        {
            _userAccountDataAccess = new UserAccountDataAccess(_usersConnectionString, _userAccountsTable);
            _validationService = new ValidationService();
            ICryptographyService cryptographyService = new CryptographyService(ConfigurationManager.AppSettings["CryptographyKey"]!);
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
                cryptographyService,
                jwtHandlerService,
                _validationService,
                loggerService
            );
            _registrationService = new RegistrationService(
                _userAccountDataAccess,
               cryptographyService,
               _validationService,
               loggerService
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
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }

            IFormFile file = CreateFormFileFromFilePath("C:\\Users\\NZXT ASRock\\Documents\\Senior Project\\SeniorProject\\src\\DevelopmentHell.Hubba\\Images\\rayquaza0.png");
            IFormFile[] uploadedFile = new IFormFile[] { file };

            CollaboratorProfile collab = new CollaboratorProfile()
            {
                Name ="Bestest  woodworker",
                PfpUrl = null,
                ContactInfo = "123 Avenue Wood Village, CA",
                Tags = "woodie, super strong, carpenter",
                Description = "I can build big old tables and chairs",
                Availability = "I'm free whenever",
                Votes = 0,
                CollabUrls = new List<string>(),
                Published = true
            };


            //Act
            Result actual = await _collaboratorManager.CreateCollaborator(collab, uploadedFile).ConfigureAwait(false);

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
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }

            IFormFile file = CreateFormFileFromFilePath("C:\\Users\\NZXT ASRock\\Documents\\Senior Project\\SeniorProject\\src\\DevelopmentHell.Hubba\\Images\\rayquaza1.png");
            IFormFile[] uploadedFile = new IFormFile[] { file };

            CollaboratorProfile collab = new CollaboratorProfile()
            {
                Name = "Bestest  woodworker",
                PfpUrl = null,
                ContactInfo = "123 Avenue Wood Village, CA",
                Tags = "woodie, super strong, carpenter",
                Description = "I can build big old tables and chairs",
                Availability = "I'm free whenever",
                Votes = 0,
                CollabUrls = new List<string>(),
                Published = true
            };


            //Act
            Result actual = await _collaboratorManager.CreateCollaborator(collab, uploadedFile, uploadedFile[0]).ConfigureAwait(false);

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
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }

            IFormFile file = CreateFormFileFromFilePath("C:\\Users\\NZXT ASRock\\Documents\\Senior Project\\SeniorProject\\src\\DevelopmentHell.Hubba\\Images\\rayquaza2.png");
            IFormFile[] uploadedFile = new IFormFile[] { file, file };

            CollaboratorProfile collab = new CollaboratorProfile()
            {
                Name = "Bestest  woodworker",
                PfpUrl = null,
                ContactInfo = "123 Avenue Wood Village, CA",
                Tags = "woodie, super strong, carpenter",
                Description = "I can build big old tables and chairs",
                Availability = "I'm free whenever",
                Votes = 0,
                CollabUrls = new List<string>(),
                Published = true
            };


            //Act
            Result actual = await _collaboratorManager.CreateCollaborator(collab, uploadedFile, uploadedFile[0]).ConfigureAwait(false);

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
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }

            IFormFile file = CreateFormFileFromFilePath("C:\\Users\\NZXT ASRock\\Documents\\Senior Project\\SeniorProject\\src\\DevelopmentHell.Hubba\\Images\\rayquaza3.png");
            IFormFile[] uploadedFile = new IFormFile[] { file, file };

            CollaboratorProfile collab = new CollaboratorProfile()
            {
                Name = "Bestest  woodworker",
                PfpUrl = null,
                ContactInfo = "123 Avenue Wood Village, CA",
                Tags = "woodie, super strong, carpenter",
                Description = "I can build big old tables and chairs",
                Availability = "I'm free whenever",
                Votes = 0,
                CollabUrls = new List<string>(),
                Published = true
            };

            await _collaboratorManager.CreateCollaborator(collab, uploadedFile, uploadedFile[0]).ConfigureAwait(false);
            var collabIdResult = await _collaboratorsDataAccess.GetCollaboratorId(accountId).ConfigureAwait(false);
            int collabId = (int)collabIdResult.Payload!;

            //Act
            var actual = await _collaboratorManager.GetCollaborator(collabId).ConfigureAwait(false);

            //Assert
            Console.WriteLine(actual.ErrorMessage);
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
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }

            IFormFile file = CreateFormFileFromFilePath("C:\\Users\\NZXT ASRock\\Documents\\Senior Project\\SeniorProject\\src\\DevelopmentHell.Hubba\\Images\\rayquaza4.png");
            IFormFile[] uploadedFile = new IFormFile[] { file, file };
            IFormFile[] changedFile = new IFormFile[] { file };

            CollaboratorProfile collab = new CollaboratorProfile()
            {
                Name = "Bestest  woodworker",
                PfpUrl = null,
                ContactInfo = "123 Avenue Wood Village, CA",
                Tags = "woodie, super strong, carpenter",
                Description = "I can build big old tables and chairs",
                Availability = "I'm free whenever",
                Votes = 0,
                CollabUrls = new List<string>(),
                Published = true
            };

            CollaboratorProfile collabChanges = new CollaboratorProfile()
            {
                Name = "Meh  woodworker",
                PfpUrl = null,
                ContactInfo = "2020 Avenue Wood Village, CA",
                Tags = "alright, super okay",
                Description = "I can't build big old tables and chairs",
                Availability = "I'm free never",
                Votes = 0,
                CollabUrls = new List<string>(),
                Published = true
            };

            await _collaboratorManager.CreateCollaborator(collab, uploadedFile, uploadedFile[0]).ConfigureAwait(false);
            var removedFileIdsResult = await _collaboratorFileDataAccess.SelectFileIdsFromOwner(accountId).ConfigureAwait(false);
            List<int> removedFileIds = new();
            foreach(var fileId in removedFileIdsResult.Payload!)
            {
                removedFileIds.Add((int)fileId!);
            }
            var removedFileUrlsResult = await _collaboratorFileDataAccess.SelectFileUrls(removedFileIds).ConfigureAwait(false);
            string[] removedFileUrls = new string[removedFileIdsResult.Payload!.Count];
            for(int i = 0; i < removedFileUrls.Length; i++)
            {
                removedFileUrls[i] = removedFileUrlsResult.Payload![i];
            }

            //Act
            var actual = await _collaboratorManager.EditCollaborator(collabChanges, changedFile,
                removedFileUrls).ConfigureAwait(false);

            //Assert
            Console.WriteLine(actual.ErrorMessage);
            Assert.IsTrue(actual.IsSuccessful);
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
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }

            IFormFile file = CreateFormFileFromFilePath("C:\\Users\\NZXT ASRock\\Documents\\Senior Project\\SeniorProject\\src\\DevelopmentHell.Hubba\\Images\\rayquaza5.png");
            IFormFile[] uploadedFile = new IFormFile[] { file, file };
            IFormFile[] changedFile = new IFormFile[] { file };

            CollaboratorProfile collab = new CollaboratorProfile()
            {
                Name = "Bestest  woodworker",
                PfpUrl = null,
                ContactInfo = "123 Avenue Wood Village, CA",
                Tags = "woodie, super strong, carpenter",
                Description = "I can build big old tables and chairs",
                Availability = "I'm free whenever",
                Votes = 0,
                CollabUrls = new List<string>(),
                Published = true
            };

            CollaboratorProfile collabChanges = new CollaboratorProfile()
            {
                Name = "Meh  woodworker",
                PfpUrl = null,
                ContactInfo = "2020 Avenue Wood Village, CA",
                Tags = "alright, super okay",
                Description = "I can't build big old tables and chairs",
                Availability = "I'm free never",
                Votes = 0,
                CollabUrls = new List<string>(),
                Published = true
            };

            await _collaboratorManager.CreateCollaborator(collab, uploadedFile, uploadedFile[0]).ConfigureAwait(false);
            var removedFileIdsResult = await _collaboratorFileDataAccess.SelectFileIdsFromOwner(accountId).ConfigureAwait(false);
            List<int> removedFileIds = new();
            foreach (var fileId in removedFileIdsResult.Payload!)
            {
                removedFileIds.Add((int)fileId!);
            }
            var removedFileUrlsResult = await _collaboratorFileDataAccess.SelectFileUrls(removedFileIds).ConfigureAwait(false);
            string[] removedFileUrls = new string[removedFileIdsResult.Payload!.Count];
            for (int i = 0; i < removedFileUrls.Length; i++)
            {
                removedFileUrls[i] = removedFileUrlsResult.Payload![i];
            }

            //Act
            var actual = await _collaboratorManager.EditCollaborator(collabChanges, changedFile,
                removedFileUrls, changedFile[0]).ConfigureAwait(false);

            //Assert
            Console.WriteLine(actual.ErrorMessage);
            Assert.IsTrue(actual.IsSuccessful);
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

            // log in as user 1
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }

            IFormFile file = CreateFormFileFromFilePath("C:\\Users\\NZXT ASRock\\Documents\\Senior Project\\SeniorProject\\src\\DevelopmentHell.Hubba\\Images\\rayquaza6.png");
            IFormFile[] uploadedFile = new IFormFile[] { file, file };

            CollaboratorProfile collab = new CollaboratorProfile()
            {
                Name = "Bestest  woodworker",
                PfpUrl = null,
                ContactInfo = "123 Avenue Wood Village, CA",
                Tags = "woodie, super strong, carpenter",
                Description = "I can build big old tables and chairs",
                Availability = "I'm free whenever",
                Votes = 0,
                CollabUrls = new List<string>(),
                Published = true
            };
            // creating first collaborator profile
            await _collaboratorManager.CreateCollaborator(collab, uploadedFile, uploadedFile[0]).ConfigureAwait(false);
            _authenticationService.Logout();

            // log in as user 2
            var accessTokenResult2 = await _authorizationService.GenerateAccessToken(accountId2, false).ConfigureAwait(false);
            var idTokenResult2 = _authenticationService.GenerateIdToken(accountId2, accessTokenResult2.Payload!);
            if (accessTokenResult2.IsSuccessful && idTokenResult2.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult2.Payload!, idTokenResult2.Payload!);
            }

            //creating second collaborator profile
            var secondCollabResult = await _collaboratorManager.CreateCollaborator(collab, uploadedFile, uploadedFile[0]).ConfigureAwait(false);


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

        [TestCleanup]
        public async Task Cleanup()
        {
            await _testingService.DeleteAllRecords().ConfigureAwait(false);
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

        [TestMethod]
        public async Task UpdateVisibilityPublished()
        {
            //Arrange
            string email = "test@gmail.com";
            string password = "12345678";
            await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }

            IFormFile file = CreateFormFileFromFilePath("C:\\Users\\NZXT ASRock\\Documents\\Senior Project\\SeniorProject\\src\\DevelopmentHell.Hubba\\Images\\rayquaza7.png");
            IFormFile[] uploadedFile = new IFormFile[] { file, file };
            IFormFile[] changedFile = new IFormFile[] { file };

            CollaboratorProfile collab = new CollaboratorProfile()
            {
                Name = "Bestest  woodworker",
                PfpUrl = null,
                ContactInfo = "123 Avenue Wood Village, CA",
                Tags = "woodie, super strong, carpenter",
                Description = "I can build big old tables and chairs",
                Availability = "I'm free whenever",
                Votes = 0,
                CollabUrls = new List<string>(),
                Published = true
            };

            await _collaboratorManager.CreateCollaborator(collab, uploadedFile, uploadedFile[0]).ConfigureAwait(false);
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

            // log in as user 1
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }

            IFormFile file = CreateFormFileFromFilePath("C:\\Users\\NZXT ASRock\\Documents\\Senior Project\\SeniorProject\\src\\DevelopmentHell.Hubba\\Images\\rayquaza8.png");
            IFormFile[] uploadedFile = new IFormFile[] { file, file };

            CollaboratorProfile collab = new CollaboratorProfile()
            {
                Name = "Bestest  woodworker",
                PfpUrl = null,
                ContactInfo = "123 Avenue Wood Village, CA",
                Tags = "woodie, super strong, carpenter",
                Description = "I can build big old tables and chairs",
                Availability = "I'm free whenever",
                Votes = 0,
                CollabUrls = new List<string>(),
                Published = true
            };
            // creating first collaborator profile
            await _collaboratorManager.CreateCollaborator(collab, uploadedFile, uploadedFile[0]).ConfigureAwait(false);
            _authenticationService.Logout();

            // log in as user 2
            var accessTokenResult2 = await _authorizationService.GenerateAccessToken(accountId2, false).ConfigureAwait(false);
            var idTokenResult2 = _authenticationService.GenerateIdToken(accountId2, accessTokenResult2.Payload!);
            if (accessTokenResult2.IsSuccessful && idTokenResult2.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult2.Payload!, idTokenResult2.Payload!);
            }

            //creating second collaborator profile
            var secondCollabResult = await _collaboratorManager.CreateCollaborator(collab, uploadedFile, uploadedFile[0]).ConfigureAwait(false);


            // get the second account's id
            var collabIdResult = await _collaboratorsDataAccess.GetCollaboratorId(accountId2).ConfigureAwait(false);
            int collabId = (int)collabIdResult.Payload!;
            var fileIdsResult = await _collaboratorFileDataAccess.SelectFileIdsFromOwner(accountId2).ConfigureAwait(false);
            List<int> fileIds = fileIdsResult.Payload!;

            //Act
            // second account is removing its collaborator profile
            var actual = await _collaboratorManager.RemoveCollaborator(collabId).ConfigureAwait(false);
            var collabIdRemovedResult = await _collaboratorsDataAccess.GetCollaboratorId(accountId2).ConfigureAwait(false);
            var fileIdsResultRemoved = await _collaboratorFileDataAccess.SelectFileIdsFromOwner(accountId2).ConfigureAwait(false);
            List<int> fileIdsDeleted = fileIdsResultRemoved.Payload!;

            //Assert
            Assert.IsTrue(actual.IsSuccessful);
            Assert.IsNotNull(collabId);
            Assert.IsTrue(collabIdRemovedResult.IsSuccessful);
            Assert.IsNotNull(fileIds);
            Assert.IsTrue(fileIdsDeleted.Count == 0);
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
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }

            IFormFile file = CreateFormFileFromFilePath("C:\\Users\\NZXT ASRock\\Documents\\Senior Project\\SeniorProject\\src\\DevelopmentHell.Hubba\\Images\\rayquaza3.png");
            IFormFile[] uploadedFile = new IFormFile[] { file, file };

            CollaboratorProfile collab = new CollaboratorProfile()
            {
                Name = "Bestest  woodworker",
                PfpUrl = null,
                ContactInfo = "123 Avenue Wood Village, CA",
                Tags = "woodie, super strong, carpenter",
                Description = "I can build big old tables and chairs",
                Availability = "I'm free whenever",
                Votes = 0,
                CollabUrls = new List<string>(),
                Published = true
            };

            // get the collaborator id before logging off
            await _collaboratorManager.CreateCollaborator(collab, uploadedFile, uploadedFile[0]).ConfigureAwait(false);
            var collabIdResult = await _collaboratorsDataAccess.GetCollaboratorId(accountId).ConfigureAwait(false);
            int collabId = (int)collabIdResult.Payload!;
            _authenticationService.Logout();

            // log in as user 2
            var accessTokenResult2 = await _authorizationService.GenerateAccessToken(accountId2, false).ConfigureAwait(false);
            var idTokenResult2 = _authenticationService.GenerateIdToken(accountId2, accessTokenResult2.Payload!);
            if (accessTokenResult2.IsSuccessful && idTokenResult2.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult2.Payload!, idTokenResult2.Payload!);
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
            Assert.IsTrue(votesBefore == 0 && votesDuring == 1);
        }











        [TestCleanup]
        public async Task Cleanup()
        {
            await _testingService.DeleteAllRecords().ConfigureAwait(false);
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
    }
}
