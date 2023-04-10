using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Models;
using System.Configuration;

namespace DevelopmentHell.Hubba.Listing.Test
{
    [TestClass]
    public class BookedTimeFrameDataAccessUnitTest
    {
        private readonly IBookedTimeFrameDataAccess _bookedtimeframeDataAccess;

        private readonly string _listingsConnectionString = ConfigurationManager.AppSettings["ListingsConnectionString"]!;
        private readonly string _bookedtimeframesTable = ConfigurationManager.AppSettings["BookedTimeFramesTable"]!;

        public BookedTimeFrameDataAccessUnitTest()
        {
            _bookedtimeframeDataAccess = new BookedTimeFrameDataAccess(_listingsConnectionString, _bookedtimeframesTable);
        }

        [TestMethod]
        public async Task CreateBookedTimeFrames_MultiTimeFrames_IsSuccessful_True()
        {
            // Arrange
            List<BookedTimeFrame> timeframes =
                new List<BookedTimeFrame>()
                {
                    new BookedTimeFrame()
                    {
                        BookingId = 4,
                        ListingId = 2,
                        AvailabilityId = 9,
                        StartDateTime = new DateTime(2023, 5,9,12,0,0),
                        EndDateTime = new DateTime(2023,5,9,13,0,0)
                    }
                    ,new BookedTimeFrame()
                    {
                        BookingId = 4,
                        ListingId = 2,
                        AvailabilityId = 9,
                        StartDateTime = new DateTime(2023, 5,9,13,0,0),
                        EndDateTime = new DateTime(2023,5,9,14,0,0)
                    }
                };
            var expected = true;
            // Actual
            var actualResult = await _bookedtimeframeDataAccess.CreateBookedTimeFrames(timeframes).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(actualResult.IsSuccessful == expected);
        }

    }
}
