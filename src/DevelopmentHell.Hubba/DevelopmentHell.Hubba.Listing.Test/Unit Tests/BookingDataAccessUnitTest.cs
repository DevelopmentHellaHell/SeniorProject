using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using System.Configuration;

namespace DevelopmentHell.Hubba.Scheduling.Test.Unit_Tests
{
    [TestClass]
    public class BookingDataAccessUnitTest
    {
        //Arrange
        private readonly string _bookingsConnectionString = ConfigurationManager.AppSettings["BookingsConnectionString"]!;
        private readonly string _bookingsTable = ConfigurationManager.AppSettings["BookingsTable"]!;

        private readonly IBookingDataAccess _bookingDAO;
        private readonly Booking validBooking1 = new Booking()
        {
            UserId = 1,
            ListingId = 10,
            FullPrice = 35,
            BookingStatusId = BookingStatus.CONFIRMED,
            CreateDate = DateTime.Now,
            LastModifyUser = 1
        };
        private readonly Booking invalidBooking1 = new Booking()
        {
            UserId = 1,
            ListingId = 0,
            FullPrice = 35,
            BookingStatusId = BookingStatus.CONFIRMED,
            CreateDate = DateTime.Now,
            LastModifyUser = 1
        };
        private readonly Booking validBooking2 = new Booking()
        {
            UserId = 1,
            ListingId = 10,
            FullPrice = 35,
            BookingStatusId = BookingStatus.CANCELLED,
            CreateDate = DateTime.Now,
            LastModifyUser = 1
        };
        public BookingDataAccessUnitTest()
        {
            _bookingDAO = new BookingDataAccess(_bookingsConnectionString, _bookingsTable);
        }
        
        [TestMethod]
        public async Task CreateBooking_Successful()
        {
            //Arrange
            var expected = new Result<int>() { IsSuccessful = true};

            //Act
            var actual = await _bookingDAO.CreateBooking(validBooking1).ConfigureAwait(false);

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
            var expeted = new Result<int> { IsSuccessful = true };

            //Act
            var actual = await _bookingDAO.CreateBooking(validBooking1).ConfigureAwait(false);
            
            
            //Assert
            Assert.IsNotNull(actual.Payload);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(expeted.Payload.GetType(), actual.Payload.GetType());
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
            int bookingId = createBooking.Payload;
            List<Tuple<string,object>> filter = new() { new Tuple<string, object>("BookingId", bookingId) };

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
            var createBooking = await _bookingDAO.CreateBooking(validBooking1).ConfigureAwait(false);
            int bookingId = createBooking.Payload;
            List<Tuple<string, object>> filter = new()
            {
                new Tuple<string,object>("BookingId",bookingId)
            };
            var expected = validBooking1;
            expected.BookingId = bookingId;

            //Act
            var actual = await _bookingDAO.GetBooking( filter).ConfigureAwait(false);

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
            List<Tuple<string, object>> filter = new() { new Tuple<string, object>("UserId", validBooking1.UserId) };
            await _bookingDAO.DeleteBooking(filter).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetBookings_ByUserId_ListingId_ListOfBookings()
        {
            //Arrange
            var expected = new Result<List<Booking>>() { Payload = new List<Booking>()};
            var myBooking = validBooking1;
            // add 2 new bookings
            for (int i = 0; i < 2; i++)
            {
                var createBooking = await _bookingDAO.CreateBooking(validBooking1).ConfigureAwait(false);
                Result<int> bookingId = (Result<int>)createBooking;
                myBooking.BookingId = bookingId.Payload;
                expected.Payload.Add(myBooking);
            }
            List<Tuple<string, object>> filters = new()
                {
                    new Tuple<string,object> ("BookingId", myBooking.BookingId),
                    new Tuple<string,object> ("ListingId", myBooking.ListingId)
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
            var createBooking = await _bookingDAO.CreateBooking(validBooking2).ConfigureAwait (false);
            int bookingId = createBooking.Payload;
            
            var expected = validBooking1;
            expected.BookingId = bookingId;
            expected.BookingStatusId = BookingStatus.CANCELLED;
            List<Tuple<string, object>> filter = new()
                {
                    new Tuple<string,object> ("BookingId", expected.BookingId),
                };
            //Act

            var actual = await _bookingDAO.UpdateBooking(validBooking1).ConfigureAwait(false);
            var getBooking = await _bookingDAO.GetBooking(filter).ConfigureAwait(false);
            var updateResult = (Result<List<Booking>>)getBooking;
            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(expected.BookingStatusId, updateResult.Payload[0].BookingStatusId);
        }
    }
}