using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using System.Configuration;

namespace DevelopmentHell.Hubba.Scheduling.Test
{
    [TestClass]
    public class BookingDataAccessUnitTest
    {
        //Arrange
        private readonly string _bookingsConnectionString = ConfigurationManager.AppSettings["BookingsConnectionString"]!;
        private readonly string _bookingsTable = "Bookings";

        private readonly IBookingDataAccess _bookingDAO;
        private readonly Booking validBooking1 = new Booking()
        {
            UserId = 1,
            ListingId = 10,
            FullPrice = 35,
            BookingStatusId = 1,
            CreateDate = DateTime.Now,
            LastModifyUser = 1
        };
        private readonly Booking invalidBooking1 = new Booking()
        {
            UserId = 1,
            ListingId = 0,
            FullPrice = 35,
            BookingStatusId = 1,
            CreateDate = DateTime.Now,
            LastModifyUser = 1
        };
        private readonly Booking invalidBooking2 = new Booking()
        {
            UserId = 1,
            ListingId = 10,
            FullPrice = 35,
            BookingStatusId = 4,
            CreateDate = DateTime.Now,
            LastModifyUser = 1
        };
        private static List<int> newBookingIds = new();
        public BookingDataAccessUnitTest()
        {
            _bookingDAO = new BookingDataAccess(_bookingsConnectionString, _bookingsTable);
        }
        
        [TestMethod]
        public async Task CreateBooking_Successful()
        {
            //Arrange
            var expectedBool = true;

            //Act
            Result createBooking = await _bookingDAO.CreateBooking(validBooking1).ConfigureAwait(false);
            var actual = (Result<int>)createBooking;
            newBookingIds.Add(actual.Payload);

            //Assert
            Assert.IsNotNull(actual.Payload);
            Assert.IsTrue(actual.IsSuccessful == expectedBool);
        }
        /// <summary>
        /// Test case: BooingId is automactically assigned from database end.
        /// Identical Booking object can be added 
        /// </summary>
        [TestMethod]
        public async Task CreateIdenticalBooking_Successful()
        {
            //Arrange
            var expectedBool = true;

            //Act
            Result createBooking = await _bookingDAO.CreateBooking(validBooking1).ConfigureAwait(false);
            var actual = (Result<int>)createBooking;
            
            //Assert
            Assert.IsNotNull(actual.Payload);
            Assert.IsTrue(actual.IsSuccessful == expectedBool);
        }

        [TestMethod]
        public async Task CreateBooking_ForNonExistingListing_Failed()
        {
            //Arrange
            var expectedBool = false;

            //Act
            Result actual = await _bookingDAO.CreateBooking(invalidBooking1).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expectedBool);
        }

        [TestMethod]
        public async Task DeleteBooking_Successful()
        {
            //Arrange
            var createBooking = await _bookingDAO.CreateBooking(validBooking1).ConfigureAwait(false);
            Result<int> bookingId = (Result<int>)createBooking;

            //Act
            Result actual = await _bookingDAO.DeleteBooking(bookingId.Payload).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
        }
        [TestMethod]
        public async Task GetBooking_byBookingId_Successful()
        {
            //Arrange
            var createBooking = await _bookingDAO.CreateBooking(validBooking1).ConfigureAwait(false);
            Result<int> bookingId = (Result<int>)createBooking;
            newBookingIds.Add(bookingId.Payload);

            var expected = validBooking1;
            expected.BookingId = bookingId.Payload;

            //Act
            var getBooking = await _bookingDAO.GetBooking( new Dictionary<string, int>() { {"BookingId",bookingId.Payload } }).ConfigureAwait(false);
            var actual = (Result<List<Booking>>)getBooking;

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Payload);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(expected.BookingId, actual.Payload[0].BookingId);
        }

        [TestMethod]
        [TestCleanup]
        public async Task DeleteTestCases()
        {
            foreach (var bookingId in newBookingIds)
            {
                await _bookingDAO.DeleteBooking(bookingId).ConfigureAwait(false);
            }
        }
        [TestMethod]
        public async Task GetBookings_ByUserId_ListingId_ListOfBookings()
        {
            //Arrange
            var expected = new Result<List<Booking>>() { Payload = new List<Booking>()};

            for (int i = 0; i < 2; i++)
            {
                var createBooking = await _bookingDAO.CreateBooking(validBooking1).ConfigureAwait(false);
                Result<int> bookingId = (Result<int>)createBooking;
                newBookingIds.Add(bookingId.Payload);
                var myBooking = validBooking1;
                myBooking.BookingId = bookingId.Payload;

                expected.Payload.Add(myBooking);
            }

            //Act
            var getBooking = await _bookingDAO.GetBooking(new Dictionary<string, int>() 
            { 
                { "ListingId", (int)validBooking1.ListingId },
                {"UserId", (int)validBooking1.UserId },
            }).ConfigureAwait(false);
            var actual = (Result<List<Booking>>)getBooking;

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Payload);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(expected.Payload[0].ListingId, actual.Payload[0].ListingId);
        }
    }
}