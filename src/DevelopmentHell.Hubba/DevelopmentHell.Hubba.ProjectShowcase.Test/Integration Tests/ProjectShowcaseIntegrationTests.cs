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
using System.ComponentModel.Design;
using DevelopmentHell.Hubba.ListingProfile.Service.Abstractions;
using DevelopmentHell.Hubba.ListingProfile.Service.Implementations;

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
        private readonly IListingProfileService _listingProfileService;
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

        private readonly string file1 = Convert.ToBase64String("This is a test file content 1".Select(c => (byte)c).ToArray());
        private readonly string file2 = Convert.ToBase64String("This is a test file content 2".Select(c => (byte)c).ToArray());
        private readonly string file1Name = "test1.txt";
        private readonly string file2Name = "test2.txt";

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
                _usersConnectionString,
                _userAccountsTable);
            _validationService = new ValidationService();
            _loggerDataAccess = new LoggerDataAccess(
                _logsConnectionString,
                _logsTable);
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
            _listingProfileService = new ListingProfileService(
                new ListingsDataAccess(
                    _showcaseConnectionString,
                    ConfigurationManager.AppSettings["ListingsTable"]!),
                new ListingAvailabilitiesDataAccess(
                    _showcaseConnectionString,
                    ConfigurationManager.AppSettings["ListingAvailabilitiesTable"]!),
                new ListingHistoryDataAccess(
                    _showcaseConnectionString,
                    ConfigurationManager.AppSettings["ListingHistoryTable"]!),
                new RatingDataAccess(
                    _showcaseConnectionString,
                    ConfigurationManager.AppSettings["ListingRatingsTable"]!),
                _userAccountDataAccess,
                _loggerService
                );

            _jwtHandlerService = new JWTHandlerService(
                _jwtKey);
            _authorizationService = new AuthorizationService(
                _userAccountDataAccess,
                _jwtHandlerService,
                _loggerService);
            _projectShowcaseManager = new ProjectShowcaseManager(
                _projectShowcaseService,
                _fileService,
                _listingProfileService,
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
            Assert.IsTrue(regResult.IsSuccessful, regResult.ErrorMessage);

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
            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            var insertResult = await _projectShowcaseManager.CreateShowcase(3, "title", "description", new() { new(file1Name, file1) });

            var getResult = await _projectShowcaseDataAccess.GetShowcase(insertResult.Payload!).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((string)getResult.Payload!["Title"] == "title");
        }

        [TestMethod]
        public async Task DeleteCommentSuccess()
        {
            var start = DateTime.UtcNow;
            var regResult = await _registrationService.RegisterAccount(email1, password);
            Assert.IsTrue(regResult.IsSuccessful);

            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId1, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult.IsSuccessful);


            var addResult = await _projectShowcaseDataAccess.AddComment(showcaseId1, accountId, "new comment", DateTime.UtcNow).ConfigureAwait(false);
            Assert.IsTrue(addResult.IsSuccessful);

            var commentResult = await _projectShowcaseDataAccess.GetComments(showcaseId1, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(commentResult.IsSuccessful);
            var commentId = Convert.ToInt32(commentResult.Payload![0]["Id"]);

            var deleteResult = await _projectShowcaseManager.DeleteComment(commentId).ConfigureAwait(false);
            Assert.IsTrue(deleteResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetComments(showcaseId1, 10, 1);
            Assert.IsTrue(getResult.Payload!.Count == 0);

            var timeElapsed = DateTime.UtcNow - start;
            Assert.IsTrue(timeElapsed.Seconds <= 3);
        }

        [TestMethod]
        public async Task DeleteShowcaseSuccess()
        {
            var start = DateTime.UtcNow;
            var regResult = await _registrationService.RegisterAccount(email1, password);
            Assert.IsTrue(regResult.IsSuccessful);

            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId1, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult.IsSuccessful);

            var deleteResult = await _projectShowcaseManager.DeleteShowcase(showcaseId1).ConfigureAwait(false);
            Assert.IsTrue(deleteResult.IsSuccessful);

            var getResult = await _projectShowcaseManager.GetUserShowcases(accountId).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);
            Assert.IsTrue(getResult.Payload!.Count == 0);

            var timeElapsed = DateTime.UtcNow - start;
            Assert.IsTrue(timeElapsed.Seconds <= 3);
        }

        [TestMethod]
        public async Task EditCommentSuccess()
        {
            var start = DateTime.UtcNow;
            var regResult = await _registrationService.RegisterAccount(email1, password);
            Assert.IsTrue(regResult.IsSuccessful);
            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId1, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult.IsSuccessful);
            var addResult = await _projectShowcaseDataAccess.AddComment(showcaseId1, accountId, "new comment", DateTime.UtcNow).ConfigureAwait(false);
            Assert.IsTrue(addResult.IsSuccessful);
            var commentResult = await _projectShowcaseDataAccess.GetComments(showcaseId1, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(commentResult.IsSuccessful);
            var commentId = Convert.ToInt32(commentResult.Payload![0]["Id"]);
            var editResult = await _projectShowcaseManager.EditComment(commentId, "edited comment").ConfigureAwait(false);
            Assert.IsTrue(editResult.IsSuccessful);
            var getResult = await _projectShowcaseDataAccess.GetComments(showcaseId1, 10, 1);
            Assert.IsTrue((string)getResult.Payload![0]["Text"] == "edited comment");
            var timeElapsed = DateTime.UtcNow - start;
            Assert.IsTrue(timeElapsed.Seconds <= 3);
        }

        [TestMethod]
        public async Task EditShowcaseSuccess()
        {
            var start = DateTime.UtcNow;
            var regResult = await _registrationService.RegisterAccount(email1, password);
            Assert.IsTrue(regResult.IsSuccessful);
            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId1, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult.IsSuccessful);

            var editResult = await _projectShowcaseManager.EditShowcase(showcaseId1, null, "New Title", null, null);
            Assert.IsTrue(editResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetShowcase(showcaseId1);
            Assert.IsTrue(getResult.IsSuccessful);
            Assert.IsTrue((string)getResult.Payload!["Title"] == "New Title");
            var timeElapsed = DateTime.UtcNow - start;
            Assert.IsTrue(timeElapsed.Seconds <= 3);
        }

        [TestMethod]
        public async Task GetCommentsSuccess()
        {
            var start = DateTime.UtcNow;
            var regResult = await _registrationService.RegisterAccount(email1, password);
            Assert.IsTrue(regResult.IsSuccessful);
            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId1, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult.IsSuccessful);

            var insertCommentResult = await _projectShowcaseDataAccess.AddComment(showcaseId1, accountId, "Test Comment 1", DateTime.UtcNow);
            Assert.IsTrue(insertCommentResult.IsSuccessful);
            insertCommentResult = await _projectShowcaseDataAccess.AddComment(showcaseId1, accountId, "Test Comment 2", DateTime.UtcNow.AddSeconds(2));
            Assert.IsTrue(insertCommentResult.IsSuccessful);

            var getComments = await _projectShowcaseManager.GetComments(showcaseId1).ConfigureAwait(false);
            Assert.IsTrue(getComments.IsSuccessful);

            bool passed1 = false;
            bool passed2 = false;
            foreach (var comment in getComments.Payload!)
            {
                if ((string)comment.Text! == "Test Comment 1")
                {
                    passed1 = true;
                }
                if ((string)comment.Text! == "Test Comment 2")
                {
                    passed2 = true;
                }
                if (passed1 && passed2)
                {
                    Assert.IsTrue(passed1 && passed2);
                    break;
                }
            }
            var timeElapsed = DateTime.UtcNow - start;
            Assert.IsTrue(timeElapsed.Seconds <= 3);
        }

        [TestMethod]
        public async Task GetCommentSuccess()
        {
            var start = DateTime.UtcNow;
            var regResult = await _registrationService.RegisterAccount(email1, password);
            Assert.IsTrue(regResult.IsSuccessful);
            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId1, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult.IsSuccessful);

            var insertCommentResult = await _projectShowcaseDataAccess.AddComment(showcaseId1, accountId, "Test Comment 1", DateTime.UtcNow);
            Assert.IsTrue(insertCommentResult.IsSuccessful);
            insertCommentResult = await _projectShowcaseDataAccess.AddComment(showcaseId1, accountId, "Test Comment 2", DateTime.UtcNow.AddSeconds(2));
            Assert.IsTrue(insertCommentResult.IsSuccessful);

            var commentResult = await _projectShowcaseDataAccess.GetComments(showcaseId1, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(commentResult.IsSuccessful);
            var commentId = Convert.ToInt32(commentResult.Payload![0]["Id"]);

            var getComment = await _projectShowcaseManager.GetComment(commentId).ConfigureAwait(false);
            Assert.IsTrue(getComment.IsSuccessful);

            Assert.IsTrue((string)getComment.Payload!.Text! == "Test Comment 1");
            var timeElapsed = DateTime.UtcNow - start;
            Assert.IsTrue(timeElapsed.Seconds <= 3);
        }

        [TestMethod]
        public async Task GetAllCommentReportsSuccess()
        {
            var start = DateTime.UtcNow;
            var regResult = await _registrationService.RegisterAccount(email1, password, "AdminUser");
            Assert.IsTrue(regResult.IsSuccessful);
            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId1, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult.IsSuccessful);

            var insertCommentResult = await _projectShowcaseDataAccess.AddComment(showcaseId1, accountId, "Test Comment 1", DateTime.UtcNow);
            Assert.IsTrue(insertCommentResult.IsSuccessful);
            insertCommentResult = await _projectShowcaseDataAccess.AddComment(showcaseId1, accountId, "Test Comment 2", DateTime.UtcNow.AddSeconds(2));
            Assert.IsTrue(insertCommentResult.IsSuccessful);

            var commentResult = await _projectShowcaseDataAccess.GetComments(showcaseId1, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(commentResult.IsSuccessful);
            var commentId = Convert.ToInt32(commentResult.Payload![0]["Id"]);
            var commentId2 = Convert.ToInt32(commentResult.Payload![1]["Id"]);


            var addCommentReportResult = await _projectShowcaseDataAccess.AddCommentReport(commentId, accountId, "reason", DateTime.UtcNow).ConfigureAwait(false);
            Assert.IsTrue(addCommentReportResult.IsSuccessful);
            var addCommentReportResult2 = await _projectShowcaseDataAccess.AddCommentReport(commentId2, accountId, "reason2", DateTime.UtcNow.AddSeconds(2)).ConfigureAwait(false);
            Assert.IsTrue(addCommentReportResult2.IsSuccessful);

            var getResult = await _projectShowcaseManager.GetAllCommentReports().ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful, getResult.ErrorMessage);

            bool passed1 = false;
            bool passed2 = false;
            foreach (var comment in getResult.Payload!)
            {
                if ((string)comment.Reason! == "reason")
                {
                    passed1 = true;
                }
                if ((string)comment.Reason! == "reason2")
                {
                    passed2 = true;
                }
                if (passed1 && passed2)
                {
                    Assert.IsTrue(passed1 && passed2);
                    break;
                }
            }
            var timeElapsed = DateTime.UtcNow - start;
            Assert.IsTrue(timeElapsed.Seconds <= 3);
        }

        [TestMethod]
        public async Task GetCommentReportsSuccess()
        {
            var start = DateTime.UtcNow;
            var regResult = await _registrationService.RegisterAccount(email1, password, "AdminUser");
            Assert.IsTrue(regResult.IsSuccessful);
            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId1, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult.IsSuccessful);

            var insertCommentResult = await _projectShowcaseDataAccess.AddComment(showcaseId1, accountId, "Test Comment 1", DateTime.UtcNow);
            Assert.IsTrue(insertCommentResult.IsSuccessful);
            insertCommentResult = await _projectShowcaseDataAccess.AddComment(showcaseId1, accountId, "Test Comment 2", DateTime.UtcNow.AddSeconds(2));
            Assert.IsTrue(insertCommentResult.IsSuccessful);

            var commentResult = await _projectShowcaseDataAccess.GetComments(showcaseId1, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(commentResult.IsSuccessful);
            var commentId = Convert.ToInt32(commentResult.Payload![0]["Id"]);
            var commentId2 = Convert.ToInt32(commentResult.Payload![1]["Id"]);


            var addCommentReportResult = await _projectShowcaseDataAccess.AddCommentReport(commentId, accountId, "reason", DateTime.UtcNow).ConfigureAwait(false);
            Assert.IsTrue(addCommentReportResult.IsSuccessful);
            var addCommentReportResult2 = await _projectShowcaseDataAccess.AddCommentReport(commentId2, accountId, "reason2", DateTime.UtcNow.AddSeconds(2)).ConfigureAwait(false);
            Assert.IsTrue(addCommentReportResult2.IsSuccessful);

            var getResult = await _projectShowcaseManager.GetCommentReports(commentId).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);

            bool passed1 = false;
            foreach (var comment in getResult.Payload!)
            {
                if ((string)comment.Reason! == "reason")
                {
                    passed1 = true;
                    Assert.IsTrue(passed1);
                    break;
                }
            }
            var timeElapsed = DateTime.UtcNow - start;
            Assert.IsTrue(timeElapsed.Seconds <= 3);
        }

        [TestMethod]
        public async Task GetShowcaseSuccess()
        {
            var start = DateTime.UtcNow;
            var regResult = await _registrationService.RegisterAccount(email1, password);
            Assert.IsTrue(regResult.IsSuccessful);
            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId1, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult.IsSuccessful);

            var getResult = await _projectShowcaseManager.GetShowcase(showcaseId1);
            Assert.IsTrue(getResult.IsSuccessful);
            Assert.IsTrue((string)getResult.Payload!.Title! == "title");
            var timeElapsed = DateTime.UtcNow - start;
            Assert.IsTrue(timeElapsed.Seconds <= 3);

            //also test if it got comments?
        }

        [TestMethod]
        public async Task GetUserShowcasesSuccess()
        {
            var start = DateTime.UtcNow;
            var regResult = await _registrationService.RegisterAccount(email1, password, "AdminUser");
            Assert.IsTrue(regResult.IsSuccessful);
            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId1, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult.IsSuccessful);
            var insertResult2 = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId2, 3, "title2", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult2.IsSuccessful);

            var getResult = await _projectShowcaseManager.GetUserShowcases(accountId).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);
            Assert.IsTrue((string)getResult.Payload![0].Title! == "title");
            Assert.IsTrue((string)getResult.Payload![1].Title! == "title2");

            var timeElapsed = DateTime.UtcNow - start;
            Assert.IsTrue(timeElapsed.Seconds <= 3);
        }

        [TestMethod]
        public async Task GetAllShowcaseReportsSuccess()
        {
            var start = DateTime.UtcNow;
            var regResult = await _registrationService.RegisterAccount(email1, password, "AdminUser");
            Assert.IsTrue(regResult.IsSuccessful);
            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId1, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult.IsSuccessful);
            var insertResult2 = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId2, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult2.IsSuccessful);

            var reportResult1 = await _projectShowcaseDataAccess.AddShowcaseReport(showcaseId1, accountId, "reason", DateTime.UtcNow).ConfigureAwait(false);
            Assert.IsTrue(reportResult1.IsSuccessful);
            var reportResult2 = await _projectShowcaseDataAccess.AddShowcaseReport(showcaseId2, accountId, "reason2", DateTime.UtcNow).ConfigureAwait(false);
            Assert.IsTrue(reportResult2.IsSuccessful);

            var getResult = await _projectShowcaseManager.GetAllShowcaseReports().ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);
            Assert.IsTrue((string)getResult.Payload![0].Reason! == "reason");
            Assert.IsTrue((string)getResult.Payload![1].Reason! == "reason2");

            var timeElapsed = DateTime.UtcNow - start;
            Assert.IsTrue(timeElapsed.Seconds <= 3);
        }

        [TestMethod]
        public async Task GetShowcaseReportsSuccess()
        {
            var start = DateTime.UtcNow;
            var regResult = await _registrationService.RegisterAccount(email1, password, "AdminUser");
            Assert.IsTrue(regResult.IsSuccessful);
            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId1, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult.IsSuccessful);
            var insertResult2 = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId2, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult2.IsSuccessful);
            var reportResult1 = await _projectShowcaseDataAccess.AddShowcaseReport(showcaseId1, accountId, "reason", DateTime.UtcNow).ConfigureAwait(false);
            Assert.IsTrue(reportResult1.IsSuccessful);
            var reportResult2 = await _projectShowcaseDataAccess.AddShowcaseReport(showcaseId2, accountId, "reason2", DateTime.UtcNow).ConfigureAwait(false);
            Assert.IsTrue(reportResult2.IsSuccessful);

            var getResult = await _projectShowcaseManager.GetShowcaseReports(showcaseId1).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);
            Assert.IsTrue((string)getResult.Payload![0].Reason! == "reason");

            var timeElapsed = DateTime.UtcNow - start;
            Assert.IsTrue(timeElapsed.Seconds <= 3);
        }

        [TestMethod]
        public async Task LikeShowcaseSuccess()
        {
            var start = DateTime.UtcNow;
            var regResult = await _registrationService.RegisterAccount(email1, password);
            Assert.IsTrue(regResult.IsSuccessful);
            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId1, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult.IsSuccessful);

            var likeResult = await _projectShowcaseManager.LikeShowcase(showcaseId1).ConfigureAwait(false);
            Assert.IsTrue(likeResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetDetails(showcaseId1).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((double)getResult.Payload!["Rating"] == 1);
            var timeElapsed = DateTime.UtcNow - start;
            Assert.IsTrue(timeElapsed.Seconds <= 3);
        }

        [TestMethod]

        public async Task PublishSuccess()
        {
            var start = DateTime.UtcNow;
            var regResult = await _registrationService.RegisterAccount(email1, password);
            Assert.IsTrue(regResult.IsSuccessful);
            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId1, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult.IsSuccessful);

            var publishResult = await _projectShowcaseManager.Publish(showcaseId1, 3).ConfigureAwait(false);
            Assert.IsTrue(publishResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetDetails(showcaseId1).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((bool)getResult.Payload!["IsPublished"] == true);
            var timeElapsed = DateTime.UtcNow - start;
            Assert.IsTrue(timeElapsed.Seconds <= 3);
        }

        [TestMethod]

        public async Task RateCommentSuccess()
        {
            var start = DateTime.UtcNow;
            var regResult = await _registrationService.RegisterAccount(email1, password);
            Assert.IsTrue(regResult.IsSuccessful);
            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId1, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult.IsSuccessful);

            var insertCommentResult = await _projectShowcaseDataAccess.AddComment(showcaseId1, accountId, "Test Comment 1", DateTime.UtcNow);
            Assert.IsTrue(insertCommentResult.IsSuccessful);
            insertCommentResult = await _projectShowcaseDataAccess.AddComment(showcaseId1, accountId, "Test Comment 2", DateTime.UtcNow.AddSeconds(2));
            Assert.IsTrue(insertCommentResult.IsSuccessful);

            var commentResult = await _projectShowcaseDataAccess.GetComments(showcaseId1, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(commentResult.IsSuccessful);
            var commentId = Convert.ToInt32(commentResult.Payload![0]["Id"]);

            var rateResult = await _projectShowcaseManager.RateComment(commentId, true).ConfigureAwait(false);
            Assert.IsTrue(rateResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetCommentDetails(commentId).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((int)getResult.Payload!["Rating"] == 1);
            var timeElapsed = DateTime.UtcNow - start;
            Assert.IsTrue(timeElapsed.Seconds <= 3);
        }

        [TestMethod]
        public async Task ReportCommentSuccess()
        {
            var start = DateTime.UtcNow;
            var regResult = await _registrationService.RegisterAccount(email1, password);
            Assert.IsTrue(regResult.IsSuccessful);
            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;
            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId1, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult.IsSuccessful);

            var insertCommentResult = await _projectShowcaseDataAccess.AddComment(showcaseId1, accountId, "Test Comment 1", DateTime.UtcNow);
            Assert.IsTrue(insertCommentResult.IsSuccessful);

            var commentResult = await _projectShowcaseDataAccess.GetComments(showcaseId1, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(commentResult.IsSuccessful);
            var commentId = Convert.ToInt32(commentResult.Payload![0]["Id"]);


            var reportResult = await _projectShowcaseManager.ReportComment(commentId, "reason").ConfigureAwait(false);
            Assert.IsTrue(reportResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetCommentReports(commentId).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);

            bool passed1 = false;
            foreach (var comment in getResult.Payload!)
            {
                if ((string)comment["Reason"]! == "reason")
                {
                    passed1 = true;
                    Assert.IsTrue(passed1);
                    break;
                }
            }
            var timeElapsed = DateTime.UtcNow - start;
            Assert.IsTrue(timeElapsed.Seconds <= 3);
        }

        [TestMethod]
        public async Task ReportShowcaseSuccess()
        {
            var start = DateTime.UtcNow;
            var regResult = await _registrationService.RegisterAccount(email1, password);
            Assert.IsTrue(regResult.IsSuccessful);
            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId1, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult.IsSuccessful);

            var reportResult1 = await _projectShowcaseManager.ReportShowcase(showcaseId1, "reason").ConfigureAwait(false);
            Assert.IsTrue(reportResult1.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetShowcaseReports(showcaseId1).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);
            Assert.IsTrue((string)getResult.Payload![0]["Reason"] == "reason");

            var timeElapsed = DateTime.UtcNow - start;
            Assert.IsTrue(timeElapsed.Seconds <= 3);
        }

        [TestMethod]
        public async Task UnlinkSuccess()
        {
            var start = DateTime.UtcNow;
            var regResult = await _registrationService.RegisterAccount(email1, password);
            Assert.IsTrue(regResult.IsSuccessful);
            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId1, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult.IsSuccessful);

            var unlinkResult = await _projectShowcaseManager.Unlink(showcaseId1).ConfigureAwait(false);
            Assert.IsTrue(unlinkResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetDetails(showcaseId1).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue(getResult.Payload!["ListingId"] == DBNull.Value);
            var timeElapsed = DateTime.UtcNow - start;
            Assert.IsTrue(timeElapsed.Seconds <= 3);
        }

        [TestMethod]
        public async Task UnpublishSuccess()
        {
            var start = DateTime.UtcNow;
            var regResult = await _registrationService.RegisterAccount(email1, password);
            Assert.IsTrue(regResult.IsSuccessful);
            await _registrationService.RegisterAccount(email1, password).ConfigureAwait(false);
            var accountIdResult = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int accountId = accountIdResult.Payload;

            // log in as user
            var accessTokenResult = await _authorizationService.GenerateAccessToken(accountId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(accountId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(accountId, showcaseId1, 3, "title", "description", DateTime.UtcNow);
            Assert.IsTrue(insertResult.IsSuccessful);

            var unpublishResult = await _projectShowcaseManager.Unpublish(showcaseId1).ConfigureAwait(false);
            Assert.IsTrue(unpublishResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetDetails(showcaseId1).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((bool)getResult.Payload!["IsPublished"] == false);
            var timeElapsed = DateTime.UtcNow - start;
            Assert.IsTrue(timeElapsed.Seconds <= 3);
        }
    }
}
