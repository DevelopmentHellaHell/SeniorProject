using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using System.Configuration;

namespace DevelopmentHell.Hubba.Scheduling.Test.DAL
{
    [TestClass]
    public class BookingsDataAccessUnitTest
    {
        //Arrange
        private readonly string _listingConnectionString = ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!;
        private readonly string _listingsTable = ConfigurationManager.AppSettings["ListingsTable"]!;
        private readonly string _bookingsConnectionString = ConfigurationManager.AppSettings["SchedulingsConnectionString"]!;
        private readonly string _bookingsTable = ConfigurationManager.AppSettings["BookingsTable"]!;

        private readonly string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;

        private readonly IListingsDataAccess _listingDAO;
        private readonly IBookingsDataAccess _bookingDAO;
        private readonly ITestingService _testingService;
        
        public BookingsDataAccessUnitTest()
        {
            _listingDAO = new ListingsDataAccess(_listingConnectionString, _listingsTable);
            _bookingDAO = new BookingsDataAccess(_bookingsConnectionString, _bookingsTable);
            _testingService = new TestingService(_jwtKey, new TestsDataAccess());
        }
        [TestInitialize]
        [TestCleanup]
        public async Task CleanUp()
        {
            await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.LISTING_PROFILES).ConfigureAwait(false);
        }
        
        private async Task<Result<int>> CreateListing()
        {
            int ownerId = 100;
            string title = "Test Booking DAL - Identical";
            var createListing = await _listingDAO.CreateListing(ownerId, title).ConfigureAwait(false);
            var getListingId = await _listingDAO.GetListingId(ownerId, title).ConfigureAwait(false);
            return getListingId;
        }
        
        [TestMethod]
        public async Task CreateBooking_Successful()
        {
            //Arrange
            var listingId = await CreateListing();

            Booking booking = new Booking()
            {
                UserId = 1,
                ListingId = listingId.Payload,
                FullPrice = 150,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreationDate = DateTime.Now, 
                LastEditUser = 1,
                TimeFrames = new List<BookedTimeFrame>
                {
                    new BookedTimeFrame()
                    {
                        ListingId = listingId.Payload,
                        AvailabilityId = 99,
                        StartDateTime = DateTime.Now,
                        EndDateTime = DateTime.Now.AddMinutes(60)
                    }
                }
            };
            var expected = new Result<int>() { IsSuccessful = true};

            //Act
            var actual = await _bookingDAO.CreateBooking(booking).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual.Payload);
            Assert.IsTrue(actual.GetType() == expected.GetType());
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(expected.Payload.GetType(), actual.Payload.GetType());
        }
        /// <summary>
        /// Test case: BooingId is automactically assigned from database end.
        /// Identical Booking object can be added 
        /// </summary>
        [TestMethod]
        public async Task CreateIdenticalBooking_Successful()
        {
            //Arrange
            var listingId = await CreateListing();

            Booking booking = new Booking()
            {
                UserId = 1,
                ListingId = listingId.Payload,
                FullPrice = 150,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreationDate = DateTime.Now,
                LastEditUser = 1
            };
            var expeted = Result<int>.Success(new int());

            //Act
            var createBooking = await _bookingDAO.CreateBooking(booking).ConfigureAwait(false);
            var actual = await _bookingDAO.CreateBooking(booking).ConfigureAwait(false);
            
            
            //Assert
            Assert.IsNotNull(actual.Payload);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(expeted.Payload.GetType(), actual.Payload.GetType());
            Assert.AreNotEqual(createBooking.Payload, actual.Payload);
        }

        [TestMethod]
        public async Task CreateBooking_ForNonExistingListing_Failed()
        {
            //Arrange
            Booking invalidBooking = new Booking()
            {
                UserId = 1,
                ListingId = 0,
                FullPrice = 150,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreationDate = DateTime.Now,
                LastEditUser = 1
            };

            //Act
            var actual = await _bookingDAO.CreateBooking(invalidBooking).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsSuccessful);
        }

        [TestMethod]
        public async Task DeleteBooking_Successful()
        {
            //Arrange
            var listingId = await CreateListing();
            Booking booking = new Booking()
            {
                UserId = 1,
                ListingId = listingId.Payload,
                FullPrice = 150,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreationDate = DateTime.Now,
                LastEditUser = 1
            };
            var createBooking = await _bookingDAO.CreateBooking(booking).ConfigureAwait(false);
            int bookingId = createBooking.Payload;
            List<Tuple<string,object>> filter = new() { new Tuple<string, object>(nameof(Booking.BookingId), bookingId) };

            //Act
            Result actual = await _bookingDAO.DeleteBooking(filter).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
        }
        [TestMethod]
        public async Task GetBooking_byBookingId_Successful()
        {
            //Arrange
            var listingId = await CreateListing();
            Booking booking = new Booking()
            {
                UserId = 1,
                ListingId = listingId.Payload,
                FullPrice = 150,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreationDate = DateTime.Now,
                LastEditUser = 1
            };
            var createBooking = await _bookingDAO.CreateBooking(booking).ConfigureAwait(false);
            int bookingId = createBooking.Payload;
            var expected = booking;
            expected.BookingId = bookingId;
            List<Tuple<string, object>> filter = new() { new Tuple<string, object>(nameof(Booking.BookingId), bookingId) };

            //Act
            var actual = await _bookingDAO.GetBooking(filter).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Payload);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(expected.BookingId, actual.Payload[0].BookingId);
        }

        [TestMethod]
        public async Task GetBookings_ByUserId_ListingId_ListOfBookings()
        {
            //Arrange
            var listingId = await CreateListing();
            Booking booking = new Booking()
            {
                UserId = 1,
                ListingId = listingId.Payload,
                FullPrice = 150,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreationDate = DateTime.Now,
                LastEditUser = 1
            };
            var expected = new Result<List<Booking>>() { Payload = new List<Booking>() };
            // add 2 new bookings
            for (int i = 0; i < 2; i++)
            {
                expected.Payload.Add(booking);
                var createBooking = await _bookingDAO.CreateBooking(booking).ConfigureAwait(false);
                int bookingId = createBooking.Payload;
                expected.Payload[i].BookingId = bookingId;
            }
            List<Tuple<string, object>> filters = new()
                {
                    new Tuple<string,object> (nameof(Booking.BookingId), booking.BookingId),
                    new Tuple<string,object> (nameof(Booking.ListingId), booking.ListingId)
                };
            //Act
            var actual = await _bookingDAO.GetBooking(filters).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Payload);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(expected.Payload[0].ListingId, actual.Payload[0].ListingId);
        }

        [TestMethod]
        public async Task UpdateBooking_ByBookingId_Successful()
        {
            //Arrange
            var listingId = await CreateListing();
            Booking booking = new Booking()
            {
                UserId = 1,
                ListingId = listingId.Payload,
                FullPrice = 150,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreationDate = DateTime.Now,
                LastEditUser = 1
            };
            var createBooking = await _bookingDAO.CreateBooking(booking).ConfigureAwait (false);
            int bookingId = createBooking.Payload;
            Dictionary<string, object> values = new() 
            {
                {nameof(Booking.BookingStatusId), (int)BookingStatus.CANCELLED}
            };

            List<Comparator> comparators = new()
            {
                new Comparator(nameof(Booking.BookingId),"=", bookingId)
            };
            List<Tuple<string, object>> filter = new() { new Tuple<string, object>(nameof(Booking.BookingId), bookingId) };

            //Act
            var actual = await _bookingDAO.UpdateBooking(values, comparators).ConfigureAwait(false);
            var getBooking = await _bookingDAO.GetBooking(filter).ConfigureAwait(false);
            var expected = getBooking.Payload[0];

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(expected.BookingStatusId, BookingStatus.CANCELLED);
        }
    }
}