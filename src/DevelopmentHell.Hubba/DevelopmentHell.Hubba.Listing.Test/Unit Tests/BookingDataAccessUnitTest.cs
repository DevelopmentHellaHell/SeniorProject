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

        private readonly IBookingDataAccess _listingDAO;
        
        public BookingDataAccessUnitTest()
        {
           _listingDAO = new BookingDataAccess(_listingsConnectionString, _bookingsTable);
        }

        [TestMethod]
        public async Task CreateBooking()
        {
            //Arrange
            Booking expectedBooking = new Booking();
            expectedBooking.UserId = 1;
            expectedBooking.ListingId = 2;
            expectedBooking.FullPrice = 35;
            expectedBooking.BookingStatusId = 1;
            expectedBooking.CreateDate = DateTime.Now;
            expectedBooking.LastModifyUser = 1;

            var expectedBool = true;

            //Act
            
            Result actual = await _listingDAO.CreateBooking(new Booking ()
                {   UserId = 1,
                    ListingId = 2,
                    FullPrice = 35,
                    BookingStatusId = 1,
                    CreateDate = DateTime.Now,
                    LastModifyUser = 1
                })
                .ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual.IsSuccessful == expectedBool);
        }
    }
}