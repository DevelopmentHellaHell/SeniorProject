using System.Configuration;
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

namespace DevelopmentHell.Hubba.ListingProfile.Test.DAL
{
    [TestClass]
    public class ListingAvailabiltiesDataAccessUnitTests
    {

        private readonly ITestingService _testingService;
        private readonly IListingsDataAccess _listingsDataAccess;
        private readonly IListingAvailabilitiesDataAccess _listingAvailabilitiesDataAccess;

        private readonly string _listingProfileConnectionString = ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!;
        private readonly string _listingsTable = ConfigurationManager.AppSettings["ListingsTable"]!;
        private readonly string _listingAvailabilitiesTable = ConfigurationManager.AppSettings["ListingAvailabilitiesTable"]!;
        private readonly string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;

        private readonly string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;


        public ListingAvailabiltiesDataAccessUnitTests()
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

            _listingAvailabilitiesDataAccess = new ListingAvailabilitiesDataAccess(_listingProfileConnectionString, _listingAvailabilitiesTable);

            _listingsDataAccess = new ListingsDataAccess(_listingProfileConnectionString, _listingsTable);


            _testingService = new TestingService(_jwtKey, new TestsDataAccess());

        }

        [TestInitialize]
        public async Task Setup()
        {
            await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.LISTING_PROFILES).ConfigureAwait(false);
        }


        [TestMethod]
        public async Task AddListingAvailabilities()
        {
            //Arrange
            int ownerId = 1;
            string title = "Test Title";
            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;
            List<ListingAvailabilityDTO> addList = new();
            ListingAvailabilityDTO temp1 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
            };
            ListingAvailabilityDTO temp2 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(3),
            };
            ListingAvailabilityDTO temp3 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(5),
            };
            addList.Add(temp1);
            addList.Add(temp2);
            addList.Add(temp3);

            var expected = true;
            var expectedCount = 3;

            //Act
            var actual = await _listingAvailabilitiesDataAccess.AddListingAvailabilities(addList);
            var listingAvailability = await _listingAvailabilitiesDataAccess.GetListingAvailabilities(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(listingAvailability.Payload.Count == expectedCount);
        }

        [TestMethod]
        public async Task AddListingAvailabilitiesFailure()
        {
            //Arrange
            int ownerId = 1;
            string title = "Test Title";
            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;
            List<ListingAvailabilityDTO> addList = new();
            ListingAvailabilityDTO temp1 = new ListingAvailabilityDTO()
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
            };
            ListingAvailabilityDTO temp2 = new ListingAvailabilityDTO()
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(3),
            };

            addList.Add(temp1);
            addList.Add(temp2);

            var expected = false;

            //Act
            var actual = await _listingAvailabilitiesDataAccess.AddListingAvailabilities(addList);
            var listingAvailability = await _listingAvailabilitiesDataAccess.GetListingAvailabilities(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(listingAvailability.Payload.Count == 0);
        }

        [TestMethod]
        public async Task GetListingAvailabilities()
        {
            //Arrange
            int ownerId = 1;
            string title = "Test Title";
            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;
            List<ListingAvailabilityDTO> addList = new();
            ListingAvailabilityDTO temp1 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
            };
            ListingAvailabilityDTO temp2 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(3),
            };
            addList.Add(temp1);
            addList.Add(temp2);
            await _listingAvailabilitiesDataAccess.AddListingAvailabilities(addList);

            var expected = true;
            var expectedType = typeof(List<ListingAvailability>);


            //Act
            var actual = await _listingAvailabilitiesDataAccess.GetListingAvailabilities(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload is not null);
            Assert.IsTrue(actual.Payload.GetType() == expectedType);
            Assert.IsTrue(actual.Payload.Count == 2);
        }

        [TestMethod]
        public async Task GetListingAvailabilities_ByMonth()
        {
            //Arrange
            int ownerId = 1;
            string title = "Test Title";
            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;
            List<ListingAvailabilityDTO> addList = new();
            ListingAvailabilityDTO temp1 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
            };
            ListingAvailabilityDTO temp2 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(3),
            };
            addList.Add(temp1);
            addList.Add(temp2);
            await _listingAvailabilitiesDataAccess.AddListingAvailabilities(addList);

            var expected = true;
            var expectedType = typeof(List<ListingAvailability>);


            //Act
            var actual = await _listingAvailabilitiesDataAccess.GetListingAvailabilitiesByMonth(listingId, ((DateTime)temp1.StartTime).Month, ((DateTime)temp1.StartTime).Year).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload is not null);
            Assert.IsTrue(actual.Payload.GetType() == expectedType);
            Assert.IsTrue(actual.Payload.Count == 2);
        }

        [TestMethod]
        public async Task DeleteListingAvailabilities1()
        {
            //Arrange
            int ownerId = 1;
            string title = "Test Title";
            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;
            List<ListingAvailabilityDTO> addList = new();
            ListingAvailabilityDTO temp1 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
            };
            ListingAvailabilityDTO temp2 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(4),
            };

            addList.Add(temp1);
            addList.Add(temp2);

            var listingAvailability = await _listingAvailabilitiesDataAccess.GetListingAvailabilities(listingId).ConfigureAwait(false);
            await _listingAvailabilitiesDataAccess.AddListingAvailabilities(addList).ConfigureAwait(false);
            listingAvailability = await _listingAvailabilitiesDataAccess.GetListingAvailabilities(listingId).ConfigureAwait(false);
            List<ListingAvailabilityDTO> deleteList = new();
            deleteList.Add(new ListingAvailabilityDTO
            {
                ListingId = listingId,
                AvailabilityId = listingAvailability.Payload[0].AvailabilityId
            });

            var expected = true;
            //Act
            var actual = await _listingAvailabilitiesDataAccess.DeleteListingAvailabilities(deleteList).ConfigureAwait(false);
            listingAvailability = await _listingAvailabilitiesDataAccess.GetListingAvailabilities(listingId).ConfigureAwait(false);
            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful = expected);
            Assert.IsTrue(listingAvailability.IsSuccessful == true);
            Assert.IsTrue(listingAvailability.Payload!.Count == 1);
        }

        [TestMethod]
        public async Task DeleteListingAvailabilities2()
        {
            //Arrange
            int ownerId = 1;
            string title = "Test Title";
            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;
            List<ListingAvailabilityDTO> addList = new();
            ListingAvailabilityDTO temp1 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
            };
            ListingAvailabilityDTO temp2 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
            };

            addList.Add(temp1);
            addList.Add(temp2);

            var listingAvailability = await _listingAvailabilitiesDataAccess.GetListingAvailabilities(listingId).ConfigureAwait(false);
            await _listingAvailabilitiesDataAccess.AddListingAvailabilities(addList).ConfigureAwait(false);
            listingAvailability = await _listingAvailabilitiesDataAccess.GetListingAvailabilities(listingId).ConfigureAwait(false);
            List<ListingAvailabilityDTO> deleteList = new();
            foreach (ListingAvailability availability in listingAvailability.Payload)
            {
                ListingAvailabilityDTO temp = new()
                {
                    AvailabilityId = availability.AvailabilityId,
                    ListingId = (int)availability.ListingId,
                };
                deleteList.Add(temp);
            }

            var expected = true;
            //Act
            var actual = await _listingAvailabilitiesDataAccess.DeleteListingAvailabilities(deleteList).ConfigureAwait(false);
            listingAvailability = await _listingAvailabilitiesDataAccess.GetListingAvailabilities(listingId).ConfigureAwait(false);
            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful = expected);
            Assert.IsTrue(listingAvailability.IsSuccessful == true);
            Assert.IsTrue(listingAvailability.Payload!.Count == 0);
        }


        [TestMethod]
        public async Task UpdateListingAvailabilities()
        {
            //Arrange
            int ownerId = 1;
            string title = "Test Title";
            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            List<ListingAvailabilityDTO> addList = new();
            ListingAvailabilityDTO temp1 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
            };

            addList.Add(temp1);
            await _listingAvailabilitiesDataAccess.AddListingAvailabilities(addList).ConfigureAwait(false);

            var listingAvailability = await _listingAvailabilitiesDataAccess.GetListingAvailabilities(listingId).ConfigureAwait(false);
            ListingAvailabilityDTO temp2 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                AvailabilityId = (int)listingAvailability.Payload[0].AvailabilityId,
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(3),
            };

            var expected = true;

            //Act
            var actual = await _listingAvailabilitiesDataAccess.UpdateListingAvailability(temp2).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
        }
    }
}
