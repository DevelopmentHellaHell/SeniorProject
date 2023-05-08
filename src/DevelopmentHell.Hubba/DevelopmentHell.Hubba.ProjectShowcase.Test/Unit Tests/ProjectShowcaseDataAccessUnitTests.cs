using System.Configuration;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;

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


            var commentResult = await _projectShowcaseDataAccess.AddComment(showcaseId, accountId, commentText, time);

            Assert.IsTrue(commentResult.IsSuccessful);
            var getResult = await _projectShowcaseDataAccess.GetComments(showcaseId, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((string)getResult.Payload![0]["ShowcaseId"] == showcaseId);
            Assert.IsTrue((int)getResult.Payload![0]["CommenterId"] == accountId);
            Assert.IsTrue((string)getResult.Payload![0]["Text"] == commentText);
            Assert.IsTrue(((DateTime)getResult.Payload![0]["Timestamp"]).Second == time.Second);
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
            Assert.IsTrue(insertResult.IsSuccessful, insertResult.ErrorMessage);

            var commentResult = await _projectShowcaseDataAccess.AddComment(showcaseId, accountId, commentText, time);
            Assert.IsTrue(commentResult.IsSuccessful, commentResult.ErrorMessage);
            var getCommentResult = await _projectShowcaseDataAccess.GetComments(showcaseId, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(getCommentResult.IsSuccessful);
            var commentId = Convert.ToInt32(getCommentResult.Payload![0]["Id"]);

            var commentReportResult = await _projectShowcaseDataAccess.AddCommentReport(commentId, accountId, reason, time);
            Assert.IsTrue(commentReportResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetCommentReports(commentId);
            Assert.IsTrue(getResult.IsSuccessful, getResult.ErrorMessage);

            Assert.IsTrue(Convert.ToInt32(getResult.Payload![0]["CommentId"]) == commentId);
            Assert.IsTrue((int)getResult.Payload![0]["ReporterId"] == accountId);
            Assert.IsTrue((string)getResult.Payload![0]["Reason"] == reason);
            Assert.IsTrue(((DateTime)getResult.Payload![0]["Timestamp"]).Second == time.Second, getResult.ErrorMessage);
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

            var getResult = await _projectShowcaseDataAccess.GetShowcase(showcaseId).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful, getResult.ErrorMessage);
            Assert.IsTrue((string)getResult.Payload!["Id"] == showcaseId);
            Assert.IsTrue((int)getResult.Payload!["ShowcaseUserId"] == accountId);
            Assert.IsTrue(((getResult.Payload!["ListingId"].GetType() == typeof(DBNull)) ? null : (int)getResult.Payload!["ListingId"]) == listingId);
            Assert.IsTrue((string)getResult.Payload!["Title"] == title);
            Assert.IsTrue((string)getResult.Payload!["Description"] == description);
            Assert.IsTrue((bool)getResult.Payload!["IsPublished"] == false);
            Assert.IsTrue((double)getResult.Payload!["Rating"] == 0);
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
            Assert.IsTrue(showcaseReportResult.IsSuccessful, showcaseReportResult.ErrorMessage);

            var getResult = await _projectShowcaseDataAccess.GetShowcaseReports(showcaseId);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((string)getResult.Payload![0]["ShowcaseId"] == showcaseId);
            Assert.IsTrue((int)getResult.Payload![0]["ReporterId"] == accountId);
            Assert.IsTrue((string)getResult.Payload![0]["Reason"] == reason);
            Assert.IsTrue(((DateTime)getResult.Payload![0]["Timestamp"]).Second == time.Second);
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

            Assert.IsTrue((bool)getResult.Payload!["IsPublished"] == isPublished, getResult.ErrorMessage);
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
            var commentId = Convert.ToInt32(getCommentResult.Payload![0]["Id"]);

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
            Assert.IsTrue(!getResult.IsSuccessful, getResult.ErrorMessage);
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

            var getResult = await _projectShowcaseDataAccess.GetShowcase(showcaseId);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((string)getResult.Payload!["Title"] == editedTitle);
            Assert.IsTrue((string)getResult.Payload!["Description"] == editedDescription);
            Assert.IsTrue(((DateTime)getResult.Payload!["EditTimestamp"]).Second == time.Second);
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
            Assert.IsTrue(getCommentResult.IsSuccessful, getCommentResult.ErrorMessage);
            var commentId = Convert.ToInt32(getCommentResult.Payload![0]["Id"]);

            var getResult = await _projectShowcaseDataAccess.GetCommentDetails(commentId);
            Assert.IsTrue(getResult.IsSuccessful, getResult.ErrorMessage);

            Assert.IsTrue(Convert.ToInt32(getResult.Payload!["Id"]) == commentId);
            Assert.IsTrue((string)getResult.Payload!["ShowcaseId"] == showcaseId);
            Assert.IsTrue((int)getResult.Payload!["CommenterId"] == accountId);
            Assert.IsTrue(((DateTime)getResult.Payload!["Timestamp"]).Second == time.Second);
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
            Assert.IsTrue(insertResult.IsSuccessful, insertResult.ErrorMessage);

            var commentResult = await _projectShowcaseDataAccess.AddComment(showcaseId, accountId, commentText, time);
            Assert.IsTrue(commentResult.IsSuccessful);
            var getResult = await _projectShowcaseDataAccess.GetComments(showcaseId, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);
            var commentId = Convert.ToInt32(getResult.Payload![0]["Id"]);

            Assert.IsTrue(Convert.ToInt32(getResult.Payload![0]["Id"]) == commentId);
            Assert.IsTrue((string)getResult.Payload![0]["Text"] == commentText);
            Assert.IsTrue((int)getResult.Payload![0]["Rating"] == 0);
            Assert.IsTrue(((DateTime)getResult.Payload![0]["Timestamp"]).Second == time.Second);
            Assert.IsTrue((string)getResult.Payload![0]["ShowcaseId"] == showcaseId);
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
            var commentId = Convert.ToInt32(getCommentResult.Payload![0]["Id"]);

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
            Assert.IsTrue(getResult.IsSuccessful, getResult.ErrorMessage);
            Assert.IsTrue((string)getResult.Payload!["Id"] == showcaseId);
            Assert.IsTrue((int)getResult.Payload![$"ShowcaseUserId"] == accountId);
            Assert.IsTrue(((getResult.Payload!["ListingId"].GetType() == typeof(DBNull)) ? null : (int)getResult.Payload!["ListingId"]) == listingId);
            Assert.IsTrue((string)getResult.Payload!["Title"] == title);
            Assert.IsTrue((bool)getResult.Payload!["IsPublished"] == !isPublished);
            Assert.IsTrue((double)getResult.Payload!["Rating"] == 0);
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
            Assert.IsTrue(incrementResult.IsSuccessful, incrementResult.ErrorMessage);

            var getResult = await _projectShowcaseDataAccess.GetDetails(showcaseId);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((double)getResult.Payload!["Rating"] == 1);
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

            var getResult = await _projectShowcaseDataAccess.GetDetails(showcaseId).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);
        }

        [TestMethod]
        public async Task RemoveShowcaseListingSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialPassword = "12345678";

            string showcaseId = $"showcaseId{time.Millisecond}";
            int? listingId = 3;
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
            Assert.IsTrue(removeResult.IsSuccessful, removeResult.ErrorMessage);

            var getResult = await _projectShowcaseDataAccess.GetShowcase(showcaseId);
            Assert.IsTrue(getResult.Payload!["ListingId"] == DBNull.Value);
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
            var commentId = Convert.ToInt32(getCommentResult.Payload![0]["Id"]);

            var updateResult = await _projectShowcaseDataAccess.UpdateComment(commentId, editedText, time);
            Assert.IsTrue(updateResult.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetComments(showcaseId, 10, 1);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((string)getResult.Payload![0]["Text"] == editedText);
            Assert.IsTrue(((DateTime)getResult.Payload[0]!["EditTimestamp"]).Second == time.Second);
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
            var commentId = Convert.ToInt32(getCommentResult.Payload![0]["Id"]);

            var updateRatingResult = await _projectShowcaseDataAccess.UpdateCommentRating(commentId, difference);
            Assert.IsTrue(updateRatingResult.IsSuccessful, updateRatingResult.ErrorMessage);

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
            Assert.IsTrue(insertResult.IsSuccessful, insertResult.ErrorMessage);

            var commentResult = await _projectShowcaseDataAccess.AddComment(showcaseId, accountId, commentText, time);
            Assert.IsTrue(commentResult.IsSuccessful);
            var getCommentResult = await _projectShowcaseDataAccess.GetComments(showcaseId, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(getCommentResult.IsSuccessful);
            var commentId = Convert.ToInt32(getCommentResult.Payload![0]["Id"]);

            var insertUserRatingResult = await _projectShowcaseDataAccess.InsertUserCommentRating(commentId, accountId, true);
            Assert.IsTrue(insertUserRatingResult.IsSuccessful);

            var updateUserRatingResult = await _projectShowcaseDataAccess.UpdateUserCommentRating(commentId, accountId, true);
            Assert.IsTrue(updateUserRatingResult.IsSuccessful, updateUserRatingResult.ErrorMessage);

            var getResult = await _projectShowcaseDataAccess.GetCommentUserRating(commentId, accountId).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue(getResult.Payload != null);
            Assert.IsTrue((bool)getResult.Payload! == true);
        }

        [TestMethod]
        public async Task GetCommentShowcaseSuccess()
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
            Assert.IsTrue(insertResult.IsSuccessful, insertResult.ErrorMessage);

            var commentResult = await _projectShowcaseDataAccess.AddComment(showcaseId, accountId, commentText, time);
            Assert.IsTrue(commentResult.IsSuccessful);
            var getCommentResult = await _projectShowcaseDataAccess.GetComments(showcaseId, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(getCommentResult.IsSuccessful);
            var commentId = Convert.ToInt32(getCommentResult.Payload![0]["Id"]);

            var getResult = await _projectShowcaseDataAccess.GetCommentShowcase(commentId).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful, getResult.ErrorMessage);
        }

        [TestMethod]
        public async Task GetUserShowcasesSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail1 = $"test{time.Millisecond}@gmail.com";
            string credentialPassword1 = "12345678";

            string credentialEmail2 = $"test2{time.Millisecond}@gmail.com";
            string credentialPassword2 = "123456789";

            string showcaseId1 = $"showcaseId1{time.Millisecond}";
            string showcaseId2 = $"showcaseId2{time.Millisecond}";
            string showcaseId3 = $"showcaseId3{time.Millisecond}";


            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            await _registrationService.RegisterAccount(credentialEmail1, credentialPassword1).ConfigureAwait(false);
            var userIdResult1 = await _userAccountDataAccess.GetId(credentialEmail1).ConfigureAwait(false);

            var accountId1 = userIdResult1.Payload;

            await _registrationService.RegisterAccount(credentialEmail2, credentialPassword2).ConfigureAwait(false);
            var userIdResult2 = await _userAccountDataAccess.GetId(credentialEmail2).ConfigureAwait(false);

            var accountId2 = userIdResult2.Payload;

            var insertResult1 = await _projectShowcaseDataAccess.InsertShowcase(
                accountId1,
                showcaseId1,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult1.IsSuccessful);

            var insertResult2 = await _projectShowcaseDataAccess.InsertShowcase(
                accountId1,
                showcaseId2,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult2.IsSuccessful);

            var insertResult3 = await _projectShowcaseDataAccess.InsertShowcase(
                accountId2,
                showcaseId3,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult3.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetUserShowcases(accountId1);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((string)getResult.Payload![0]["Id"] == showcaseId1);
            Assert.IsTrue((string)getResult.Payload![1]["Id"] == showcaseId2);
        }

        [TestMethod]
        public async Task GetAllShowcaseReportsSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail1 = $"test{time.Millisecond}@gmail.com";
            string credentialPassword1 = "12345678";

            string credentialEmail2 = $"test2{time.Millisecond}@gmail.com";
            string credentialPassword2 = "123456789";

            string showcaseId1 = $"showcaseId1{time.Millisecond}";
            string showcaseId2 = $"showcaseId2{time.Millisecond}";
            string showcaseId3 = $"showcaseId3{time.Millisecond}";


            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            string reason = "Test Reason";
            string reason2 = "Reason 2";

            await _registrationService.RegisterAccount(credentialEmail1, credentialPassword1).ConfigureAwait(false);
            var userIdResult1 = await _userAccountDataAccess.GetId(credentialEmail1).ConfigureAwait(false);

            var accountId1 = userIdResult1.Payload;

            await _registrationService.RegisterAccount(credentialEmail2, credentialPassword2).ConfigureAwait(false);
            var userIdResult2 = await _userAccountDataAccess.GetId(credentialEmail2).ConfigureAwait(false);

            var accountId2 = userIdResult2.Payload;

            var insertResult1 = await _projectShowcaseDataAccess.InsertShowcase(
                accountId1,
                showcaseId1,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult1.IsSuccessful);

            var insertResult2 = await _projectShowcaseDataAccess.InsertShowcase(
                accountId1,
                showcaseId2,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult2.IsSuccessful);

            var insertResult3 = await _projectShowcaseDataAccess.InsertShowcase(
                accountId2,
                showcaseId3,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult3.IsSuccessful);

            var showcaseReportResult1 = await _projectShowcaseDataAccess.AddShowcaseReport(showcaseId1, accountId2, reason, time);
            Assert.IsTrue(showcaseReportResult1.IsSuccessful);

            var showcaseReportResult2 = await _projectShowcaseDataAccess.AddShowcaseReport(showcaseId2, accountId1, reason2, time);
            Assert.IsTrue(showcaseReportResult2.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetAllShowcaseReports();
            Assert.IsTrue(getResult.IsSuccessful);

            bool var1 = false;
            bool var2 = false;

            for (int i = 0; i < getResult.Payload!.Count; i++)
            {
                var1 = var1 || ((string)getResult.Payload![i]["ShowcaseId"] == showcaseId1);
                var2 = var2 || ((string)getResult.Payload![i]["ShowcaseId"] == showcaseId2);

                if (var1 && var2)
                {
                    Assert.IsTrue(var1 && var2);

                }
            }
        }

        [TestMethod]
        public async Task GetShowcaseReportsSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialEmail2 = $"test2{time.Millisecond}@gmail.com";

            string credentialPassword = "12345678";
            string showcaseId = $"showcaseId{time.Millisecond}";
            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            string reason = "Test Reason";
            string reason2 = "Reason 2";

            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            var accountId = userIdResult.Payload;

            await _registrationService.RegisterAccount(credentialEmail2, credentialPassword).ConfigureAwait(false);
            var userIdResult2 = await _userAccountDataAccess.GetId(credentialEmail2).ConfigureAwait(false);

            var accountId2 = userIdResult2.Payload;

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
            Assert.IsTrue(showcaseReportResult.IsSuccessful, showcaseReportResult.ErrorMessage);

            var showcaseReportResult2 = await _projectShowcaseDataAccess.AddShowcaseReport(showcaseId, accountId2, reason2, time);
            Assert.IsTrue(showcaseReportResult2.IsSuccessful, showcaseReportResult2.ErrorMessage);

            var getResult = await _projectShowcaseDataAccess.GetShowcaseReports(showcaseId);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue((string)getResult.Payload![0][$"ShowcaseId"] == showcaseId);
            Assert.IsTrue((string)getResult.Payload![0]["Reason"] == reason);
            Assert.IsTrue((string)getResult.Payload![1][$"ShowcaseId"] == showcaseId);
            Assert.IsTrue((string)getResult.Payload![1]["Reason"] == reason2);
        }

        [TestMethod]
        public async Task GetAllCommentReportsSuccess()
        {
            DateTime time = DateTime.UtcNow;
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialPassword = "12345678";
            string showcaseId1 = $"showcaseId1{time.Millisecond}";
            string showcaseId2 = $"showcaseId2{time.Millisecond}";
            DateTime time2 = DateTime.UtcNow.AddSeconds(3);


            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            string reason = "Test Reason";

            string commentText = "Text";
            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            var accountId = userIdResult.Payload;

            var insertResult1 = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId1,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult1.IsSuccessful, insertResult1.ErrorMessage);

            var insertResult2 = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId2,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult2.IsSuccessful, insertResult2.ErrorMessage);

            var commentResult1 = await _projectShowcaseDataAccess.AddComment(showcaseId1, accountId, commentText, time);
            Assert.IsTrue(commentResult1.IsSuccessful, commentResult1.ErrorMessage);
            var commentResult2 = await _projectShowcaseDataAccess.AddComment(showcaseId2, accountId, commentText, time2);
            Assert.IsTrue(commentResult2.IsSuccessful, commentResult2.ErrorMessage);

            var getCommentResult1 = await _projectShowcaseDataAccess.GetComments(showcaseId1, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(getCommentResult1.IsSuccessful);
            var commentId1 = Convert.ToInt32(getCommentResult1.Payload![0]["Id"]);
            var getCommentResult2 = await _projectShowcaseDataAccess.GetComments(showcaseId2, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(getCommentResult2.IsSuccessful);
            var commentId2 = Convert.ToInt32(getCommentResult2.Payload![0]["Id"]);

            var commentReportResult1 = await _projectShowcaseDataAccess.AddCommentReport(commentId1, accountId, reason, time);
            Assert.IsTrue(commentReportResult1.IsSuccessful);
            var commentReportResult2 = await _projectShowcaseDataAccess.AddCommentReport(commentId2, accountId, reason, time);
            Assert.IsTrue(commentReportResult2.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetAllCommentReports().ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful, getResult.ErrorMessage);

            bool var1 = false;
            bool var2 = false;

            for (int i = 0; i < getResult.Payload!.Count; i++)
            {
                var1 = var1 || (Convert.ToInt32(getResult.Payload![i]["CommentId"]) == commentId1);
                var2 = var2 || (Convert.ToInt32(getResult.Payload![i]["CommentId"]) == commentId2);

                if (var1 && var2)
                {
                    Assert.IsTrue(var1 && var2);

                }
            }
        }

        [TestMethod]
        public async Task GetCommentReportsSuccess()
        {
            DateTime time = DateTime.UtcNow;
            DateTime time2 = DateTime.UtcNow.AddSeconds(3);
            string credentialEmail = $"test{time.Millisecond}@gmail.com";
            string credentialPassword = "12345678";
            string showcaseId1 = $"showcaseId1{time.Millisecond}";
            string showcaseId2 = $"showcaseId2{time.Millisecond}";

            int? listingId = null;
            string title = "Test Title";
            string description = "Test Description";

            string reason = "Test Reason";

            string commentText = "Text";
            await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);

            var accountId = userIdResult.Payload;

            var insertResult1 = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId1,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult1.IsSuccessful, insertResult1.ErrorMessage);

            var insertResult2 = await _projectShowcaseDataAccess.InsertShowcase(
                accountId,
                showcaseId2,
                listingId,
                title,
                description,
                time
            ).ConfigureAwait(false);
            Assert.IsTrue(insertResult2.IsSuccessful, insertResult2.ErrorMessage);

            var commentResult1 = await _projectShowcaseDataAccess.AddComment(showcaseId1, accountId, commentText, time);
            Assert.IsTrue(commentResult1.IsSuccessful, commentResult1.ErrorMessage);
            var commentResult2 = await _projectShowcaseDataAccess.AddComment(showcaseId2, accountId, commentText, time2);
            Assert.IsTrue(commentResult2.IsSuccessful, commentResult2.ErrorMessage);

            var getCommentResult1 = await _projectShowcaseDataAccess.GetComments(showcaseId1, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(getCommentResult1.IsSuccessful);
            var commentId1 = Convert.ToInt32(getCommentResult1.Payload![0]["Id"]);
            var getCommentResult2 = await _projectShowcaseDataAccess.GetComments(showcaseId2, 10, 1).ConfigureAwait(false);
            Assert.IsTrue(getCommentResult2.IsSuccessful);
            var commentId2 = Convert.ToInt32(getCommentResult2.Payload![0]["Id"]);

            var commentReportResult1 = await _projectShowcaseDataAccess.AddCommentReport(commentId1, accountId, reason, time);
            Assert.IsTrue(commentReportResult1.IsSuccessful);
            var commentReportResult2 = await _projectShowcaseDataAccess.AddCommentReport(commentId2, accountId, reason, time);
            Assert.IsTrue(commentReportResult2.IsSuccessful);

            var getResult = await _projectShowcaseDataAccess.GetCommentReports(commentId1).ConfigureAwait(false);
            Assert.IsTrue(getResult.IsSuccessful);

            Assert.IsTrue(((DateTime)getResult.Payload![0]["Timestamp"]).Second == time.Second);
            Assert.IsTrue((int)getResult.Payload![0]["ReporterId"] == accountId);
            Assert.IsTrue((bool)getResult.Payload![0]["IsResolved"] == false);
            Assert.IsTrue((string)getResult.Payload![0]["Reason"] == reason);
        }
    }
}