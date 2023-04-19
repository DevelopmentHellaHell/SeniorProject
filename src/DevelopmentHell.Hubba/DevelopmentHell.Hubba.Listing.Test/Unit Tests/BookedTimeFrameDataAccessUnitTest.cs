using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Models;
using System.Configuration;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security;

namespace DevelopmentHell.Hubba.Scheduling.Test.Unit_Tests
{
    [TestClass]
    public class BookedTimeFrameDataAccessUnitTest
    {
        private readonly IBookingDataAccess _bookingDAO;
        private readonly IBookedTimeFrameDataAccess _bookedtimeframeDAO;

        private readonly string _bookingsConnectionString = ConfigurationManager.AppSettings["BookingsConnectionString"]!;
        private readonly string _bookingsTable = ConfigurationManager.AppSettings["BookingsTable"]!;
        private readonly string _bookedtimeframesTable = ConfigurationManager.AppSettings["BookedTimeFramesTable"]!;

        private List<BookedTimeFrame> testtimeframes =
                new List<BookedTimeFrame>()
                {
                    new BookedTimeFrame()
                    {
                        ListingId = 3,
                        AvailabilityId = 9,
                        StartDateTime = new DateTime(2023, 5,9,12,0,0,0),
                        EndDateTime = new DateTime(2023,5,9,13,0,0,0)
                    }
                    ,new BookedTimeFrame()
                    {
                        ListingId = 3,
                        AvailabilityId = 9,
                        StartDateTime = new DateTime(2023, 5,9,13,0,0,0),
                        EndDateTime = new DateTime(2023,5,9,14,0,0,0)
                    }
                };
        private List<BookedTimeFrame> othertimeframes =
                new List<BookedTimeFrame>()
                {
                    new BookedTimeFrame()
                    {
                        ListingId = 3,
                        AvailabilityId = 9,
                        StartDateTime = new DateTime(2023, 5,9,14,0,0,0),
                        EndDateTime = new DateTime(2023,5,9,15,0,0,0)
                    }
                    ,new BookedTimeFrame()
                    {
                        ListingId = 3,
                        AvailabilityId = 9,
                        StartDateTime = new DateTime(2023, 5,9,16,0,0,0),
                        EndDateTime = new DateTime(2023,5,9,17,0,0,0)
                    }
                };
        public BookedTimeFrameDataAccessUnitTest()
        {
            _bookingDAO = new BookingDataAccess(_bookingsConnectionString, _bookingsTable);
            _bookedtimeframeDAO = new BookedTimeFrameDataAccess(_bookingsConnectionString, _bookedtimeframesTable);
        }

        [TestMethod]
        public async Task CreateBookedTimeFrames_NonExistedBooking_Failed()
        {
            // Arrange
            var expected = false;
            var nonexistedBookingId = 5;
            // Actual
            var actualResult = await _bookedtimeframeDAO.CreateBookedTimeFrames(nonexistedBookingId,testtimeframes).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsTrue(actualResult.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task GetBookedTimeFrame_ByBookingId_ListOfBookedTimeFrame()
        {
            //Arrange
            var createBooking = await _bookingDAO.CreateBooking
                (
                    new Booking()
                    {
                        UserId = 1,
                        ListingId =3,
                        FullPrice = 35,
                        BookingStatusId = BookingStatus.CONFIRMED,
                        CreateDate = DateTime.Now,
                        LastModifyUser = 1
                    }
                ).ConfigureAwait(false);
            var bookingId = ((Result<int>)createBooking).Payload;

            var createBookedTimeFrames = await _bookedtimeframeDAO.CreateBookedTimeFrames(bookingId,testtimeframes).ConfigureAwait(false);
            List<Tuple<string, object>> filter = new() { new Tuple<string, object>(nameof(BookedTimeFrame.BookingId), bookingId) };
            var expected = testtimeframes;

            //Act
            var actual = await _bookedtimeframeDAO.GetBookedTimeFrames(filter).ConfigureAwait(false);
            
            //Assert
            Assert.IsNotNull (actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(expected.Count, actual.Payload.Count);
        }

        [TestMethod]
        public async Task GetBookedTimeFrame_ByListingId_ListOfBookedTimeFrame()
        {
            //Arrange
            var listingId = othertimeframes[0].ListingId;
            var createBooking = await _bookingDAO.CreateBooking
                (
                    new Booking()
                    {
                        UserId = 1,
                        ListingId = listingId,
                        FullPrice = 35,
                        BookingStatusId = BookingStatus.CONFIRMED,
                        CreateDate = DateTime.Now,
                        LastModifyUser = 1
                    }
                ).ConfigureAwait(false);
            var bookingId = ((Result<int>)createBooking).Payload;

            var createBookedTimeFrames = await _bookedtimeframeDAO.CreateBookedTimeFrames(bookingId, othertimeframes).ConfigureAwait(false);
            
            List<Tuple<string, object>> filter = new() { new Tuple<string, object>(nameof(BookedTimeFrame.ListingId), listingId) };
            var expected = othertimeframes;

            //Act
            var actual = await _bookedtimeframeDAO.GetBookedTimeFrames(filter).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(expected.Count, actual.Payload.Count);
        }

        /// <summary>
        /// Delete 2 test rows just inserted to the table
        /// </summary>
        [TestMethod]
        [TestCleanup]
        public async Task DeleteBookedTimeFrame_ByBookingIdAndListingId_Successful()
        {
            //Arrange
            var expected = true;
            List<Tuple<string, object>> filters = new() 
            { 
                new Tuple<string,object>(nameof(BookedTimeFrame.ListingId),testtimeframes[0].ListingId),
                //new Tuple<string, object>(nameof(BookedTimeFrame.ListingId),othertimeframes[0].ListingId),
                
            };

            //Act
            var actual = await _bookedtimeframeDAO.DeleteBookedTimeFrames(filters).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected);
        }
    }
}
