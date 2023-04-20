using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
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

namespace DevelopmentHell.Hubba.ListingProfile.Test.Unit_Tests
{
    [TestClass]
    public class ListingRatingsDataAccessUnitTests
    {
        private readonly IListingsDataAccess _listingsDataAccess;
        private IListingHistoryDataAccess _listingHistoryDataAccess;
        private IRatingDataAccess _ratingDataAccess;
        private readonly ITestingService _testingService;
        private readonly IUserAccountDataAccess _userAccountDataAccess;


        private readonly string _userConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private readonly string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;

        private readonly string _listingProfileConnectionString = ConfigurationManager.AppSettings["ListingProfileConnectionString"]!;
        private readonly string _listingsTable = ConfigurationManager.AppSettings["ListingsTable"]!;
        private readonly string _listingHistoryTable = ConfigurationManager.AppSettings["ListingHistoryTable"]!;
        private readonly string _listingRatingsTable = ConfigurationManager.AppSettings["ListingRatingsTable"]!;
        private readonly string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;

        private readonly string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;


        public ListingRatingsDataAccessUnitTests()
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

            _ratingDataAccess = new RatingDataAccess(_listingProfileConnectionString, _listingRatingsTable);

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
        public async Task AddRatingNoComment()
        {
            // Arrange
            var ownerId = 1;
            var title = "Listing Test Title 1";
            var expected = true;

            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var userId = 2;
            await _listingHistoryDataAccess.AddUser(listingId, userId).ConfigureAwait(false);

            var rating = 4;

            // Actual
            var actual = await _ratingDataAccess.AddRating(Feature.Listing, listingId, userId, rating, null, false).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task AddRatingWithComment()
        {
            // Arrange
            var ownerId = 1;
            var title = "Listing Test Title 1";
            var expected = true;

            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var userId = 2;
            await _listingHistoryDataAccess.AddUser(listingId, userId).ConfigureAwait(false);

            var rating = 4;
            var comment = "Yo this is dope!";

            // Actual
            var actual = await _ratingDataAccess.AddRating(Feature.Listing, listingId, userId, rating, comment, false).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task AddRatingNoCommentNoAnonymous()
        {
            // Arrange
            var ownerId = 1;
            var title = "Listing Test Title 1";
            var expected = true;

            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var userId = 2;
            await _listingHistoryDataAccess.AddUser(listingId, userId).ConfigureAwait(false);

            var rating = 3;

            // Actual
            var actual = await _ratingDataAccess.AddRating(Feature.Listing, listingId, userId, rating, null, null).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task GetAverageRating()
        {
            // Arrange
            var ownerId = 1;
            var title = "Listing Test Title 1";
            var expected = true;
            var expectedRating = 3.6;

            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var userId1 = 2;
            var userId2 = 3;
            var userId3 = 4;

            await _listingHistoryDataAccess.AddUser(listingId, userId1).ConfigureAwait(false);
            await _listingHistoryDataAccess.AddUser(listingId, userId2).ConfigureAwait(false);
            await _listingHistoryDataAccess.AddUser(listingId, userId3).ConfigureAwait(false);

            var rating1 = 2;
            var rating2 = 4;
            var rating3 = 5;
            await _ratingDataAccess.AddRating(Feature.Listing, listingId, userId1, rating1, null, null).ConfigureAwait(false);
            await _ratingDataAccess.AddRating(Feature.Listing, listingId, userId2, rating2, null, null).ConfigureAwait(false);
            await _ratingDataAccess.AddRating(Feature.Listing, listingId, userId3, rating3, null, null).ConfigureAwait(false);


            // Actual
            var actual = await _ratingDataAccess.GetAverageRating(Feature.Listing, listingId).ConfigureAwait(false);
            Console.WriteLine(actual.ErrorMessage);
            

            //Assert
            
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload is not null);
            Assert.IsTrue(actual.Payload[listingId] == expectedRating);
        }
    }

}