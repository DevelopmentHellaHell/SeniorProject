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
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;

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
            var expectedValue = true;

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

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload is not null);
            Assert.IsTrue(actual.Payload.ContainsKey(listingId) == expectedValue);
            Assert.IsTrue(actual.Payload[listingId] == expectedRating);
        }

        [TestMethod]
        public async Task GetAverageRatingWhenNull()
        {
            // Arrange
            var ownerId = 1;
            var title = "Listing Test Title 1";
            var expected = true;
            var expectedValue = false;

            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;


            // Actual
            var actual = await _ratingDataAccess.GetAverageRating(Feature.Listing, listingId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload is not null);
            Assert.IsTrue(actual.Payload.ContainsKey(listingId) == expectedValue);
        }


        [TestMethod]
        public async Task GetListingRatings()
        {
            //Arrange
            var ownerId = 1;
            var title = "Listing Test Title 1";
            var expected = true;


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

            var comment2 = "Nice!";
            var comment3 = "Really cool!";
            await _ratingDataAccess.AddRating(Feature.Listing, listingId, userId1, rating1, null, true).ConfigureAwait(false);
            await _ratingDataAccess.AddRating(Feature.Listing, listingId, userId2, rating2, comment2, null).ConfigureAwait(false);
            await _ratingDataAccess.AddRating(Feature.Listing, listingId, userId3, rating3, comment3, false).ConfigureAwait(false);

            var expectedRatings = new List<int>() { rating1, rating2, rating3 };
            var expectedComments = new List<string?>() { null, comment2, comment3 };
            var expectedUserIds = new List<int>() { userId1, userId2, userId3 };

            bool errorFound = false;

            //Act
            var actual = await _ratingDataAccess.GetListingRatings(listingId).ConfigureAwait(false);


            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload is not null);
            foreach (ListingRating rating in actual.Payload)
            {

                // Check rating value
                if (!expectedRatings.Any(expectedRating => expectedRating == rating.Rating))
                {
                    errorFound = true;
                    break;
                }

                // Check comment value
                if (!expectedComments.Any(expectedComment => expectedComment == rating.Comment))
                {
                    errorFound = true;
                    break;
                }

                // Check userId value
                if (!expectedUserIds.Any(expectedUserId => expectedUserId == rating.UserId))
                {
                    errorFound = true;
                    break;
                }
            }
            Assert.IsTrue(errorFound == false);
        }

        [TestMethod]
        public async Task GetListingRatingsEmpty()
        {
            //Arrange
            var ownerId = 1;
            var title = "Listing Test Title 1";
            var expected = true;


            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            //Act
            var actual = await _ratingDataAccess.GetListingRatings(listingId).ConfigureAwait(false);


            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload is not null);
        }

        [TestMethod]
        public async Task CountRatingWith1()
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

            var rating = 4;
            var comment = "Yo this is dope!";

            await _ratingDataAccess.AddRating(Feature.Listing, listingId, userId, rating, comment, false).ConfigureAwait(false);

            // Actual
            var actual = await _ratingDataAccess.CountRating(Feature.Listing, listingId, userId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload == expectedCount);
        }

        [TestMethod]
        public async Task CountRatingWith0()
        {
            // Arrange
            var ownerId = 1;
            var title = "Listing Test Title 1";
            var expected = true;
            var expectedCount = 0;

            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var userId = 2;

            // Actual
            var actual = await _ratingDataAccess.CountRating(Feature.Listing, listingId, userId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload == expectedCount);
        }

        [TestMethod]
        public async Task DeleteRating()
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

            var rating = 4;
            var comment = "Yo this is dope!";

            await _ratingDataAccess.AddRating(Feature.Listing, listingId, userId, rating, comment, false).ConfigureAwait(false);

            // Actual
            var actual = await _ratingDataAccess.DeleteRating(Feature.Listing, listingId, userId);
            var checkCount = await _ratingDataAccess.CountRating(Feature.Listing, listingId, userId);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(checkCount.Payload == 0);
        }

        [TestMethod]
        public async Task GetRating()
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

            await _ratingDataAccess.AddRating(Feature.Listing, listingId, userId, rating, comment, false).ConfigureAwait(false);

            //Act
            var actual = await _ratingDataAccess.GetRating(Feature.Listing, listingId, userId).ConfigureAwait(false);


            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload is not null);
            Assert.IsTrue(actual.Payload[0]["ListingId"].Equals(listingId));
            Assert.IsTrue(actual.Payload[0]["UserId"].Equals(userId));
            Assert.IsTrue(actual.Payload[0]["Rating"].Equals(rating));
            Assert.IsTrue(actual.Payload[0]["Comment"].Equals(comment));

        }


        [TestMethod]
        public async Task UpdateRatingNoComment()
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
            var anonymous = false;

            await _ratingDataAccess.AddRating(Feature.Listing, listingId, userId, rating, comment, anonymous).ConfigureAwait(false);

            var newRating = 2;
            var newComment = DBNull.Value;

            ListingRatingEditorDTO ratingEdit = new ListingRatingEditorDTO()
            {

                ListingId= listingId,
                UserId= userId,
                Rating = newRating,
            };

            // Actual
            var actual = await _ratingDataAccess.UpdateListingRating(ratingEdit).ConfigureAwait(false);
            var getRating = await _ratingDataAccess.GetRating(Feature.Listing, listingId, userId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(getRating.Payload is not null);
            Assert.IsTrue(getRating.Payload[0]["ListingId"].Equals(listingId));
            Assert.IsTrue(getRating.Payload[0]["UserId"].Equals(userId));
            Assert.IsTrue(getRating.Payload[0]["Rating"].Equals(newRating));
            Assert.IsTrue(getRating.Payload[0]["Comment"].Equals(newComment));
            Assert.IsTrue(getRating.Payload[0]["Anonymous"].Equals(anonymous));
        }

        [TestMethod]
        public async Task UpdateRatingWithComment()
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
            var anonymous = false;

            await _ratingDataAccess.AddRating(Feature.Listing, listingId, userId, rating, comment, anonymous).ConfigureAwait(false);

            var newRating = 2;
            var newComment = "Overrated...";
            var newAnonymous = true;

            ListingRatingEditorDTO ratingEdit = new ListingRatingEditorDTO()
            {

                ListingId = listingId,
                UserId = userId,
                Rating = newRating,
                Comment = newComment,
                Anonymous = newAnonymous,
            };

            // Actual
            var actual = await _ratingDataAccess.UpdateListingRating(ratingEdit).ConfigureAwait(false);
            var getRating = await _ratingDataAccess.GetRating(Feature.Listing, listingId, userId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(getRating.Payload is not null);
            Assert.IsTrue(getRating.Payload[0]["ListingId"].Equals(listingId));
            Assert.IsTrue(getRating.Payload[0]["UserId"].Equals(userId));
            Assert.IsTrue(getRating.Payload[0]["Rating"].Equals(newRating));
            Assert.IsTrue(getRating.Payload[0]["Comment"].Equals(newComment));
            Assert.IsTrue(getRating.Payload[0]["Anonymous"].Equals(newAnonymous));
        }
    }

}