using Development.Hubba.JWTHandler.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.Scheduling.Service.Abstractions;
using DevelopmentHell.Hubba.Scheduling.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using System.Configuration;
using System.Runtime.InteropServices;

namespace DevelopmentHell.Hubba.Scheduling.Test.Service
{
    [TestClass]
    public class BookingServiceTest
    {
        //Arrange
        private readonly IListingsDataAccess _listingDAO;
        private readonly IListingAvailabilitiesDataAccess _listingavailabilityDAO;
        
        private readonly IBookingsDataAccess _bookingDAO;
        private readonly IBookedTimeFramesDataAccess _bookedTimeFrameDAO;

        private readonly IBookingService _bookingService;
        private readonly ITestingService _testingService;

        private readonly string _listingsConnectionString = ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!;
        private readonly string _listingsTable = ConfigurationManager.AppSettings["ListingsTable"]!;
        private readonly string _listingavailabilitiesTable = ConfigurationManager.AppSettings["ListingAvailabilitiesTable"]!;

        private readonly string _bookingsConnectionString = ConfigurationManager.AppSettings["SchedulingsConnectionString"]!;
        private readonly string _bookingsTable = ConfigurationManager.AppSettings["BookingsTable"]!;
        private readonly string _bookedTimeFramesTable = ConfigurationManager.AppSettings["BookedTimeFramesTable"]!;

        private readonly string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;

        public BookingServiceTest()
        {
            _listingDAO = new ListingsDataAccess(_listingsConnectionString, _listingsTable);
            _listingavailabilityDAO = new ListingAvailabilitiesDataAccess(_listingsConnectionString, _listingavailabilitiesTable);

            _bookingDAO = new BookingsDataAccess(_bookingsConnectionString, _bookingsTable);
            _bookedTimeFrameDAO = new BookedTimeFramesDataAccess(_bookingsConnectionString, _bookedTimeFramesTable);

            _bookingService = new BookingService(_bookingDAO, _bookedTimeFrameDAO);
            _testingService = new TestingService(_jwtKey, new TestsDataAccess());
        }

        [TestInitialize]
        [TestCleanup]
        public async Task CleanUp()
        {
            await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.LISTING_PROFILES).ConfigureAwait(false);
        }
        /// <summary>
        /// Set up dependencies, add a new Listing, add new Listing Availability
        /// </summary>
        /// <returns><Tuple<int, List<ListingAvailability>>(listingId, availId)</returns>
        private async Task<Tuple<int, List<ListingAvailability>>> TestInitialize()
        {
            int ownerId = 200;
            string title = "Booked time frame test";
            // create a listing then get the listingId from inserted row
            var createListing = await _listingDAO.CreateListing(ownerId, title).ConfigureAwait(false);
            Result<int> listingId = await _listingDAO.GetListingId(ownerId, title).ConfigureAwait(false);

            var listingAvailabilities = new List<Models.DTO.ListingAvailabilityDTO>()
                {
                    new Models.DTO.ListingAvailabilityDTO()
                    {
                        ListingId = listingId.Payload,
                        StartTime = DateTime.Today.AddDays(2),
                        EndTime = DateTime.Today.AddDays(2).AddHours(20)
                    },
                    new Models.DTO.ListingAvailabilityDTO()
                    {
                        ListingId = listingId.Payload,
                        StartTime = DateTime.Today.AddDays(3),
                        EndTime = DateTime.Today.AddDays(3).AddHours(20)
                    }
                };
            

            // create Listing Availability
            var createAvailability = await _listingavailabilityDAO.AddListingAvailabilities(listingAvailabilities).ConfigureAwait(false);
            var getAvailabilities = await _listingavailabilityDAO.GetListingAvailabilities(listingId.Payload).ConfigureAwait(false);

            return new Tuple<int, List<ListingAvailability>>(listingId.Payload, getAvailabilities.Payload);
        }

        /// <summary>
        /// Same user, same listing, different booking, same timeframes
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AddBooking_DupTimeFrames_Failed()
        {
            //Arrange
            var createListingAvail = await TestInitialize().ConfigureAwait(false);
            int listingId = createListingAvail.Item1;
            List<ListingAvailability> listingAvailabilities = createListingAvail.Item2;

            Booking booking = new Booking()
            {
                UserId = 1,
                ListingId = listingId,
                FullPrice = 150,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreationDate = DateTime.Now,
                LastEditUser = 1,
                TimeFrames = new List<BookedTimeFrame>()
                {
                    new BookedTimeFrame()
                    {
                        ListingId = listingId,
                        AvailabilityId = (int)listingAvailabilities[0].AvailabilityId,
                        StartDateTime = DateTime.Today.AddDays(2).AddHours(1),
                        EndDateTime = DateTime.Today.AddDays(2).AddHours(2)
                    }
                }
            };
            var addBooking = await _bookingService.AddNewBooking(booking).ConfigureAwait(false);

            //Act
            var actual = await _bookingService.AddNewBooking(booking).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(!actual.IsSuccessful);
        }
        /// <summary>
        /// Same user, same listing availability, diff timeframes
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AddBooking_SameUserListingDifferentTime_Successful()
        {
            //Arrange
            var createListingAvail = await TestInitialize().ConfigureAwait(false);
            int listingId = createListingAvail.Item1;
            List<ListingAvailability> listingAvailabilities = createListingAvail.Item2;

            Booking booking = new Booking()
            {
                UserId = 1,
                ListingId = listingId,
                FullPrice = 150,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreationDate = DateTime.Now,
                LastEditUser = 1,
                TimeFrames = new List<BookedTimeFrame>()
                {
                    new BookedTimeFrame()
                    {
                        ListingId = listingId,
                        AvailabilityId = (int)listingAvailabilities[0].AvailabilityId,
                        StartDateTime = DateTime.Today.AddDays(2).AddHours(1),
                        EndDateTime = DateTime.Today.AddDays(2).AddHours(2)
                    }
                }
            };
            var actual = await _bookingService.AddNewBooking(booking).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
        }
            //    //Arrange
            //    var expected = new Result<int>();
            //    var addBooking1 = await _bookingService.AddNewBooking(validBooking1).ConfigureAwait(false);

            //    //Act
            //    var actual = await _bookingService.AddNewBooking(validBooking2).ConfigureAwait(false);

            //    //Assert
            //    Assert.IsNotNull(actual);
            //    Assert.IsTrue(actual.IsSuccessful);
            //    Assert.AreEqual(expected.GetType(), actual.GetType());
            //}
            //[TestMethod]
            //public async Task AddBooking_Successful()
            //{
            //    //Arrange
            //    var expected = new Result<int>();

            //    //Act
            //    var actual = await _bookingService.AddNewBooking(validBooking1).ConfigureAwait(false);

            //    //Assert
            //    Assert.IsNotNull(actual);
            //    Assert.IsTrue(actual.IsSuccessful);
            //    Assert.AreEqual(expected.GetType(), actual.GetType());
            //}
            ///// <summary>
            ///// Return a List of 1 Booking
            ///// </summary>
            ///// <returns></returns>
            //[TestMethod]
            //public async Task GetBooking_ByBookingId_Successful()
            //{
            //    //Arrange
            //    var addBooking = await _bookingService.AddNewBooking(validBooking2).ConfigureAwait(false);
            //    var bookingId = ((Result<int>)addBooking).Payload;

            //    //Act
            //    var actual = await _bookingService.GetBookingByBookingId(bookingId).ConfigureAwait(false);
            //    var payload = ((Result<Booking>)actual).Payload;
            //    //Assert
            //    Assert.IsNotNull(actual);
            //    Assert.IsTrue(actual.IsSuccessful);
            //    Assert.AreEqual(validBooking2.BookingId, payload.BookingId);
            //}
            ///// <summary>
            ///// Should return BookingStatus in payload
            ///// </summary>
            //[TestMethod]
            //public async Task GetBookingStatus_ByBookingId_Successful()
            //{
            //    //Arrange
            //    var addBooking = await _bookingService.AddNewBooking(validBooking2).ConfigureAwait(false);
            //    var bookingId = ((Result<int>)addBooking).Payload;
            //    var expected = new BookingStatus();

            //    //Act
            //    var getBookingStatus = await _bookingService.GetBookingStatusByBookingId(bookingId).ConfigureAwait(false);
            //    var actual = ((Result<BookingStatus>)getBookingStatus).Payload;

            //    //Assert
            //    Assert.IsTrue(getBookingStatus.IsSuccessful);
            //    Assert.IsNotNull(actual);
            //    Assert.AreEqual(expected.GetType(), actual.GetType());
            //}

            ///// <summary>
            ///// BookingStatusId change from CONFIRMED to CANCELLED
            ///// </summary>
            ///// <returns></returns>
            //[TestMethod]
            //public async Task CancelBooking_Successful()
            //{
            //    //Arrange
            //    var addBooking = await _bookingService.AddNewBooking(validBooking1).ConfigureAwait(false);
            //    int bookingId = addBooking.Payload;

            //    //Act
            //    var actual = await _bookingService.CancelBooking(bookingId).ConfigureAwait(false);

            //    var doubleCheck = await _bookingService.GetBookingStatusByBookingId(bookingId).ConfigureAwait(false);


            //    //Assert
            //    Assert.IsNotNull(actual);
            //    Assert.IsTrue(actual.IsSuccessful);
            //    Assert.IsTrue(doubleCheck.Payload == BookingStatus.CANCELLED);
            //}

        }
}