using System.Configuration;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;

namespace DevelopmentHell.Hubba.ListingProfile.Test.Unit_Tests
{
    [TestClass]
    public class ListingHistoryDataAccessUnitTests
    {
        private readonly IListingsDataAccess _listingsDataAccess;
        private IListingHistoryDataAccess _listingHistoryDataAccess;
        private readonly ITestingService _testingService;
        private readonly IUserAccountDataAccess _userAccountDataAccess;


        private readonly string _userConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private readonly string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;

        private readonly string _listingProfileConnectionString = ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!;
        private readonly string _listingsTable = ConfigurationManager.AppSettings["ListingsTable"]!;
        private readonly string _listingHistoryTable = ConfigurationManager.AppSettings["ListingHistoryTable"]!;
        private readonly string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;

        private readonly string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;


        public ListingHistoryDataAccessUnitTests()
        {
            LoggerService loggerService = new LoggerService(
                new LoggerDataAccess(
                    _logsConnectionString,
                    _logsTable
                )
            );
            ICryptographyService cryptographyService = new CryptographyService(
                ConfigurationManager.AppSettings["CryptographyKey"]!
            );
            IValidationService validationService = new ValidationService();

            _listingsDataAccess = new ListingsDataAccess(_listingProfileConnectionString, _listingsTable);

            _listingHistoryDataAccess = new ListingHistoryDataAccess(_listingProfileConnectionString, _listingHistoryTable);

            _testingService = new TestingService(_jwtKey, new TestsDataAccess());

            _userAccountDataAccess = new UserAccountDataAccess(
                _userConnectionString,
                _userAccountsTable
            );
        }

        [TestInitialize]
        public async Task Setup()
        {
            await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.LISTING_PROFILES).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task AddHistory()
        {
            // Arrange
            var ownerId = 1;
            var title = "Listing Test Title 1";
            var expected = true;
            var expectedCount = 1;

            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var userId = 2;

            // Actual
            var actual = await _listingHistoryDataAccess.AddUser(listingId, userId).ConfigureAwait(false);
            var getCount = await _listingHistoryDataAccess.CountListingHistory(listingId, userId).ConfigureAwait(false);


            // Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(getCount.Payload == expectedCount);
        }

        [TestMethod]
        public async Task AddHistoryFailure()
        {
            // Arrange
            var listingId = 1;
            var userId = 2;
            var expected = false;
            var expectedErrorMessage = "Listing doesn't exist.";
            var expectedCount = 0;

            // Actual
            var actual = await _listingHistoryDataAccess.AddUser(listingId, userId).ConfigureAwait(false);
            var getCount = await _listingHistoryDataAccess.CountListingHistory(listingId, userId).ConfigureAwait(false);


            // Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
            Assert.IsTrue(getCount.Payload == expectedCount);
        }

        [TestMethod]
        public async Task CountUserHistory()
        {
            // Arrange
            var ownerId = 1;
            var title = "Listing Test Title 1";
            var expected = true;
            var expectedCount = 1;

            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var userId = 2;
            await _listingHistoryDataAccess.AddUser(listingId, userId).ConfigureAwait(false);

            // Actual
            var actual = await _listingHistoryDataAccess.CountListingHistory(listingId, userId).ConfigureAwait(false);


            // Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload == expectedCount);
        }

        [TestMethod]
        public async Task DeleteUser()
        {
            // Arrange
            var ownerId = 1;
            var title = "Listing Test Title 1";
            var expected = true;
            var expectedCountBeforeInsert = 0;
            var expectedCountAfterInsert = 1;
            var expectedCountAfterDelete = 0;

            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var userId = 2;


            // Actual
            var getCountBeforeInsert = await _listingHistoryDataAccess.CountListingHistory(listingId, userId).ConfigureAwait(false);
            await _listingHistoryDataAccess.AddUser(listingId, userId).ConfigureAwait(false);
            var getCountAfterInsert = await _listingHistoryDataAccess.CountListingHistory(listingId, userId).ConfigureAwait(false);
            var actual = await _listingHistoryDataAccess.DeleteUser(listingId, userId).ConfigureAwait(false);
            var getCountAfterDelete = await _listingHistoryDataAccess.CountListingHistory(listingId, userId).ConfigureAwait(false);




            // Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(getCountBeforeInsert.Payload == expectedCountBeforeInsert);
            Assert.IsTrue(getCountAfterInsert.Payload == expectedCountAfterInsert);
            Assert.IsTrue(getCountAfterDelete.Payload == expectedCountAfterDelete);
        }
    }
}
