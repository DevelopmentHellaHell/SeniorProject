using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Files.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.ProjectShowcase.Manager.Abstractions;
using DevelopmentHell.Hubba.ProjectShowcase.Manager.Implementations;
using DevelopmentHell.Hubba.ProjectShowcase.Service.Abstractions;
using DevelopmentHell.Hubba.ProjectShowcase.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using System.Configuration;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Authentication.Service.Implementations;
using Development.Hubba.JWTHandler.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.Files.Service.Implementations;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.AspNetCore.Mvc;


namespace DevelopmentHell.Hubba.ProjectShowcase.Test.Integration_Tests
{
    [TestClass]
    public class ProjectShowcaseIntegrationTests
    {
        private readonly string _usersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private readonly string _showcaseConnectionString = ConfigurationManager.AppSettings["ProjectShowcasesConnectionString"]!;
        private readonly string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private readonly string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private readonly string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        private readonly string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;
        private readonly string _showcasesTable = ConfigurationManager.AppSettings["ShowcasesTable"]!;
        private readonly string _showcaseCommentsTable = ConfigurationManager.AppSettings["ShowcaseCommentsTable"]!;
        private readonly string _showcaseVotesTable = ConfigurationManager.AppSettings["ShowcaseVotesTable"]!;
        private readonly string _showcaseCommentVotesTable = ConfigurationManager.AppSettings["ShowcaseCommentVotesTable"]!;
        private readonly string _showcaseReportsTable = ConfigurationManager.AppSettings["ShowcaseReportsTable"]!;
        private readonly string _showcaseCommentReportsTable = ConfigurationManager.AppSettings["ShowcaseCommentReportsTable"]!;
        private readonly string _ftpServer = ConfigurationManager.AppSettings["FTPServer"]!;
        private readonly string _ftpUsername = ConfigurationManager.AppSettings["FTPUsername"]!;
        private readonly string _ftpPassword = ConfigurationManager.AppSettings["FTPPassword"]!;

        private readonly IProjectShowcaseService _projectShowcaseService;
        private readonly IProjectShowcaseManager _projectShowcaseManager;
        private readonly IProjectShowcaseDataAccess _projectShowcaseDataAccess;
        private readonly IUserAccountDataAccess _userAccountDataAccess;
        private readonly IValidationService _validationService;
        private readonly ILoggerService _loggerService;
        private readonly IFileService _fileService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILoggerDataAccess _loggerDataAccess;
        private readonly IJWTHandlerService _jwtHandlerService;

        private readonly IRegistrationService _registrationService;

        private readonly IAuthenticationService _authenticationService;
        private readonly ITestingService _testingService;

        private readonly string email1 = "email1@mail.com";
        private readonly string email2 = "email2@mail.com";

        private readonly string showcaseId1 = "showcaseId1";
        private readonly string showcaseId2 = "showcaseId2";
        private readonly string showcaseId3 = "showcaseId3";

        private readonly string password = "12345678";


        // Create a test file
        string fileName = "test.txt";
        string fileContent = "This is a test file content";
        MemoryStream ms;
        Mock<IFormFile> file =  new Mock<IFormFile>();

        // Create a test file
        string fileName2 = "test2.txt";
        string fileContent2 = "This is a test file content";
        Mock<IFormFile> file2 = new Mock<IFormFile>();
        MemoryStream ms2;

        public ProjectShowcaseIntegrationTests()
        {
            _projectShowcaseDataAccess = new ProjectShowcaseDataAccess(
                _showcaseConnectionString,
                _showcasesTable,
                _showcaseCommentsTable,
                _showcaseVotesTable,
                _showcaseCommentVotesTable,
                _showcaseReportsTable,
                _showcaseCommentReportsTable);
            _userAccountDataAccess = new UserAccountDataAccess(
                _showcaseConnectionString,
                _showcasesTable);
            _validationService = new ValidationService();
            _loggerDataAccess = new LoggerDataAccess(
                _showcaseConnectionString,
                _showcasesTable);
            _loggerService = new LoggerService(
                _loggerDataAccess);
            _projectShowcaseService = new ProjectShowcaseService(
                _projectShowcaseDataAccess,
                _userAccountDataAccess,
                _validationService,
                _loggerService);
            _fileService = new FTPFileService(
                _ftpServer,
                _ftpUsername,
                _ftpPassword,
                _loggerService);
            _jwtHandlerService = new JWTHandlerService(
                _jwtKey);
            _authorizationService = new AuthorizationService(
                _userAccountDataAccess,
                _jwtHandlerService,
                _loggerService);
            _projectShowcaseManager = new ProjectShowcaseManager(
                _projectShowcaseService,
                _fileService,
                _loggerService,
                _authorizationService);

            _testingService = new TestingService(
                _jwtKey,
                new TestsDataAccess()
            );

            _authenticationService = new AuthenticationService(
                _userAccountDataAccess,
                new UserLoginDataAccess(
                    _usersConnectionString,
                    ConfigurationManager.AppSettings["UserLoginsTable"]!),
                new CryptographyService(ConfigurationManager.AppSettings["CryptographyKey"]!),
                _jwtHandlerService,
                _validationService,
                _loggerService
                );

            _registrationService = new RegistrationService(
                _userAccountDataAccess,
                new CryptographyService(ConfigurationManager.AppSettings["CryptographyKey"]!),
                _validationService,
                _loggerService
                );


            ms = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            ms2 = new MemoryStream(Encoding.UTF8.GetBytes(fileContent2));
            file.Setup(f => f.FileName).Returns(fileName);
            file.Setup(f => f.Length).Returns(ms.Length);
            file.Setup(f => f.OpenReadStream()).Returns(ms);
            file2.Setup(f => f.FileName).Returns(fileName2);
            file2.Setup(f => f.Length).Returns(ms2.Length);
            file2.Setup(f => f.OpenReadStream()).Returns(ms2);
        }

        [TestInitialize]
        public async Task Setup()
        {
            await _testingService.DeleteAllRecords().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task AddCommentSuccess()
        {
            //add timer
            var regResult = await _registrationService.RegisterAccount(email1, password);
            Assert.IsTrue(regResult.IsSuccessful);

            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId1, null, showcaseId1, "description", DateTime.UtcNow).ConfigureAwait(false);
            Assert.IsTrue(insertResult.IsSuccessful);

            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }

            var addResult = await _projectShowcaseManager.AddComment(showcaseId1, "new comment").ConfigureAwait(false);
            Assert.IsTrue(addResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetComments(showcaseId1, 100, 1).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);

            bool passed = false;
            foreach (var comment in getResult.Payload!)
            {
                if ((string)comment["Text"]! == "new comment")
                {
                    passed = true;
                    break;
                }
            }
            Assert.IsTrue(passed);
        }

        [TestMethod]
        public async Task CreateShowcaseSuccess()
        {
            //add timer
            var regResult = await _registrationService.RegisterAccount(email1, password);
            Assert.IsTrue(regResult.IsSuccessful);

            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            var insertResult = await _projectShowcaseManager.CreateShowcase(3, "title", "description", new() { file.Object });
            Assert.IsTrue(insertResult.IsSuccessful);

            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }

            var getResult = await _projectShowcaseDataAccess.GetShowcase(showcaseId1).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((string)getResult.Payload!["Title"] == "title");
        }

        [TestMethod]
        public async Task DeleteCommentSuccess()
        {
            //add timer
            var regResult = await _registrationService.RegisterAccount(email1, password);
            Assert.IsTrue(regResult.IsSuccessful);

            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            var insertResult = await _projectShowcaseManager.CreateShowcase(3, "title", "description", new() { file.Object });
            Assert.IsTrue(insertResult.IsSuccessful);

            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }

            var addResult = await _projectShowcaseDataAccess.AddComment(showcaseId1, accountId, "new comment", DateTime.UtcNow).ConfigureAwait(false);
            Assert.IsTrue(addResult.IsSuccessful);

            var commentResult = await _projectShowcaseDataAccess.GetComments(showcaseId1, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(commentResult.IsSuccessful);
            var commentId = Convert.ToInt32(commentResult.Payload![0]["Id"]);

            var deleteResult = await _projectShowcaseManager.DeleteComment(commentId).ConfigureAwait(false);
            Assert.IsTrue(deleteResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetComments(showcaseId1, 10, 1);
            Assert.IsTrue(getResult.Payload!.Count == 0);
        }
    }
}
