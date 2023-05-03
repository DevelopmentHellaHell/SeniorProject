using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using System.Configuration;

namespace DevelopmentHell.Hubba.Scheduling.Test.DAL
{
    [TestClass]
    public class BookedTimeFramesDataAccessUnitTest
    {
        private readonly IListingsDataAccess _listingDAO;
        private readonly IListingAvailabilitiesDataAccess _listingavailabilityDAO;
        private readonly IBookingsDataAccess _bookingDAO;
        private readonly IBookedTimeFramesDataAccess _bookedtimeframeDAO;
        private readonly ITestingService _testingService;

        private readonly string _listingsConnectionString = ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!;
        private readonly string _listingsTable = ConfigurationManager.AppSettings["ListingsTable"]!;
        private readonly string _listingavailabilitiesTable = ConfigurationManager.AppSettings["ListingAvailabilitiesTable"]!;

        private readonly string _bookingsConnectionString = ConfigurationManager.AppSettings["SchedulingsConnectionString"]!;
        private readonly string _bookingsTable = ConfigurationManager.AppSettings["BookingsTable"]!;
        private readonly string _bookedtimeframesTable = ConfigurationManager.AppSettings["BookedTimeFramesTable"]!;

        private readonly string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;
        public BookedTimeFramesDataAccessUnitTest()
        {
            _listingDAO = new ListingsDataAccess(_listingsConnectionString, _listingsTable);
            _listingavailabilityDAO = new ListingAvailabilitiesDataAccess(_listingsConnectionString, _listingavailabilitiesTable);

            _bookingDAO = new BookingsDataAccess(_bookingsConnectionString, _bookingsTable);
            _bookedtimeframeDAO = new BookedTimeFramesDataAccess(_bookingsConnectionString, _bookedtimeframesTable);

            _testingService = new TestingService(_jwtKey, new TestsDataAccess());
        }
        [TestInitialize]
        [TestCleanup]
        public async Task CleanUp() 
        {
            await _testingService.DeleteTableRecords(Models.Tests.Databases.LISTING_PROFILES, Models.Tests.Tables.BOOKINGS).ConfigureAwait(false);
        }
        /// <summary>
        /// Set up dependencies. 
        /// Create new Listing, new Listing Availabilities, new Booking
        /// </summary>
        /// <returns>Tuple<int,int,List<ListingAvailability>>(listingId, bookingId, listingAvailabilities)</returns>
        private async Task<Tuple<int, int, List<ListingAvailability>>> TestInitialize()
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
                        StartTime = DateTime.Now.AddDays(1),
                        EndTime = DateTime.Now.AddDays(1).AddHours(20)
                    },
                    new Models.DTO.ListingAvailabilityDTO()
                    {
                        ListingId = listingId.Payload,
                        StartTime = DateTime.Today.AddDays(2),
                        EndTime = DateTime.Today.AddDays(2).AddHours(20)
                    }
                };
            Booking booking = new Booking()
            {
                UserId = 1,
                ListingId = listingId.Payload,
                FullPrice = 150,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreationDate = DateTime.Now,
                LastEditUser = 1,
            };

            // create Listing Availability
            var createAvailability = await _listingavailabilityDAO.AddListingAvailabilities(listingAvailabilities).ConfigureAwait(false);
            var getAvailabilities = await _listingavailabilityDAO.GetListingAvailabilities(listingId.Payload).ConfigureAwait(false);


            // create a booking
            var createBooking = await _bookingDAO.CreateBooking(booking).ConfigureAwait(false);

            return new Tuple<int, int, List<ListingAvailability>>(listingId.Payload, createBooking.Payload, getAvailabilities.Payload!);
        }

        [TestMethod]
        public async Task CreateBookedTimeFrames_NonExistedAvailability_Failed()
        {
            // Arrange
            var createListingBookingAvailability = await TestInitialize();
            int listingId = createListingBookingAvailability.Item1;
            int bookingId = createListingBookingAvailability.Item2;
            int nonexistedAvailability = 0;
            List<BookedTimeFrame> testtimeframes =
                new List<BookedTimeFrame>()
                {
                        new BookedTimeFrame()
                        {
                            BookingId = bookingId,
                            ListingId = listingId,
                            AvailabilityId = nonexistedAvailability,
                            StartDateTime = DateTime.Now,
                            EndDateTime = DateTime.Now.AddHours(1)
                        }
                };

            // Actual
            var actual = await _bookedtimeframeDAO.CreateBookedTimeFrames(bookingId, testtimeframes).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsSuccessful);
        }
        [TestMethod]
        public async Task CreateBookedTimeFrames_NonExistedBooking_Failed()
        {
            // Arrange
            var createListingBookingAvailability = await TestInitialize();
            int listingId = createListingBookingAvailability.Item1;
            int availId = (int)createListingBookingAvailability.Item3[0].AvailabilityId!;
            int nonexistedBookingId = 0;
            List<BookedTimeFrame> testtimeframes =
                new List<BookedTimeFrame>()
                {
                        new BookedTimeFrame()
                        {
                            BookingId = nonexistedBookingId,
                            ListingId = listingId,
                            AvailabilityId = availId,
                            StartDateTime = new DateTime(2023, 5,9,12,0,0,0),
                            EndDateTime = new DateTime(2023,5,9,13,0,0,0)
                        }
                };
            // Actual
            var actual = await _bookedtimeframeDAO.CreateBookedTimeFrames(nonexistedBookingId, testtimeframes).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsSuccessful);
        }

        [TestMethod]
        public async Task CreateValidBookedTimeFrames_Successful()
        {
            // Arrange
            var createListingBookingAvailability = await TestInitialize().ConfigureAwait(false);
            var listingId = createListingBookingAvailability.Item1;
            var bookingId = createListingBookingAvailability.Item2;
            var availId = createListingBookingAvailability.Item3[0].AvailabilityId;
            List<BookedTimeFrame> testtimeframes =
                new List<BookedTimeFrame>()
                {
                        new BookedTimeFrame()
                        {
                            BookingId = bookingId,
                            ListingId = listingId,
                            AvailabilityId = (int) availId!,
                            StartDateTime = DateTime.Now.AddDays(1),
                            EndDateTime = DateTime.Now.AddDays(1).AddHours(1)
                        }
                };

            // Actual
            var actualResult = await _bookedtimeframeDAO.CreateBookedTimeFrames(bookingId, testtimeframes).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsTrue(actualResult.IsSuccessful);
        }

        [TestMethod]
        public async Task GetBookedTimeFrame_ByBookingId_ListOfBookedTimeFrame()
        {
            // Arrange
            var createListingBookingAvailability = await TestInitialize().ConfigureAwait(false);
            var listingId = createListingBookingAvailability.Item1;
            var bookingId = createListingBookingAvailability.Item2;
            var availId = createListingBookingAvailability.Item3[0].AvailabilityId;
            List<BookedTimeFrame> testtimeframes =
                new List<BookedTimeFrame>()
                {
                        new BookedTimeFrame()
                        {
                            BookingId =bookingId,
                            ListingId = listingId,
                            AvailabilityId = (int) availId!,
                            StartDateTime = DateTime.Now.AddDays(1),
                            EndDateTime = DateTime.Now.AddDays(1).AddHours(1)
                        }
                };
            var addBookedTimeFrames = await _bookedtimeframeDAO.CreateBookedTimeFrames(bookingId,testtimeframes).ConfigureAwait(false);
            // filter prep to get the inserted BookedTimeFrame
            List<Tuple<string, object>> filter = new List<Tuple<string, object>>() { new Tuple<string, object>(nameof(BookedTimeFrame.BookingId), bookingId) };
            
            //Act
            var actual = await _bookedtimeframeDAO.GetBookedTimeFrames(filter).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(testtimeframes.Count, actual.Payload!.Count);
        }
        [TestMethod]
        public async Task GetBookedTimeFrame_ByListingId_ListOfBookedTimeFrame()
        {
            // Arrange
            var createListingBookingAvailability = await TestInitialize().ConfigureAwait(false);
            var listingId = createListingBookingAvailability.Item1;
            var bookingId = createListingBookingAvailability.Item2;
            var availId = createListingBookingAvailability.Item3[0].AvailabilityId;
            List<BookedTimeFrame> testtimeframes =
                new List<BookedTimeFrame>()
                {
                        new BookedTimeFrame()
                        {
                            BookingId =bookingId,
                            ListingId = listingId,
                            AvailabilityId = (int) availId!,
                            StartDateTime = DateTime.Now.AddDays(1),
                            EndDateTime = DateTime.Now.AddDays(1).AddHours(1)
                        },
                        new BookedTimeFrame()
                        {
                            BookingId =bookingId,
                            ListingId = listingId,
                            AvailabilityId = (int) availId,
                            StartDateTime = DateTime.Now.AddDays(1).AddHours(1),
                            EndDateTime = DateTime.Now.AddDays(1).AddHours(2)
                        }
                };
            var addBookedTimeFrames = await _bookedtimeframeDAO.CreateBookedTimeFrames(bookingId, testtimeframes).ConfigureAwait(false);
            // filter prep to get the inserted BookedTimeFrame
            List<Tuple<string, object>> filter = new List<Tuple<string, object>>() { new Tuple<string, object>(nameof(BookedTimeFrame.ListingId), listingId) };

            //Act
            var actual = await _bookedtimeframeDAO.GetBookedTimeFrames(filter).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(testtimeframes.Count, actual.Payload!.Count);
        }
        [TestMethod]
        public async Task DeleteBookedTimeFrames_ByBookingId_Successful()
        {
            //Arrange
            var createListingBookingAvail = await TestInitialize().ConfigureAwait(false);
            int listingId = createListingBookingAvail.Item1;
            int bookingId = createListingBookingAvail.Item2;
            var availId = createListingBookingAvail.Item3[0].AvailabilityId;
            List<BookedTimeFrame> testtimeframes =
                new List<BookedTimeFrame>()
                {
                        new BookedTimeFrame()
                        {
                            BookingId =bookingId,
                            ListingId = listingId,
                            AvailabilityId = (int) availId!,
                            StartDateTime = DateTime.Now.AddDays(1),
                            EndDateTime = DateTime.Now.AddDays(1).AddHours(1)
                        },
                        new BookedTimeFrame()
                        {
                            BookingId =bookingId,
                            ListingId = listingId,
                            AvailabilityId = (int) availId,
                            StartDateTime = DateTime.Now.AddDays(1).AddHours(1),
                            EndDateTime = DateTime.Now.AddDays(1).AddHours(2)
                        }
                };
            var addBookedTimeFrames = await _bookedtimeframeDAO.CreateBookedTimeFrames(bookingId, testtimeframes).ConfigureAwait(false);
            // filter prep to get the inserted BookedTimeFrame
            List<Tuple<string, object>> filter = new List<Tuple<string, object>>() { new Tuple<string, object>(nameof(BookedTimeFrame.BookingId), bookingId) };

            //Act
            var actual = await _bookedtimeframeDAO.DeleteBookedTimeFrames(filter).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
        }
    }
}
