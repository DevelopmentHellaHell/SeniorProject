using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace DevelopmentHell.Hubba.ProjectShowcase.Test.Unit_Tests
{
    [TestClass]
    public class ProjectShowcaseDataAccessUnitTests
    {
        private string _usersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private string _showcaseConnectionString = ConfigurationManager.AppSettings["ProjectShowcasesConnectionString"]!;
        private string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;
        private readonly string _showcasesTable = ConfigurationManager.AppSettings["ShowcasesTable"]!;
        private readonly string _showcaseCommentsTable = ConfigurationManager.AppSettings["ShowcaseCommentsTable"]!;
        private readonly string _showcaseVotesTable = ConfigurationManager.AppSettings["ShowcaseVotesTable"]!;
        private readonly string _showcaseCommentVotesTable = ConfigurationManager.AppSettings["ShowcaseCommentVotesTable"]!;
        private readonly string _showcaseReportsTable = ConfigurationManager.AppSettings["ShowcaseReportsTable"]!;
        private readonly string _showcaseCommentReportsTable = ConfigurationManager.AppSettings["ShowcaseCommentReportsTable"]!;

        private readonly IUserAccountDataAccess _userAccountDataAccess;
        private readonly IRegistrationService _registrationService;
        private readonly ITestingService _testingService;

        private readonly IProjectShowcaseDataAccess _projectShowcaseDataAccess;

        public ProjectShowcaseDataAccessUnitTests()
        {
            ILoggerService loggerService = new LoggerService(
                new LoggerDataAccess(_logsConnectionString, _logsTable)
            );
            IValidationService validationService = new ValidationService();
            ICryptographyService cryptographyService = new CryptographyService(ConfigurationManager.AppSettings["CryptographyKey"]!);
            _userAccountDataAccess = new UserAccountDataAccess(_usersConnectionString, _userAccountsTable);
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
            _projectShowcaseDataAccess = new ProjectShowcaseDataAccess(
                _showcaseConnectionString,
                _showcasesTable,
                _showcaseCommentsTable,
                _showcaseVotesTable,
                _showcaseCommentVotesTable,
                _showcaseReportsTable,
                _showcaseCommentReportsTable
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
            DateTime time = DateTime.UtcNow;
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialPassword = "12345678";

            string showcaseId = $"showcaseId{time.Millisecond}";
            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            string commentText = "Test Comment";

            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            var accountId = userIdResult.Payload;

            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult.IsSuccessful);


            var commentResult = await _projectShowcaseDataAccess.AddComment(showcaseId, accountId, commentText, time);

            Assert.IsTrue(commentResult.IsSuccessful);
            var getResult = await _projectShowcaseDataAccess.GetComments(showcaseId, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((string)getResult.Payload![0][$"{_showcasesTable}.Id"] == showcaseId);
            Assert.IsTrue((int)getResult.Payload![0][$"{_userAccountsTable}.Id"] == accountId);
            Assert.IsTrue((string)getResult.Payload![0]["Text"] == commentText);
            Assert.IsTrue((DateTime)getResult.Payload![0]["EditTimestamp"]! == time);
            Assert.IsTrue((int)getResult.Payload![0]["Rating"] == 0);
        }

        [TestMethod]
        public async Task AddCommentReportSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialPassword = "12345678";

            string showcaseId = $"showcaseId{time.Millisecond}";
            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            string commentText = "Test Comment";
            string reason = "Test Reason";

            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            var accountId = userIdResult.Payload;

            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult.IsSuccessful,insertResult.ErrorMessage);

            var commentResult = await _projectShowcaseDataAccess.AddComment(showcaseId, accountId, commentText, time);
            Assert.IsTrue(commentResult.IsSuccessful, commentResult.ErrorMessage);
            var getCommentResult = await _projectShowcaseDataAccess.GetComments(showcaseId, 10, 1).ConfigureAwait(false);
            var commentId = (int)getCommentResult.Payload![0]["Id"];

            var commentReportResult = await _projectShowcaseDataAccess.AddCommentReport(commentId, accountId, reason, time);
            Assert.IsTrue(commentReportResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetCommentReports(commentId);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((int)getResult.Payload![0][$"{_showcaseCommentsTable}.Id"] == commentId);
            Assert.IsTrue((int)getResult.Payload![0][$"{_userAccountsTable}.Id"] == accountId);
            Assert.IsTrue((string)getResult.Payload![0]["Reason"] == reason);
            Assert.IsTrue((DateTime)getResult.Payload![0]["Timestamp"]! == time);
            Assert.IsTrue((bool)getResult.Payload![0]["IsResolved"] == false);
        }

        [TestMethod]
        public async Task InsertGetShowcaseSuccess()
        {
            DateTime time = DateTime.UtcNow;
            var credentialEmail = $"test{time.Millisecond}@gmail.com";
            var credentialPassword = "12345678";
            var showcaseId = $"showcaseId{time.Millisecond}";
            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";
            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            var accountId = userIdResult.Payload;

            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult.IsSuccessful);
            var getResult = await _projectShowcaseDataAccess.GetShowcase("showcaseId").ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);
            Assert.IsTrue((string)getResult.Payload![$"{_showcasesTable}.Id"] == showcaseId);
            Assert.IsTrue((string)getResult.Payload!["Email"] == credentialEmail);
            Assert.IsTrue((int)getResult.Payload![$"{_userAccountsTable}.Id"] == accountId);
            Assert.IsTrue((int)getResult.Payload!["ListingId"] == listingId);
            Assert.IsTrue((string)getResult.Payload!["Title"] == title);
            Assert.IsTrue((string)getResult.Payload!["Description"] == description);
            Assert.IsTrue((bool)getResult.Payload!["IsPublished"] == false);
            Assert.IsTrue((int)getResult.Payload!["Rating"] == 0);
            Assert.IsTrue((DateTime)getResult.Payload!["EditTimestamp"]! == time);
        }

        [TestMethod]
        public async Task AddShowcaseReportSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialPassword = "12345678";

            string showcaseId = $"showcaseId{time.Millisecond}";
            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            string reason = "Test Reason";

            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            var accountId = userIdResult.Payload;

            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult.IsSuccessful);

            var showcaseReportResult = await _projectShowcaseDataAccess.AddShowcaseReport(showcaseId, accountId, reason, time);
            Assert.IsTrue(showcaseReportResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetShowcaseReports(showcaseId);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((string)getResult.Payload![0][$"{_showcaseCommentsTable}.Id"] == showcaseId);
            Assert.IsTrue((int)getResult.Payload![0][$"{_userAccountsTable}.Id"] == accountId);
            Assert.IsTrue((string)getResult.Payload![0]["Reason"] == reason);
            Assert.IsTrue((DateTime)getResult.Payload![0]["Timestamp"]! == time);
            Assert.IsTrue((bool)getResult.Payload![0]["IsResolved"] == false);
        }

        [TestMethod]
        public async Task ChangePublishStatusSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialPassword = "12345678";

            string showcaseId = $"showcaseId{time.Millisecond}";
            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            bool isPublished = true;

            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            var accountId = userIdResult.Payload;

            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult.IsSuccessful);

            var changePublishStatusResult = await _projectShowcaseDataAccess.ChangePublishStatus(showcaseId, isPublished, time);
            Assert.IsTrue(changePublishStatusResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetDetails(showcaseId);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((bool)getResult.Payload!["IsPublished"] == !isPublished);
        }

        [TestMethod]
        public async Task DeleteCommentSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialPassword = "12345678";

            string showcaseId = $"showcaseId{time.Millisecond}";
            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            string commentText = "Test Comment";

            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            var accountId = userIdResult.Payload;

            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult.IsSuccessful);

            var commentResult = await _projectShowcaseDataAccess.AddComment(showcaseId, accountId, commentText, time);
            Assert.IsTrue(commentResult.IsSuccessful);
            var getCommentResult = await _projectShowcaseDataAccess.GetComments(showcaseId, 10, 1).ConfigureAwait(false);
            var commentId = (int)getCommentResult.Payload![0][$"{_showcaseCommentsTable}.Id"];

            var deleteCommentResult = await _projectShowcaseDataAccess.DeleteComment(commentId);
            Assert.IsTrue(deleteCommentResult.IsSuccessful);


            var getResult = await _projectShowcaseDataAccess.GetComments(showcaseId, 10, 1);
            Assert.IsTrue(getResult.Payload!.Count == 0);
        }

        [TestMethod]
        public async Task DeleteShowcaseSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialPassword = "12345678";

            string showcaseId = $"showcaseId{time.Millisecond}";
            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            var accountId = userIdResult.Payload;

            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult.IsSuccessful);

            var deleteShowcaseResult = await _projectShowcaseDataAccess.DeleteShowcase(showcaseId);
            Assert.IsTrue(deleteShowcaseResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetShowcase(showcaseId);
            Assert.IsTrue(getResult.Payload!.Count == 0);
        }

        [TestMethod]
        public async Task EditShowcaseSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialPassword = "12345678";

            string showcaseId = $"showcaseId{time.Millisecond}";
            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            string editedTitle = "Edited Title";
            string editedDescription = "Edited Description";

            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            var accountId = userIdResult.Payload;

            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult.IsSuccessful);

            var editShowcaseResult = await _projectShowcaseDataAccess.EditShowcase(showcaseId, editedTitle, editedDescription, time);
            Assert.IsTrue(editShowcaseResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetDetails(showcaseId);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((string)getResult.Payload!["Title"] == editedTitle);
            Assert.IsTrue((string)getResult.Payload!["Description"] == editedDescription);
            Assert.IsTrue((DateTime)getResult.Payload!["Timestamp"]! == time);
        }

        [TestMethod]
        public async Task GetCommentDetailsSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialPassword = "12345678";

            string showcaseId = $"showcaseId{time.Millisecond}";
            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            string commentText = "Test Comment";

            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            var accountId = userIdResult.Payload;

            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult.IsSuccessful);

            var commentResult = await _projectShowcaseDataAccess.AddComment(showcaseId, accountId, commentText, time);
            Assert.IsTrue(commentResult.IsSuccessful);
            var getCommentResult = await _projectShowcaseDataAccess.GetComments(showcaseId, 10, 1).ConfigureAwait(false);
            var commentId = (int)getCommentResult.Payload![0][$"{_showcaseCommentsTable}.Id"];

            var getResult = await _projectShowcaseDataAccess.GetCommentDetails(commentId);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((string)getResult.Payload![$"{_showcasesTable}.Id"] == showcaseId);
            Assert.IsTrue((string)getResult.Payload!["Email"] == credentialEmail);
            Assert.IsTrue((int)getResult.Payload![$"{_userAccountsTable}.Id"] == accountId);
            Assert.IsTrue((int)getResult.Payload!["CommenterId"] == accountId);
            Assert.IsTrue((int)getResult.Payload!["ListingId"] == listingId);
            Assert.IsTrue((string)getResult.Payload!["Title"] == title);
            Assert.IsTrue((bool)getResult.Payload!["IsPublished"] == false);
            Assert.IsTrue((int)getResult.Payload!["Rating"] == 0);
        }

        [TestMethod]
        public async Task GetCommentsSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialPassword = "12345678";

            string showcaseId = $"showcaseId{time.Millisecond}";
            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            string commentText = "Test Comment";

            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            var accountId = userIdResult.Payload;

            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult.IsSuccessful);

            var commentResult = await _projectShowcaseDataAccess.AddComment(showcaseId, accountId, commentText, time);
            Assert.IsTrue(commentResult.IsSuccessful);
            var getResult = await _projectShowcaseDataAccess.GetComments(showcaseId, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);
            var commentId = (int)getResult.Payload![0][$"{_showcaseCommentsTable}.Id"];

            Assert.IsTrue((int)getResult.Payload![0][$"{_showcaseCommentsTable}.Id"] == commentId);
            Assert.IsTrue((string)getResult.Payload![0]["Email"] == credentialEmail);
            Assert.IsTrue((string)getResult.Payload![0]["Text"] == commentText);
            Assert.IsTrue((int)getResult.Payload![0]["Rating"] == 0);
            Assert.IsTrue((DateTime)getResult.Payload![0]["Timestamp"]! == time);
            Assert.IsTrue((DateTime)getResult.Payload![0]["EditTimestamp"]! == time);
            Assert.IsTrue((string)getResult.Payload![0][$"{_showcasesTable}.Id"] == showcaseId);
        }

        [TestMethod]
        public async Task InsertGetUserCommentRatingSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialPassword = "12345678";

            string showcaseId = $"showcaseId{time.Millisecond}";
            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            string commentText = "Test Comment";

            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            var accountId = userIdResult.Payload;

            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult.IsSuccessful);

            var commentResult = await _projectShowcaseDataAccess.AddComment(showcaseId, accountId, commentText, time);
            Assert.IsTrue(commentResult.IsSuccessful);
            var getCommentResult = await _projectShowcaseDataAccess.GetComments(showcaseId, 10, 1).ConfigureAwait(false);
            var commentId = (int)getCommentResult.Payload![0][$"{_showcaseCommentsTable}.Id"];

            var insertUserCommentRatingResult = await _projectShowcaseDataAccess.InsertUserCommentRating(commentId, accountId, true);
            Assert.IsTrue(insertUserCommentRatingResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetCommentUserRating(commentId, accountId);

            Assert.IsTrue((bool)getResult.Payload! == true);
        }

        [TestMethod]
        public async Task GetDetailsSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialPassword = "12345678";

            string showcaseId = $"showcaseId{time.Millisecond}";
            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            bool isPublished = true;

            var regResult = await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            Assert.IsTrue(regResult.IsSuccessful, regResult.ErrorMessage);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            Assert.IsTrue(userIdResult.IsSuccessful);

            var accountId = userIdResult.Payload;

            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult.IsSuccessful, insertResult.ErrorMessage);

            var getResult = await _projectShowcaseDataAccess.GetDetails(showcaseId);
            Assert.IsTrue(getResult.IsSuccessful,getResult.ErrorMessage);

            Assert.IsTrue((string)getResult.Payload![$"{_showcasesTable}.Id"] == showcaseId);
            Assert.IsTrue((string)getResult.Payload!["Email"] == credentialEmail);
            // ShowcaseUserId and {_userAccountsTableName}.Id ???
            Assert.IsTrue((int)getResult.Payload![$"{_userAccountsTable}.Id"] == accountId);
            Assert.IsTrue((int)getResult.Payload!["ListingId"] == listingId);
            Assert.IsTrue((string)getResult.Payload!["Title"] == title);
            Assert.IsTrue((bool)getResult.Payload!["IsPublished"] == !isPublished);
            Assert.IsTrue((int)getResult.Payload!["Rating"] == 0);
        }

        [TestMethod]
        public async Task IncrementShowcaseLikesSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialPassword = "12345678";

            string showcaseId = $"showcaseId{time.Millisecond}";
            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            var accountId = userIdResult.Payload;

            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult.IsSuccessful);

            var incrementResult = await _projectShowcaseDataAccess.IncrementShowcaseLikes(showcaseId);
            Assert.IsTrue(incrementResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetDetails(showcaseId);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((int)getResult.Payload!["Rating"] == 1);
        }

        [TestMethod]
        public async Task RecordUserLikeSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialPassword = "12345678";

            string showcaseId = $"showcaseId{time.Millisecond}";
            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            var accountId = userIdResult.Payload;

            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult.IsSuccessful);

            var userLikeResult = await _projectShowcaseDataAccess.RecordUserLike(accountId, showcaseId).ConfigureAwait(false);
            Assert.IsTrue(userLikeResult.IsSuccessful);

            //potentially other get method needed?
        }

        [TestMethod]
        public async Task RemoveShowcaseListingSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialPassword = "12345678";

            string showcaseId = $"showcaseId{time.Millisecond}";
            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            var accountId = userIdResult.Payload;

            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult.IsSuccessful);

            var removeResult = await _projectShowcaseDataAccess.RemoveShowcaseListing(showcaseId);
            Assert.IsTrue(removeResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetShowcase(showcaseId);
            Assert.IsNull(getResult.Payload);
        }

        [TestMethod]
        public async Task UpdateCommentSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialPassword = "12345678";

            string showcaseId = $"showcaseId{time.Millisecond}";
            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            string commentText = "Test Comment";
            string editedText = "Edited Text";

            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            var accountId = userIdResult.Payload;

            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult.IsSuccessful);

            var commentResult = await _projectShowcaseDataAccess.AddComment(showcaseId, accountId, commentText, time);
            Assert.IsTrue(commentResult.IsSuccessful);
            var getCommentResult = await _projectShowcaseDataAccess.GetComments(showcaseId, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(getCommentResult.IsSuccessful);
            var commentId = (int)getCommentResult.Payload![0][$"{_showcaseCommentsTable}.Id"];

            var updateResult = await _projectShowcaseDataAccess.UpdateComment(commentId, editedText, time);
            Assert.IsTrue(updateResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetComments(showcaseId, 10, 1);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((string)getResult.Payload![0]["Text"] == editedText);
            Assert.IsTrue((DateTime)getResult.Payload[0]!["EditTimestamp"]! == time);
        }

        [TestMethod]
        public async Task UpdateCommentRatingSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialPassword = "12345678";

            string showcaseId = $"showcaseId{time.Millisecond}";
            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            string commentText = "Test Comment";
            int difference = 3;

            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            var accountId = userIdResult.Payload;

            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult.IsSuccessful);

            var commentResult = await _projectShowcaseDataAccess.AddComment(showcaseId, accountId, commentText, time);
            Assert.IsTrue(commentResult.IsSuccessful);
            var getCommentResult = await _projectShowcaseDataAccess.GetComments(showcaseId, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(getCommentResult.IsSuccessful);
            var commentId = (int)getCommentResult.Payload![0][$"{_showcaseCommentsTable}.Id"];

            var updateRatingResult = await _projectShowcaseDataAccess.UpdateCommentRating(commentId, difference);
            Assert.IsTrue(updateRatingResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetComments(showcaseId, 10, 1);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((int)getResult.Payload![0]["Rating"] == 0 + difference);
        }

        [TestMethod]
        public async Task UpdateUserCommentRatingSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialPassword = "12345678";

            string showcaseId = $"showcaseId{time.Millisecond}";
            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            string commentText = "Test Comment";
            int difference = 3;

            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            var accountId = userIdResult.Payload;

            var insertResult = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult.IsSuccessful);

            var commentResult = await _projectShowcaseDataAccess.AddComment(showcaseId, accountId, commentText, time);
            Assert.IsTrue(commentResult.IsSuccessful);
            var getCommentResult = await _projectShowcaseDataAccess.GetComments(showcaseId, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(getCommentResult.IsSuccessful);
            var commentId = (int)getCommentResult.Payload![0][$"{_showcaseCommentsTable}.Id"];

            var updateUserRatingResult = await _projectShowcaseDataAccess.UpdateUserCommentRating(commentId, accountId, true);
            Assert.IsTrue(updateUserRatingResult.IsSuccessful);

            //potentially other get method needed?
        }
    }
}
