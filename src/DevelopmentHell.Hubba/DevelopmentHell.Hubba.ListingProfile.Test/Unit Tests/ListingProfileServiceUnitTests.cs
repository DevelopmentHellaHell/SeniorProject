using System.Configuration;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.ListingProfile.Service.Abstractions;
using DevelopmentHell.Hubba.ListingProfile.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.Notification.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Manager.Abstractions;
using DevelopmentHell.Hubba.Registration.Manager.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Implementations;

namespace DevelopmentHell.Hubba.ListingProfile.Test.Unit_Tests
{
    [TestClass]
    public class ListingProfileServiceUnitTests
    {
        private readonly IListingProfileService _listingProfileService;

        //helper
        private readonly ITestingService _testingService;
        private readonly IListingsDataAccess _listingsDataAccess;
        private readonly IUserAccountDataAccess _userAccountDataAccess;
        private readonly IRegistrationManager _registrationManager;
        private readonly IListingAvailabilitiesDataAccess _listingAvailabilitiesDataAccess;
        private readonly IListingHistoryDataAccess _listingHistoryDataAccess;


        private readonly string _userConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private readonly string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;

        private readonly string _listingProfileConnectionString = ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!;
        private readonly string _listingsTable = ConfigurationManager.AppSettings["ListingsTable"]!;
        private readonly string _listingAvailabilitiesTable = ConfigurationManager.AppSettings["ListingAvailabilitiesTable"]!;
        private readonly string _listingHistoryTable = ConfigurationManager.AppSettings["ListingHistoryTable"]!;
        private readonly string _listingRatingsTable = ConfigurationManager.AppSettings["ListingRatingsTable"]!;
        private readonly string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;

        private readonly string _notificationConnectionString = ConfigurationManager.AppSettings["NotificationsConnectionString"]!;
        private readonly string _userNotificationsTable = ConfigurationManager.AppSettings["UserNotificationsTable"]!;
        private readonly string _userNotificationSettingsTable = ConfigurationManager.AppSettings["NotificationSettingsTable"]!;

        private readonly string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;
        private string _cryptographyKey = ConfigurationManager.AppSettings["CryptographyKey"]!;


        public ListingProfileServiceUnitTests()
        {
            LoggerService loggerService = new LoggerService(
                new LoggerDataAccess(
                    _logsConnectionString,
                    _logsTable
                )
            );

            _listingProfileService = new ListingProfileService
            (
                new ListingsDataAccess(_listingProfileConnectionString, _listingsTable),
                new ListingAvailabilitiesDataAccess(_listingProfileConnectionString, _listingAvailabilitiesTable),
                new ListingHistoryDataAccess(_listingProfileConnectionString, _listingHistoryTable),
                new RatingDataAccess(_listingProfileConnectionString, _listingRatingsTable),
                new UserAccountDataAccess(_userConnectionString, _userAccountsTable),
                loggerService
            );

            _registrationManager = new RegistrationManager(
                new RegistrationService
                (
                    new UserAccountDataAccess
                    (
                        _userConnectionString,
                        _userAccountsTable
                    ),
                    new CryptographyService(
                        _cryptographyKey
                    ),
                    new ValidationService(),
                    loggerService
                ),
                new AuthorizationService
                (
                     new UserAccountDataAccess
                    (
                        _userConnectionString,
                        _userAccountsTable
                    ),
                      new JWTHandlerService(
                        _jwtKey
                    ),
                    loggerService
                ),
                new CryptographyService
                (
                    _cryptographyKey
                ),
                new NotificationService(
                    new NotificationDataAccess(
                        _notificationConnectionString,
                        _userNotificationsTable
                    ),
                    new NotificationSettingsDataAccess(
                        _notificationConnectionString,
                        _userNotificationSettingsTable
                    ),
                    new UserAccountDataAccess
                    (
                        _userConnectionString,
                        _userAccountsTable
                    ),
                    loggerService
                ),
                loggerService
            );

            _testingService = new TestingService(_jwtKey, new TestsDataAccess());

            _listingsDataAccess = new ListingsDataAccess(_listingProfileConnectionString, _listingsTable);

            _userAccountDataAccess = new UserAccountDataAccess(_userConnectionString, _userAccountsTable);

            _listingAvailabilitiesDataAccess = new ListingAvailabilitiesDataAccess(_listingProfileConnectionString, _listingAvailabilitiesTable);

            _listingHistoryDataAccess = new ListingHistoryDataAccess(_listingProfileConnectionString, _listingHistoryTable);
        }

        [TestInitialize]
        public async Task Setup()
        {
            await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.LISTING_PROFILES).ConfigureAwait(false);
            await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.USERS).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task CreateListing()
        {
            //Arrange
            var ownerId = 1;
            var title = "Title 1";

            var expected = true;

            //Act
            var actual = await _listingProfileService.CreateListing(ownerId, title).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task UpdateListingWithPriceDescLocation()
        {
            //Arrange
            var ownerId = 1;
            var title = "Title 1";
            await _listingProfileService.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var price = 56.39;
            var location = "1000 lol flexin";

            var expected = true;


            ListingEditorDTO listingEdit = new ListingEditorDTO()
            {
                ListingId = listingId,
                Title = title,
                OwnerId = ownerId,
                Description = null,
                Price = price,
                Location = location,
            };

            //Act
            var actual = await _listingProfileService.UpdateListing(listingEdit).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task UpdateListingRemoveLocation()
        {
            //Arrange
            var ownerId = 1;
            var title = "Title 1";
            await _listingProfileService.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var price = 56.39;
            var location = "1000 lol flexin";

            var expected = true;


            ListingEditorDTO listingEdit1 = new ListingEditorDTO()
            {
                ListingId = listingId,
                Title = title,
                OwnerId = ownerId,
                Description = null,
                Price = price,
                Location = location,
            };
            await _listingProfileService.UpdateListing(listingEdit1).ConfigureAwait(false);

            ListingEditorDTO listingEdit2 = new ListingEditorDTO()
            {
                ListingId = listingId,
                Title = title,
                OwnerId = ownerId,
                Description = null,
                Price = price,
                Location = null,
            };

            //Act
            var actual = await _listingProfileService.UpdateListing(listingEdit2).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task Getlisting()
        {
            // Arrange
            var email = "jeffrey@gmail.com";
            var password = "12345678";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var getOwnerId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            var ownerId = getOwnerId.Payload;
            var title = "Title 1";
            await _listingProfileService.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var price = 56.39;
            var location = "1000 lol flexin";
            var username = "jeffrey";

            var expected = true;


            ListingEditorDTO listingEdit = new ListingEditorDTO()
            {
                ListingId = listingId,
                Title = title,
                OwnerId = ownerId,
                Description = null,
                Price = price,
                Location = location,
            };
            await _listingsDataAccess.UpdateListing(listingEdit).ConfigureAwait(false);

            //Act
            var actual = await _listingProfileService.GetListing(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload is not null);
            Assert.IsTrue(actual.Payload.ListingId.Equals(listingId));
            Assert.IsTrue(actual.Payload.Title.Equals(title));
            Assert.IsTrue(actual.Payload.OwnerId.Equals(ownerId));
            Assert.IsTrue(actual.Payload.Price.Equals(price));
            Assert.IsTrue(actual.Payload.Location.Equals(location));
            Assert.IsTrue(actual.Payload.Description is null);
            Assert.IsTrue(actual.Payload.OwnerUsername.Equals(username));
            Assert.IsTrue(actual.Payload.AverageRating is null);
        }

        [TestMethod]
        public async Task GetListingFailure()
        {
            // Arrange
            var ownerId = 1;
            var title = "Title 1";
            await _listingProfileService.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var expected = false;
            var expectedErrorMessage = "Unable to retrieve username.";


            //Act
            var actual = await _listingProfileService.GetListing(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }

        [TestMethod]
        public async Task GetlistingWithRating()
        {
            // Arrange
            var email = "jeffrey@gmail.com";
            var password = "12345678";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var getOwnerId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            var ownerId = getOwnerId.Payload;
            var title = "Title 1";
            await _listingProfileService.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var expected = true;
            var expectedAvgRating = 4.0;

            var userId1 = 2;
            var rating1 = 3;
            var comment1 = "nice!";
            var anonymous1 = false;
            await _listingHistoryDataAccess.AddUser(listingId, userId1).ConfigureAwait(false);

            var userId2 = 3;
            var rating2 = 5;
            var anonymous2 = false;
            await _listingHistoryDataAccess.AddUser(listingId, userId2).ConfigureAwait(false);

            await _listingProfileService.AddRating(listingId, userId1, rating1, comment1, anonymous1).ConfigureAwait(false);
            await _listingProfileService.AddRating(listingId, userId2, rating2, null, anonymous2).ConfigureAwait(false);

            //Act
            var actual = await _listingProfileService.GetListing(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload is not null);
            Assert.IsTrue(actual.Payload.ListingId.Equals(listingId));
            Assert.IsTrue(actual.Payload.Title.Equals(title));
            Assert.IsTrue(actual.Payload.OwnerId.Equals(ownerId));
            Assert.IsTrue(actual.Payload.AverageRating.Equals(expectedAvgRating));
        }

        [TestMethod]
        public async Task GetListingRatings()
        {
            // Arrange
            var ownerId = 1;
            var title = "Title 1";
            await _listingProfileService.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var email1 = "jeffrey@gmail.com";
            var email2 = "jacob@gmail.com";
            var password = "12345678";

            await _registrationManager.Register(email1, password).ConfigureAwait(false);
            var getUserId1 = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int userId1 = getUserId1.Payload;

            await _registrationManager.Register(email2, password).ConfigureAwait(false);
            var getUserId2 = await _userAccountDataAccess.GetId(email2).ConfigureAwait(false);
            int userId2 = getUserId2.Payload;


            var expected = true;
            var rating1 = 3;
            var comment1 = "nice!";
            var anonymous1 = false;
            await _listingHistoryDataAccess.AddUser(listingId, userId1).ConfigureAwait(false);

            var rating2 = 5;
            var anonymous2 = false;
            await _listingHistoryDataAccess.AddUser(listingId, userId2).ConfigureAwait(false);

            await _listingProfileService.AddRating(listingId, userId1, rating1, comment1, anonymous1).ConfigureAwait(false);
            await _listingProfileService.AddRating(listingId, userId2, rating2, null, anonymous2).ConfigureAwait(false);

            var expectedRatings = new List<int>() { rating1, rating2 };
            var expectedUserIds = new List<int>() { userId1, userId2 };
            var expectedUsernames = new List<string> { "jacob", "jeffrey" };

            bool errorFound = false;

            //Act
            var actual = await _listingProfileService.GetListingRatings(listingId).ConfigureAwait(false);
            Console.WriteLine(actual.ErrorMessage);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload is not null);
            foreach (ListingRatingViewDTO ratingDTO in actual.Payload)
            {
                if (!expectedRatings.Any(expectedRating => expectedRating == ratingDTO.Rating))
                {
                    errorFound = true;
                    break;
                }

                if (!expectedUserIds.Any(expectedUserId => expectedUserId == ratingDTO.UserId))
                {
                    errorFound = true;
                    break;
                }

                if (!expectedUsernames.Any(expectedUsername => expectedUsername == ratingDTO.Username))
                {
                    errorFound = true;
                    break;
                }
            }
            Assert.IsTrue(errorFound == false);
        }

        [TestMethod]
        public async Task DeleteListing()
        {
            //Arrange
            var ownerId = 1;
            var title = "Title 1";
            await _listingProfileService.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var expected = true;

            //Act
            var actual = await _listingProfileService.DeleteListing(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task AddAvailabilities()
        {
            //Arrange
            int ownerId = 1;
            string title = "Test Title";
            await _listingProfileService.CreateListing(ownerId, title).ConfigureAwait(false);
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

            //Act
            var actual = await _listingProfileService.AddListingAvailabilities(addList);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task DeleteListingWithTables()
        {
            //Arrange
            int ownerId = 1;
            string title = "Test Title";
            await _listingProfileService.CreateListing(ownerId, title).ConfigureAwait(false);
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
            await _listingProfileService.AddListingAvailabilities(addList);

            var userId = 2;
            var rating1 = 4;
            var comment1 = "nice!";
            var anonymous1 = false;
            await _listingHistoryDataAccess.AddUser(listingId, 2).ConfigureAwait(false);
            await _listingProfileService.AddRating(listingId, userId, rating1, comment1, anonymous1).ConfigureAwait(false);

            var expected = true;

            //Act
            var actual = await _listingProfileService.DeleteListing(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task PublishListing()
        {
            //Arrange
            var ownerId = 1;
            var title = "Title 1";
            await _listingProfileService.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var expected = true;

            //Act
            var actual = await _listingProfileService.PublishListing(listingId).ConfigureAwait(false);
            var getListing = await _listingsDataAccess.GetListing(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(getListing.Payload.Published == true);
        }

        [TestMethod]
        public async Task AddRating()
        {
            //Arrange
            var ownerId = 1;
            var title = "Title 1";
            await _listingProfileService.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var userId = 2;
            var rating = 4;
            var comment = "nice!";
            var anonymous = false;
            await _listingHistoryDataAccess.AddUser(listingId, 2).ConfigureAwait(false);

            var expected = true;

            //Act
            var actual = await _listingProfileService.AddRating(listingId, userId, rating, comment, anonymous).ConfigureAwait(false);


            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task UpdateRating()
        {
            //Arrange
            var ownerId = 1;
            var title = "Title 1";
            await _listingProfileService.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var userId = 2;
            var rating1 = 4;
            var comment1 = "nice!";
            var anonymous1 = false;
            await _listingHistoryDataAccess.AddUser(listingId, 2).ConfigureAwait(false);
            await _listingProfileService.AddRating(listingId, userId, rating1, comment1, anonymous1).ConfigureAwait(false);


            var newRating = 2;
            var newAnon = true;
            ListingRatingEditorDTO ratingEdit = new ListingRatingEditorDTO()
            {
                ListingId = listingId,
                UserId = userId,
                Rating = newRating,
                Anonymous = newAnon
            };

            var expected = true;

            //Act
            var actual = await _listingProfileService.UpdateRating(ratingEdit).ConfigureAwait(false);


            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task DeleteRating()
        {
            //Arrange
            var ownerId = 1;
            var title = "Title 1";
            await _listingProfileService.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var userId = 2;
            var rating1 = 4;
            var comment1 = "nice!";
            var anonymous1 = false;
            await _listingHistoryDataAccess.AddUser(listingId, userId).ConfigureAwait(false);
            await _listingProfileService.AddRating(listingId, userId, rating1, comment1, anonymous1).ConfigureAwait(false);

            var expected = true;

            //Act
            var actual = await _listingProfileService.DeleteRating(listingId, userId).ConfigureAwait(false);


            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
        }


        [TestMethod]
        public async Task UpdateAvailabilities()
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
                StartTime = DateTime.Now.AddDays(1),
                EndTime = DateTime.Now.AddDays(1).AddHours(1),
            };

            addList.Add(temp1);
            addList.Add(temp2);
            await _listingAvailabilitiesDataAccess.AddListingAvailabilities(addList).ConfigureAwait(false);

            var listingAvailability = await _listingAvailabilitiesDataAccess.GetListingAvailabilities(listingId).ConfigureAwait(false);
            temp1 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                AvailabilityId = (int)listingAvailability.Payload[0].AvailabilityId,
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(3),
            };

            temp2 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                AvailabilityId = (int)listingAvailability.Payload[1].AvailabilityId,
                StartTime = DateTime.Now.AddDays(1).AddHours(2),
                EndTime = DateTime.Now.AddDays(1).AddHours(3),
            };

            List<ListingAvailabilityDTO> updateList = new();
            var expected = true;

            //Act
            var actual = await _listingProfileService.UpdateListingAvailabilities(updateList).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task GetListingOwnerId()
        {
            //Arrange
            int ownerId = 1;
            string title = "Test Title";
            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var expected = true;

            //Act
            var actual = await _listingProfileService.GetListingOwnerId(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload.Equals(ownerId));
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
            var start1 = DateTime.Now;
            var end1 = DateTime.Now.AddHours(1);

            var start2 = DateTime.Now.AddDays(1);
            var end2 = DateTime.Now.AddDays(1).AddHours(1);
            List<ListingAvailabilityDTO> addList = new();
            ListingAvailabilityDTO temp1 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = start1,
                EndTime = end1,
            };
            ListingAvailabilityDTO temp2 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = start2,
                EndTime = end2,
            };
            addList.Add(temp1);
            addList.Add(temp2);
            await _listingAvailabilitiesDataAccess.AddListingAvailabilities(addList);

            var expected = true;
            var expectedStarts = new List<DateTime>() { start1, start2 };
            var expectedEnds = new List<DateTime>() { end1, end2 };
            bool errorFound = false;


            //Act
            var actual = await _listingProfileService.GetListingAvailabilities(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload is not null);
            Assert.IsTrue(actual.Payload[0].ListingId.Equals(listingId));
            foreach (ListingAvailabilityViewDTO availability in actual.Payload)
            {
                if (!expectedStarts.Any(expectedStart => expectedStart.ToString().Trim().CompareTo(availability.StartTime.ToString().Trim()) == 0))
                {
                    errorFound = true;
                    break;
                }

                if (!expectedEnds.Any(expectedEnd => expectedEnd.ToString().Trim().CompareTo(availability.EndTime.ToString().Trim()) == 0))
                {
                    errorFound = true;
                    break;
                }

                if (!availability.ListingId.Equals(listingId))
                {
                    errorFound = true;
                    break;
                }
            }
            Assert.IsTrue(errorFound == false);
        }

        [TestMethod]
        public async Task DeleteListingAvailabilities()
        {
            //Arrange
            int ownerId = 1;
            string title = "Test Title";
            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;
            var start1 = DateTime.Now;
            var end1 = DateTime.Now.AddHours(1);

            var start2 = DateTime.Now.AddDays(1);
            var end2 = DateTime.Now.AddDays(1).AddHours(1);
            List<ListingAvailabilityDTO> addList = new();
            ListingAvailabilityDTO temp1 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = start1,
                EndTime = end1,
            };
            ListingAvailabilityDTO temp2 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = start2,
                EndTime = end2,
            };
            addList.Add(temp1);
            addList.Add(temp2);
            await _listingAvailabilitiesDataAccess.AddListingAvailabilities(addList);

            var expected = true;
            var listingAvailability = await _listingAvailabilitiesDataAccess.GetListingAvailabilities(listingId).ConfigureAwait(false);
            temp1 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                AvailabilityId = (int)listingAvailability.Payload[0].AvailabilityId,
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(3),
            };

            temp2 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                AvailabilityId = (int)listingAvailability.Payload[1].AvailabilityId,
                StartTime = DateTime.Now.AddDays(1).AddHours(2),
                EndTime = DateTime.Now.AddDays(1).AddHours(3),
            };

            List<ListingAvailabilityDTO> deleteList = new();
            deleteList.Add(temp1);
            deleteList.Add(temp2);

            //Act

            var actual = await _listingProfileService.DeleteListingAvailabilities(deleteList).ConfigureAwait(false);
            var get = await _listingProfileService.GetListingAvailabilities(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(get.Payload is not null);
            Assert.IsTrue(get.Payload.Count == 0);
        }

        [TestMethod]
        public async Task DeleteListingAvailabilitiesWith1()
        {
            //Arrange
            int ownerId = 1;
            string title = "Test Title";
            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;
            var start1 = DateTime.Now;
            var end1 = DateTime.Now.AddHours(1);

            var start2 = DateTime.Now.AddDays(1);
            var end2 = DateTime.Now.AddDays(1).AddHours(1);
            List<ListingAvailabilityDTO> addList = new();
            ListingAvailabilityDTO temp1 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = start1,
                EndTime = end1,
            };
            ListingAvailabilityDTO temp2 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = start2,
                EndTime = end2,
            };
            ListingAvailabilityDTO temp3 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = start1,
                EndTime = end2,
            };
            addList.Add(temp1);
            addList.Add(temp2);
            addList.Add(temp3);
            await _listingAvailabilitiesDataAccess.AddListingAvailabilities(addList);

            var expected = true;
            var listingAvailability = await _listingAvailabilitiesDataAccess.GetListingAvailabilities(listingId).ConfigureAwait(false);
            temp1 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                AvailabilityId = (int)listingAvailability.Payload[0].AvailabilityId,
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(3),
            };

            temp2 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                AvailabilityId = (int)listingAvailability.Payload[1].AvailabilityId,
                StartTime = DateTime.Now.AddDays(1).AddHours(2),
                EndTime = DateTime.Now.AddDays(1).AddHours(3),
            };

            List<ListingAvailabilityDTO> deleteList = new();
            deleteList.Add(temp1);
            deleteList.Add(temp2);

            //Act

            var actual = await _listingProfileService.DeleteListingAvailabilities(deleteList).ConfigureAwait(false);
            var get = await _listingProfileService.GetListingAvailabilities(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(get.Payload is not null);
            Assert.IsTrue(get.Payload.Count == 1);
        }

        [TestMethod]
        public async Task CheckListingHistoryFalse()
        {
            //Arrange
            int ownerId = 1;
            string title = "Test Title";
            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            int userId = 2;

            var expected = true;
            var expectedValue = false;

            //Act
            var actual = await _listingProfileService.CheckListingHistory(listingId, userId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload.Equals(expectedValue));
        }

        [TestMethod]
        public async Task CheckListingHistoryTrue()
        {
            //Arrange
            int ownerId = 1;
            string title = "Test Title";
            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            int userId = 2;
            await _listingHistoryDataAccess.AddUser(listingId, userId).ConfigureAwait(false);

            var expected = true;
            var expectedValue = true;

            //Act
            var actual = await _listingProfileService.CheckListingHistory(listingId, userId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload.Equals(expectedValue));
        }

        [TestMethod]
        public async Task CheckListingRatingTrue()
        {
            //Arrange
            int ownerId = 1;
            string title = "Test Title";
            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var userId = 2;
            var rating1 = 4;
            var comment1 = "nice!";
            var anonymous1 = false;
            await _listingHistoryDataAccess.AddUser(listingId, userId).ConfigureAwait(false);
            await _listingProfileService.AddRating(listingId, userId, rating1, comment1, anonymous1).ConfigureAwait(false);


            var expected = true;
            var expectedValue = true;

            //Act
            var actual = await _listingProfileService.CheckListingRating(listingId, userId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload.Equals(expectedValue));
        }

        [TestMethod]
        public async Task CheckListingRatingFalse()
        {
            //Arrange
            int ownerId = 1;
            string title = "Test Title";
            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var listingIdResult = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = (int)listingIdResult.Payload;

            var userId = 2;


            var expected = true;
            var expectedValue = false;

            //Act
            var actual = await _listingProfileService.CheckListingRating(listingId, userId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload.Equals(expectedValue));
        }

        [TestMethod]
        public async Task GetUserListings()
        {
            //Arrange
            var ownerId = 1;

            var title1 = "Listing Test Title 1";
            var title2 = "Listing Test Title 2";
            var title3 = "Listing Test Title 3";

            await _listingProfileService.CreateListing(ownerId, title1).ConfigureAwait(false);
            await _listingProfileService.CreateListing(ownerId, title2).ConfigureAwait(false);
            await _listingProfileService.CreateListing(ownerId, title3).ConfigureAwait(false);

            var expected = true;
            var expectedType = typeof(List<ListingViewDTO>);
            var expectedTitles = new List<string>() { title1, title2, title3 };
            bool errorFound = false;

            //Act
            var actual = await _listingProfileService.GetUserListings(ownerId).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful = expected);
            Assert.IsTrue(actual.Payload is not null);
            Assert.IsTrue(actual.Payload.GetType() == expectedType);
            Assert.IsTrue(actual.Payload.Count == 3);
            foreach (ListingViewDTO viewDTO in actual.Payload)
            {
                if (!expectedTitles.Any(expectedTitle => expectedTitle.Equals(viewDTO.Title)))
                {
                    errorFound = true;
                    break;
                }

                if (!viewDTO.OwnerId.Equals(ownerId))
                {
                    errorFound = true;
                    break;
                }
            }
            Assert.IsTrue(errorFound == false);
        }
    }
}
