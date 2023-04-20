using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using System;
using System.Configuration;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Security.Policy;

namespace DevelopmentHell.Hubba.ListingProfile.Test.Unit_Tests
{
    [TestClass]
    public class ListingDataAccessUnitTests
    {
        private readonly IListingsDataAccess _listingsDataAccess;
        private readonly ITestingService _testingService;
        private readonly IUserAccountDataAccess _userAccountDataAccess;


        private readonly string _userConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private readonly string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;

        private readonly string _listingProfileConnectionString = ConfigurationManager.AppSettings["ListingProfileConnectionString"]!;
        private readonly string _listingsTable = ConfigurationManager.AppSettings["ListingsTable"]!;
        private readonly string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;

        private readonly string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;


        public ListingDataAccessUnitTests()
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
        public async Task CreateListing()
        {
            // Arrange
            var ownerId = 1;
            var title = "Listing Test Title 1";
            var expected = true;

            // Actual
            var actual = await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task CreateListingUKFailure()
        {
            // Arrange
            var ownerId = 1;
            var title = "Listing Test Title 1";
            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var expected = false;
            var expectedErrorMessage = "Cannot create multiple listings with the same title.";

            // Actual
            var actual = await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }


        [TestMethod]
        public async Task CreateListingFailure()
        {
            // Arrange
            var ownerId = 1;
            var expected = false;
            var expectedErrorMessage = "Unable to create listing.";

            // Actual
            var actual = await _listingsDataAccess.CreateListing(ownerId, null).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }



        [TestMethod]
        public async Task GetListing()
        {
            // Arrange
            var ownerId = 1;
            var title = "Listing Test Title 2";
            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;
            var expected = true;
            var expectedType = typeof(Listing);

            // Actual
            var actual = await _listingsDataAccess.GetListing(listingId);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsNotNull(actual.Payload);
            Assert.IsTrue(actual.Payload.GetType() == expectedType);
            Assert.IsTrue(actual.Payload.Title.Equals(title));
            Assert.IsTrue(actual.Payload.OwnerId.Equals(ownerId));
        }

        [TestMethod]
        public async Task GetListingFailure()
        {
            // Arrange
            var expected = false;
            var expectedErrorMessage = "Cannot retrieve specified listing.";

            // Actual
            var actual = await _listingsDataAccess.GetListing(1);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);

        }

        [TestMethod]
        public async Task EditListing()
        {
            // Arrange
            var ownerId = 1;
            var title = "Listing Test Title 3";
            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var description = "New description";

            ListingEditorDTO editListing = new()
            {
                ListingId = listingId,
                OwnerId = ownerId,
                Title = title,
                Description = description
            };
            var expected = true;

            // Actual
            var actual = await _listingsDataAccess.UpdateListing(editListing).ConfigureAwait(false);
            var listingResult = await _listingsDataAccess.GetListing(listingId).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsNotNull(listingResult.Payload!.Description);
        }

        [TestMethod]
        public async Task EditListingFailure()
        {
            // Arrange
            var ownerId = 1;
            var title = "Listing Test Title 3";

            var description = "New description";

            ListingEditorDTO editListing = new()
            {
                ListingId = 1,
                OwnerId = ownerId,
                Title = title,
                Description = description
            };

            var expected = false;

            // Actual
            var actual = await _listingsDataAccess.UpdateListing(editListing).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
        }


        [TestMethod]
        public async Task GetUserListings()
        {
            //Arrange
            var ownerId = 1;
            var title1 = "Listing Test Title 1";
            var title2 = "Listing Test Title 2";
            var title3 = "Listing Test Title 3";

            await _listingsDataAccess.CreateListing(ownerId, title1).ConfigureAwait(false);
            await _listingsDataAccess.CreateListing(ownerId, title2).ConfigureAwait(false);
            await _listingsDataAccess.CreateListing(ownerId, title3).ConfigureAwait(false);

            var expected = true;
            var expectedType = typeof(List<Listing>);

            //Act
            var actual = await _listingsDataAccess.GetUserListings(ownerId).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful = expected);
            Assert.IsTrue(actual.Payload is not null);
            Assert.IsTrue(actual.Payload.GetType() == expectedType);
            Assert.IsTrue(actual.Payload.Count == 3);
        }

        [TestMethod]
        public async Task DeleteListing()
        {
            // Arrange
            var ownerId = 1;
            var title = "Listing Test Title 1";

            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var expected = true;


            // Actual
            var actual = await _listingsDataAccess.DeleteListing(listingId).ConfigureAwait(false);
            var listingResult = await _listingsDataAccess.GetListing(listingId).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(listingResult.Payload is null);
        }

        [TestMethod]
        public async Task PublishListing()
        {
            //Arrange
            var ownerId = 1;
            var title = "Listing Test Title 1";

            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var expected = true;

            //Act
            var actual = await _listingsDataAccess.PublishListing(listingId).ConfigureAwait(false);
            var listingResult = await _listingsDataAccess.GetListing(listingId).ConfigureAwait(false);


            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(listingResult.Payload.Published == true);
        }

        [TestMethod]
        public async Task PublishListingFailure()
        {
            //Arrange
            var expected = false;

            //Act
            var actual = await _listingsDataAccess.PublishListing(0).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
        }

    }
}