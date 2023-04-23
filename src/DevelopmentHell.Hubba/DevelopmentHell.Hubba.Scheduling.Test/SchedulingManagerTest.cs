using Development.Hubba.JWTHandler.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Notification.Service.Abstractions;
using DevelopmentHell.Hubba.Notification.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Manager;
using DevelopmentHell.Hubba.Scheduling.Service.Abstractions;
using DevelopmentHell.Hubba.Scheduling.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using System.Configuration;
using DevelopmentHell.Hubba.Scheduling.Manager;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Manager.Abstractions;
using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Manager.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using DevelopmentHell.Hubba.Authentication.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using Microsoft.IdentityModel.Tokens;

namespace DevelopmentHell.Hubba.Scheduling.Test.Manager
{
    [TestClass]
    public class SchedulingManagerTest
    {
        private readonly ICryptographyService _cryptographyService;
        private readonly IValidationService _validationService;
        private readonly IRegistrationService _registrationService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IJWTHandlerService _jwtHandlerService;
        private readonly ILoggerService _loggerService;
        private readonly INotificationService _notificationService;
        private readonly IBookingService _bookingService;
        private readonly IAvailabilityService _availabilityService;
        private readonly ITestingService _testingService;

        private readonly IUserAccountDataAccess _userAccountDataAccess;
        private readonly ILoggerDataAccess _loggerDataAccess;
        private readonly IListingsDataAccess _listingsDataAccess;
        private readonly IListingAvailabilitiesDataAccess _listingAvailabilitiesDataAccess;
        private readonly IBookingsDataAccess _bookingsDataAccess;
        private readonly IBookedTimeFramesDataAccess _bookedTimeFramesDataAccess;

        private readonly IRegistrationManager _registrationManager;
        private readonly ISchedulingManager _schedulingManager;


        public SchedulingManagerTest() 
        {
            //other services
            _loggerDataAccess = new LoggerDataAccess(
                    ConfigurationManager.AppSettings["LogsConnectionString"]!,
                    ConfigurationManager.AppSettings["UserNotificationsTable"]!
                );
            _loggerService = new LoggerService(_loggerDataAccess);

            _jwtHandlerService = new JWTHandlerService(
                ConfigurationManager.AppSettings["JwtKey"]!
            );
            _userAccountDataAccess = new UserAccountDataAccess(
                    ConfigurationManager.AppSettings["UsersConnectionString"]!,
                    ConfigurationManager.AppSettings["UserAccountsTable"]!
            );
            _cryptographyService = new CryptographyService(
                ConfigurationManager.AppSettings["CryptographyKey"]!
            );
            _validationService = new ValidationService();
            _registrationService = new RegistrationService(
                _userAccountDataAccess,
                _cryptographyService,
                _validationService,
                _loggerService
            );
            
            _authenticationService = new AuthenticationService(
                _userAccountDataAccess,
                new UserLoginDataAccess(
                    ConfigurationManager.AppSettings["UsersConnectionString"]!,
                    ConfigurationManager.AppSettings["UserLogins"]!
                ),
                _cryptographyService,
                _jwtHandlerService,
                _validationService,
                _loggerService
            );

            _authorizationService = new AuthorizationService(_userAccountDataAccess, _jwtHandlerService,_loggerService);
            _notificationService = new NotificationService(
                new NotificationDataAccess(
                    ConfigurationManager.AppSettings["NotificationsConnectionString"]!,
                    ConfigurationManager.AppSettings["UserNotificationsTable"]!
                ),
                new NotificationSettingsDataAccess (
                    ConfigurationManager.AppSettings["NotificationsConnectionString"]!,
                    ConfigurationManager.AppSettings["NotificationSettingsTable"]!
                ),
                _userAccountDataAccess,
                _loggerService
            );
            _testingService = new TestingService(
                ConfigurationManager.AppSettings["JwtKey"]!,
                new TestsDataAccess()
            );
            
            // listingsDAO, listingAvailDAO, bookingsDAO, bookedTimeFramesDAO
            _listingsDataAccess = new ListingsDataAccess(
                ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!,
                ConfigurationManager.AppSettings["ListingsTable"]!
            );
            _listingAvailabilitiesDataAccess = new ListingAvailabilitiesDataAccess(
                ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!,
                ConfigurationManager.AppSettings["ListingAvailabilitiesTable"]!
            );
            _bookingsDataAccess = new BookingsDataAccess(
                ConfigurationManager.AppSettings["SchedulingsConnectionString"]!,
                ConfigurationManager.AppSettings["BookingsTable"]!
            );
            _bookedTimeFramesDataAccess = new BookedTimeFramesDataAccess(
                ConfigurationManager.AppSettings["SchedulingsConnectionString"]!,
                ConfigurationManager.AppSettings["BookedTimeFramesTable"]!
            );
            // bookingService, availabilityService
            _bookingService = new BookingService(_bookingsDataAccess, _bookedTimeFramesDataAccess);
            _availabilityService = new AvailabilityService(_listingsDataAccess, _listingAvailabilitiesDataAccess, _bookedTimeFramesDataAccess);

            // Managers
            _registrationManager = new RegistrationManager(
                _registrationService,
                _authorizationService,
                _cryptographyService,
                _notificationService,
                _loggerService
            );
            _schedulingManager = new SchedulingManager(
                _bookingService,
                _availabilityService,
                _authorizationService,
                _notificationService,
                _loggerService
            );
        }

        [TestInitialize]
        [TestCleanup]
        public async Task CleanUp()
        {
            await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.LISTING_PROFILES).ConfigureAwait(false);
        }

        private async Task<Dictionary<string, object>> SetUpTestData()
        {
            var testData = new Dictionary<string, object>();
            int ownerId = 200;
            string title = "Scheduling Manager test";
            testData["OwnerId"] = ownerId;
            testData["Title"] = title;

            // create a listing then get the listingId from inserted row
            var createListing = await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            Result<int> getListingId = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;
            testData["ListingId"] = listingId;

            var listingAvailabilities = new List<ListingAvailabilityDTO>()
                {
                    new ListingAvailabilityDTO()
                    {
                        ListingId = listingId,
                        StartTime = DateTime.Today.AddDays(2).AddHours(8),
                        EndTime = DateTime.Today.AddDays(2).AddHours(18)
                    },
                    new ListingAvailabilityDTO()
                    {
                        ListingId = listingId,
                        StartTime = DateTime.Today.AddDays(3).AddHours(8),
                        EndTime = DateTime.Today.AddDays(3).AddHours(18)
                    },
                    new ListingAvailabilityDTO()
                    {
                        ListingId = listingId,
                        StartTime = DateTime.Today.AddMonths(1).AddHours(8),
                        EndTime = DateTime.Today.AddMonths(1).AddHours(18)
                    },
                    new ListingAvailabilityDTO()
                    {
                        ListingId = listingId,
                        StartTime = DateTime.Today.AddMonths(1).AddDays(1).AddHours(8),
                        EndTime = DateTime.Today.AddMonths(1).AddDays(1).AddHours(18)
                    }
                };

            // create Listing Availability
            var createAvailability = await _listingAvailabilitiesDataAccess.AddListingAvailabilities(listingAvailabilities).ConfigureAwait(false);
            var getAvailabilities = await _listingAvailabilitiesDataAccess.GetListingAvailabilities(listingId).ConfigureAwait(false);
            testData["ListingAvailabilities"] = getAvailabilities.Payload;

            // add Booking
            Booking booking = new Booking()
            {
                UserId = 1,
                ListingId = listingId,
                FullPrice = 150,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreationDate = DateTime.Now,
                LastEditUser = 1,
            };
            testData["Booking"] = booking;

            Result<int> addBooking = await _bookingsDataAccess.CreateBooking(booking).ConfigureAwait(false);
            int bookingId = addBooking.Payload;
            testData["BookingId"] = bookingId;
            // add BookedTimeFrame
            List<BookedTimeFrame> bookedTimeFrame_0 = new List<BookedTimeFrame>()
            {
                new BookedTimeFrame()
                {
                    BookingId = bookingId,
                    ListingId = listingId,
                    AvailabilityId = (int) ((List<ListingAvailability>)testData["ListingAvailabilities"])[0].AvailabilityId,
                    StartDateTime = DateTime.Today.AddDays(2).AddHours(8),
                    EndDateTime = DateTime.Today.AddDays(2).AddHours(11)
                },
                new BookedTimeFrame()
                {
                    BookingId = bookingId,
                    ListingId = listingId,
                    AvailabilityId = (int) ((List<ListingAvailability>)testData["ListingAvailabilities"])[0].AvailabilityId,
                    StartDateTime = DateTime.Today.AddDays(2).AddHours(13),
                    EndDateTime = DateTime.Today.AddDays(2).AddHours(15)
                }
            };
            Result<bool> addBookedTimeFrames = await _bookedTimeFramesDataAccess.CreateBookedTimeFrames(bookingId, bookedTimeFrame_0).ConfigureAwait(false);
            testData["BookedTimeFrames"] = bookedTimeFrame_0;

            int month = ((List<ListingAvailability>)testData["ListingAvailabilities"])[0].StartTime.Month;
            int year = ((List<ListingAvailability>)testData["ListingAvailabilities"])[0].StartTime.Year;
            testData["Month"] = month;
            testData["Year"] = year;

            return testData;
        }

        [TestMethod]
        public async Task FindListingAvailabilityByMonth_PayloadEmpty_Successful()
        {
            //Arrange
            var testData = await SetUpTestData().ConfigureAwait(false);
            int listingId = (int)testData["ListingId"];
            int month = 10;
            int year = 2023;
            
            //Act
            var actual = await _schedulingManager.FindListingAvailabiityByMonth(listingId, month, year).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.IsTrue(actual.Payload.Count == 0);
        }
        [TestMethod]
        public async Task FindListingAvailabilityByMonth_PayloadNotEmpty_Successful()
        {
            //Arrange
            var testData = await SetUpTestData().ConfigureAwait(false);
            int listingId = (int)testData["ListingId"];
            int avail_0 = (int)((List<ListingAvailability>)testData["ListingAvailabilities"])[0].AvailabilityId;
            int avail_1 = (int)((List<ListingAvailability>)testData["ListingAvailabilities"])[1].AvailabilityId;
            int month = (int)testData["Month"];
            int year = (int)testData["Year"];
            var expected = new List<ListingAvailabilityDTO>()
            {
                new ListingAvailabilityDTO() { AvailabilityId = avail_0, StartTime = DateTime.Today.AddDays(2).AddHours(11), EndTime = DateTime.Today.AddDays(2).AddHours(13) },
                new ListingAvailabilityDTO() { AvailabilityId = avail_0, StartTime = DateTime.Today.AddDays(2).AddHours(15), EndTime = DateTime.Today.AddDays(2).AddHours(18) },
                new ListingAvailabilityDTO(){ AvailabilityId = avail_1, StartTime = DateTime.Today.AddDays(3).AddHours(8), EndTime = DateTime.Today.AddDays(3).AddHours(18) },
            };
            
            //Act
            var actual = await _schedulingManager.FindListingAvailabiityByMonth(listingId, month, year).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.IsTrue(expected.GetType() == actual.Payload.GetType());
            Assert.AreEqual(expected.Count, actual.Payload.Count);
        }
        [TestMethod]
        public async Task ReserveBooking_InvalidListingId_Failed()
        {
            //Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationManager.Register(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var userId = userIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(userId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(userId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);

            }
            //  - Setup test data
            var testData = await SetUpTestData().ConfigureAwait(false);
            int listingId = (int)testData["ListingId"];
            int availabilityId = (int)((List<ListingAvailability>)testData["ListingAvailabilities"])[0].AvailabilityId;
            float fullPrice = 100;
            int invalidListingId = 0;
            List<BookedTimeFrame> chosenTimeFrames = new()
            {
                new BookedTimeFrame()
                {
                    ListingId = invalidListingId,                 
                    AvailabilityId = availabilityId, 
                    StartDateTime = DateTime.Today.AddDays(2).AddHours(11),
                    EndDateTime = DateTime.Today.AddDays(2).AddHours(12) 
                },
            };
            
            //Act
            var actual = await _schedulingManager.ReserveBooking(userId, invalidListingId, fullPrice, chosenTimeFrames).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsSuccessful);
        }
        [TestMethod]
        public async Task ReserveBooking_OwnerTryToBookTheirListing_Failed()
        {
            //Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationManager.Register(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var userId = userIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(userId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(userId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);

            }
            //  - Setup test data
            var testData = await SetUpTestData().ConfigureAwait(false);
            int ownerId = (int)testData["OwnerId"];
            int listingId = (int)testData["ListingId"];
            int availabilityId = (int)((List<ListingAvailability>)testData["ListingAvailabilities"])[0].AvailabilityId;
            float fullPrice = 100;
            List<BookedTimeFrame> chosenTimeFrames = new()
            {
                new BookedTimeFrame(){
                    ListingId = listingId,
                    AvailabilityId = availabilityId, 
                    StartDateTime = DateTime.Today.AddDays(2).AddHours(11),
                    EndDateTime = DateTime.Today.AddDays(2).AddHours(12) 
                },
            };
            //Act
            var actual = await _schedulingManager.ReserveBooking(ownerId, listingId, fullPrice, chosenTimeFrames).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsSuccessful);
            Assert.IsFalse(actual.ErrorMessage.IsNullOrEmpty());
        }
        [TestMethod]
        public async Task ReserveBooking_UnauthorizedUser_Failed()
        {
            //Arrange
            //  - Setup test data
            var testData = await SetUpTestData().ConfigureAwait(false);
            int listingId = (int)testData["ListingId"];
            int availabilityId = (int)((List<ListingAvailability>)testData["ListingAvailabilities"])[0].AvailabilityId;
            float fullPrice = 100;
            List<BookedTimeFrame> chosenTimeFrames = new()
            {
                new BookedTimeFrame()
                {
                    ListingId = listingId,
                    AvailabilityId = availabilityId, 
                    StartDateTime = DateTime.Today.AddDays(2).AddHours(11),
                    EndDateTime = DateTime.Today.AddDays(2).AddHours(12) 
                },
            };
            int randomUserId = 0;

            //Act
            var actual = await _schedulingManager.ReserveBooking(randomUserId, listingId, fullPrice, chosenTimeFrames).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull (actual);
            Assert.IsFalse(actual.IsSuccessful);

        }
        [TestMethod]
        public async Task ReserveBooking_InvalidTimeFramesInput_Failed()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationManager.Register(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var userId = userIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(userId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(userId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            //  - Setup test data
            var testData = await SetUpTestData().ConfigureAwait(false);
            int listingId = (int)testData["ListingId"];
            int availabilityId = (int)((List<ListingAvailability>)testData["ListingAvailabilities"])[0].AvailabilityId;
            float fullPrice = 100;
            List<BookedTimeFrame> chosenTimeFrames = new()
            {
                new BookedTimeFrame()
                {
                    ListingId = listingId,
                    AvailabilityId = availabilityId,
                    StartDateTime = DateTime.Today.AddDays(2).AddHours(9),
                    EndDateTime = DateTime.Today.AddDays(3).AddHours(11)
                },
            };
            //Act
            var actual = await _schedulingManager.ReserveBooking(userId, listingId, fullPrice, chosenTimeFrames).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsSuccessful);
        }

            [TestMethod]
        public async Task ReserveBooking_ChosenTimeFramesAlreadyBooked_Failed() 
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationManager.Register(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var userId = userIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(userId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(userId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
            }
            //  - Setup test data
            var testData = await SetUpTestData().ConfigureAwait(false);
            int listingId = (int)testData["ListingId"];
            int availabilityId = (int)((List<ListingAvailability>)testData["ListingAvailabilities"])[0].AvailabilityId;
            float fullPrice = 100;
            List<BookedTimeFrame> chosenTimeFrames = new()
            {
                new BookedTimeFrame()
                {
                    ListingId = listingId,
                    AvailabilityId = availabilityId, 
                    StartDateTime = DateTime.Today.AddDays(2).AddHours(9),
                    EndDateTime = DateTime.Today.AddDays(2).AddHours(11) 
                },
            };
            //Act
            var actual = await _schedulingManager.ReserveBooking(userId, listingId, fullPrice, chosenTimeFrames).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsSuccessful);
        }
        [TestMethod]
        public async Task ReserveBooking_Successful()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationManager.Register(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var userId = userIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(userId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(userId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);

            }
            //  - Setup test data
            var testData = await SetUpTestData().ConfigureAwait(false);
            int listingId = (int) testData["ListingId"];
            int availabilityId = (int)((List<ListingAvailability>)testData["ListingAvailabilities"])[0].AvailabilityId;
            float fullPrice = 100;
            List<BookedTimeFrame> chosenTimeFrames = new()
            {
                new BookedTimeFrame()
                {
                    ListingId = listingId,
                    AvailabilityId = availabilityId, 
                    StartDateTime = DateTime.Today.AddDays(2).AddHours(11),
                    EndDateTime = DateTime.Today.AddDays(2).AddHours(12) 
                },
            };

            //Act
            var actual = await _schedulingManager.ReserveBooking(userId, listingId, fullPrice, chosenTimeFrames).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(actual.Payload.GetType(), typeof(BookingViewDTO));
        }
        [TestMethod]
        public async Task CancelBooking_AuthorizedUser_BookingBelongsToOtherUser_Failed()
        {
            //Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationManager.Register(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var userId = userIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(userId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(userId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);

            }
            //  - Setup test data
            var testData = await SetUpTestData().ConfigureAwait(false);
            int listingId = (int)testData["ListingId"];
            int availabilityId = (int)((List<ListingAvailability>)testData["ListingAvailabilities"])[0].AvailabilityId;
            float fullPrice = 100;
            List<BookedTimeFrame> chosenTimeFrames = new()
            {
                new BookedTimeFrame()
                {
                    ListingId = listingId,
                    AvailabilityId = availabilityId,
                    StartDateTime = DateTime.Today.AddDays(2).AddHours(11),
                    EndDateTime = DateTime.Today.AddDays(2).AddHours(12)
                }
            };
            int validBookingId = (int)testData["BookingId"];
            //Act
            var actual = await _schedulingManager.CancelBooking(userId, validBookingId).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsSuccessful);
        }
        [TestMethod]
        public async Task CancelBooking_UnauthorizedUser_Failed()
        {
            // Arrange
            //  - Setup test data
            var testData = await SetUpTestData().ConfigureAwait(false);
            int bookingId = (int)testData["BookingId"];
            int userId = ((Booking)testData["Booking"]).UserId;
            int listingId = (int)testData["ListingId"];
            int availabilityId = (int)((List<ListingAvailability>)testData["ListingAvailabilities"])[0].AvailabilityId;
            float fullPrice = 100;
            List<BookedTimeFrame> chosenTimeFrames = new()
            {
                new BookedTimeFrame()
                {
                    ListingId = listingId,
                    AvailabilityId = availabilityId,
                    StartDateTime = DateTime.Today.AddDays(2).AddHours(11),
                    EndDateTime = DateTime.Today.AddDays(2).AddHours(12)
                }
            };

            //Act
            var actual = await _schedulingManager.CancelBooking(userId, bookingId).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsSuccessful);
        }

        [TestMethod]
        public async Task CancelBooking_NonExistingBooking_Failed()
        {
            // Arrange
            //  - Setup test data
            var testData = await SetUpTestData().ConfigureAwait(false);
            int userId = ((Booking)testData["Booking"]).UserId;
            int listingId = (int)testData["ListingId"];
            int availabilityId = (int)((List<ListingAvailability>)testData["ListingAvailabilities"])[0].AvailabilityId;
            float fullPrice = 100;
            List<BookedTimeFrame> chosenTimeFrames = new()
            {
                new BookedTimeFrame()
                {
                    ListingId = listingId,
                    AvailabilityId = availabilityId,
                    StartDateTime = DateTime.Today.AddDays(2).AddHours(11),
                    EndDateTime = DateTime.Today.AddDays(2).AddHours(12)
                }
            };
            int nonExistingBookingId = 0;
            //Act
            var actual = await _schedulingManager.CancelBooking(userId, nonExistingBookingId).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsSuccessful);
        }
        [TestMethod]
        public async Task CancelBooking_Successful()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationManager.Register(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var userId = userIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(userId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(userId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);

            }
            //  - Setup test data
            var testData = await SetUpTestData().ConfigureAwait(false);
            int listingId = (int)testData["ListingId"];
            int availabilityId = (int)((List<ListingAvailability>)testData["ListingAvailabilities"])[0].AvailabilityId;
            float fullPrice = 100;
            List<BookedTimeFrame> chosenTimeFrames = new()
            {
                new BookedTimeFrame()
                {
                    ListingId = listingId,
                    AvailabilityId = availabilityId, 
                    StartDateTime = DateTime.Today.AddDays(2).AddHours(11),
                    EndDateTime = DateTime.Today.AddDays(2).AddHours(12) 
                },
            };

            // reserve a booking
            var reserveBooking = await _schedulingManager.ReserveBooking(userId, listingId, fullPrice, chosenTimeFrames).ConfigureAwait(false);
            int bookingId = reserveBooking.Payload!.BookingId;

            //Act
            var actual = await _schedulingManager.CancelBooking(userId, bookingId).ConfigureAwait(false);
            var doubleCheck = await _bookingService.GetBookingByBookingId(bookingId).ConfigureAwait(false);
            var returnBookingStatus = doubleCheck.Payload.BookingStatusId;

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.IsTrue(returnBookingStatus == BookingStatus.CANCELLED);
        }

        [TestMethod]
        public async Task CancelBooking_AlreadyCancelledBooking_Failed()
        {
            // Arrange
            //  - Setup user and initial state
            var credentialEmail = "test@gmail.com";
            var credentialPassword = "12345678";
            await _registrationManager.Register(credentialEmail, credentialPassword).ConfigureAwait(false);
            var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
            var userId = userIdResult.Payload;

            var accessTokenResult = await _authorizationService.GenerateAccessToken(userId, false).ConfigureAwait(false);
            var idTokenResult = _authenticationService.GenerateIdToken(userId, accessTokenResult.Payload!);
            if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            {
                _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);

            }
            //  - Setup test data
            var testData = await SetUpTestData().ConfigureAwait(false);
            int listingId = (int)testData["ListingId"];
            int availabilityId = (int)((List<ListingAvailability>)testData["ListingAvailabilities"])[0].AvailabilityId;
            float fullPrice = 100;
            List<BookedTimeFrame> chosenTimeFrames = new()
            {
                new BookedTimeFrame(){ListingId = listingId, AvailabilityId = availabilityId, StartDateTime = DateTime.Today.AddDays(2).AddHours(11),EndDateTime = DateTime.Today.AddDays(2).AddHours(12) },
            };

            // reserve a booking
            var reserveBooking = await _schedulingManager.ReserveBooking(userId, listingId, fullPrice, chosenTimeFrames).ConfigureAwait(false);
            int bookingId = reserveBooking.Payload!.BookingId;
            var cancelBooking = await _schedulingManager.CancelBooking(userId, bookingId).ConfigureAwait(false);

            //Act
            var actual = await _schedulingManager.CancelBooking(userId, bookingId).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsSuccessful);
        }
    }
}
