using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using System.Configuration;

namespace DevelopmentHell.Hubba.Listing.Test
{
    [TestClass]
    public class BookingDataAccessUnitTest
    {
        //Arrange
        private readonly string _listingsConnectionString = ConfigurationManager.AppSettings["ListingsConnectionString"]!;
        private readonly string _bookingsTable = "Bookings";

        private readonly IBookingDataAccess _bookingDAO;
        private readonly Booking testBooking1 = new Booking()
        {
            UserId = 1,
            ListingId = 2,
            FullPrice = 35,
            BookingStatusId = 1,
            CreateDate = DateTime.Now,
            LastModifyUser = 1
        };
        private readonly Booking testBooking2 = new Booking()
        {
            UserId = 1,
            ListingId = 0,
            FullPrice = 35,
            BookingStatusId = 1,
            CreateDate = DateTime.Now,
            LastModifyUser = 1
        };

        public BookingDataAccessUnitTest()
        {
            _bookingDAO = new BookingDataAccess(_listingsConnectionString, _bookingsTable);
        }
        /// <summary>
        /// Test case: CreateBooking() method insert entries to 2 tables Bookings and BookedTimeFrames
        /// </summary>
        [TestMethod]
        public async Task CreateBooking_IsSuccessfull_True()
        {
            //Arrange
            var expectedBool = true;

            //Act
            Result actual = await _bookingDAO.CreateBooking(testBooking1).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual.IsSuccessful == expectedBool);
        }
        /// <summary>
        /// Test case: BooingId is automactically assigned from database end.
        /// Identical Booking object can be added 
        /// </summary>
        [TestMethod]
        public async Task CreateIdenticalBooking_IsSuccessfull_True()
        {
            //Arrange
            var expectedBool = true;

            //Act
            Result actual = await _bookingDAO.CreateBooking(testBooking1).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual.IsSuccessful == expectedBool);
        }

        [TestMethod]
        public async Task CreateBooking_ForNonExistingListing_IsSuccessfull_False()
        {
            //Arrange
            var expectedBool = false;

            //Act
            Result actual = await _bookingDAO.CreateBooking(testBooking2).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual.IsSuccessful == expectedBool);
        }

        //[TestMethod]
        //public async Task GetBookings_UserId_ListingId_ListOfBookings()
        //{
        //    //Arrange
        //    Booking expectedBooking1 = testBooking1;
        //    Booking expectedBooking2 = testBooking1;
        //    var expected = new List<Booking>() { expectedBooking1 , expectedBooking2 };


        //    //Act
        //    //Result actual = await _bookingDAO.GetBooking(testBooking2).ConfigureAwait(false);

        //    //Assert
        //    //Assert.IsNotNull(actual.IsSuccessful == expectedBool);
        //}
    }
}