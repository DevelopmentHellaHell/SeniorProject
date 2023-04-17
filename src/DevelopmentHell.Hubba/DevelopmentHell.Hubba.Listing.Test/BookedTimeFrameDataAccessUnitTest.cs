using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Models;
using System.Configuration;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevelopmentHell.Hubba.Scheduling.Test
{
    [TestClass]
    public class BookedTimeFrameDataAccessUnitTest
    {
        private readonly IBookedTimeFrameDataAccess _bookedtimeframeDataAccess;

        private readonly string _bookingsConnectionString = ConfigurationManager.AppSettings["BookingsConnectionString"]!;
        private readonly string _bookedtimeframesTable = ConfigurationManager.AppSettings["BookedTimeFramesTable"]!;

        private List<BookedTimeFrame> timeframes =
                new List<BookedTimeFrame>()
                {
                    new BookedTimeFrame()
                    {
                        BookingId = 5,
                        ListingId = 3,
                        AvailabilityId = 9,
                        StartDateTime = new DateTime(2023, 5,9,12,0,0,0),
                        EndDateTime = new DateTime(2023,5,9,13,0,0,0)
                    }
                    ,new BookedTimeFrame()
                    {
                        BookingId = 5,
                        ListingId = 3,
                        AvailabilityId = 9,
                        StartDateTime = new DateTime(2023, 5,9,13,0,0,0),
                        EndDateTime = new DateTime(2023,5,9,14,0,0,0)
                    }
                };
        public BookedTimeFrameDataAccessUnitTest()
        {
            _bookedtimeframeDataAccess = new BookedTimeFrameDataAccess(_bookingsConnectionString, _bookedtimeframesTable);
        }

        [TestMethod]
        public async Task CreateBookedTimeFrames_MultiTimeFrames_Successful()
        {
            // Arrange
            var expected = true;
            // Actual
            var actualResult = await _bookedtimeframeDataAccess.CreateBookedTimeFrames(timeframes).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsTrue(actualResult.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task GetBookedTimeFrame_ByBookingId_ListOfBookedTimeFrame()
        {
            //Arrange
            var createBookedTimeFrames = await _bookedtimeframeDataAccess.CreateBookedTimeFrames(timeframes).ConfigureAwait(false);

            var expected = timeframes;

            //Act
            var result = await _bookedtimeframeDataAccess.GetBookedTimeFrames("BookingId", (int) timeframes[0].BookingId).ConfigureAwait(false);
            var actual = (Result<List<BookedTimeFrame>>) result;
            //Assert
            Assert.IsNotNull (actual);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(expected.Count, actual.Payload.Count);
        }

        [TestMethod]
        public async Task GetBookedTimeFrame_ByListingId_ListOfBookedTimeFrame()
        {
            //Arrange
            var createBookedTimeFrames = await _bookedtimeframeDataAccess.CreateBookedTimeFrames(timeframes).ConfigureAwait(false);

            var expected = timeframes;

            //Act
            var result = await _bookedtimeframeDataAccess.GetBookedTimeFrames("ListingId", (int)timeframes[0].ListingId).ConfigureAwait(false);
            var actual = (Result<List<BookedTimeFrame>>)result;
            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(expected.Count, actual.Payload.Count);
        }

        /// <summary>
        /// Delete 2 test rows just inserted to the table
        /// </summary>
        [TestMethod]
        [TestInitialize]
        [TestCleanup]
        public async Task DeleteBookedTimeFrame_ByBookingIdAndListingId_Successful()
        {
            //Arrange
            var expected = true;
            int bookingId = (int)timeframes[0].BookingId;
            int listingId = (int)timeframes[0].ListingId;
            
            //Act
            var actual = await _bookedtimeframeDataAccess.DeleteBookedTimeFrames(bookingId, listingId).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
        }
    }
}
