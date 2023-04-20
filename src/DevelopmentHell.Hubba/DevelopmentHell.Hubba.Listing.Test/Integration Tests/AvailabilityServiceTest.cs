using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Scheduling.Service.Abstractions;
using DevelopmentHell.Hubba.Scheduling.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DevelopmentHell.Hubba.Scheduling.Test.Service
{
    [TestClass]
    public class AvailabilityServiceTest
    {
        private readonly IAvailabilityService _availabilityService;
        private readonly IBookingService _bookingService;

        private readonly IListingDataAccess _listingDAO;
        private readonly IBookingDataAccess _bookingDAO;
        private readonly IBookedTimeFrameDataAccess _bookedTimeFrameDAO;

        private readonly string _bookingsConnectionString = ConfigurationManager.AppSettings["BookingsConnectionString"]!;
        private readonly string _bookingsTable = ConfigurationManager.AppSettings["BookingsTable"]!;
        private readonly string _bookedTimeFramesTable = ConfigurationManager.AppSettings["BookedTimeFramesTable"]!;
        private readonly string _listingsConnectionString = ConfigurationManager.AppSettings["ListingsConnectionString"]!;
        private readonly string _listingsTable = ConfigurationManager.AppSettings["ListingsTable"]!;

        public AvailabilityServiceTest()
        {
            _listingDAO = new ListingDataAccess(_listingsConnectionString, _listingsTable);
            _bookingDAO = new BookingDataAccess(_bookingsConnectionString, _bookingsTable);
            _bookedTimeFrameDAO = new BookedTimeFrameDataAccess(_bookingsConnectionString, _bookedTimeFramesTable);
            _availabilityService = new AvailabilityService(_listingDAO, _bookedTimeFrameDAO);
            _bookingService = new BookingService(_bookingDAO, _bookedTimeFrameDAO);
        }

        /// <summary>
        /// Add a new listing, a new booking, then check if a list of provided TimeFrames booked
        /// Test input has 2 timeframes, 1 overlapped booked timeframes, 1 valid
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AreTimeFramesBooked_OverlappedTimeFrames_Failed()
        {
            //Arrange
            //Add new listing
            ListingModel listing = new ListingModel()
            {
                OwnerId = 200,
                Title = "Test listing",
                Published = true
            };
            var addListing = await _listingDAO.CreateListing(listing).ConfigureAwait(false);
            //Add new booking
            BookedTimeFrame bookedTimeFrame = new BookedTimeFrame()
            {
                ListingId = addListing.Payload,
                AvailabilityId = 5,
                StartDateTime = new DateTime(2023,9,15,8,0,0),
                EndDateTime = new DateTime(2023,9,15,12,0,0)
            };
            Booking booking = new Booking()
            {
                UserId = 100,
                ListingId = addListing.Payload,
                FullPrice = (float)250.59,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreateDate = DateTime.Now,
                LastModifyUser = 100,
                TimeFrames = new List<BookedTimeFrame>() {  bookedTimeFrame }
            };
            
            var addBooking = await _bookingService.AddNewBooking(booking).ConfigureAwait(false);
            booking.BookingId = addBooking.Payload;
            bookedTimeFrame.BookingId = addBooking.Payload;

            List<BookedTimeFrame> checkedTimeFrames = new() 
            {
                new BookedTimeFrame() //valid
                {
                    ListingId = addListing.Payload,
                    AvailabilityId = 5,
                    StartDateTime = new DateTime(2023, 9, 15, 12, 0, 0),
                    EndDateTime = new DateTime(2023, 9, 15, 13, 0, 0)
                },
                new BookedTimeFrame() //overlapped booked timeframe
                {
                    ListingId = addListing.Payload,
                    AvailabilityId = 5,
                    StartDateTime = new DateTime(2023, 9, 15, 9, 0, 0),
                    EndDateTime = new DateTime(2023, 9, 15, 11, 0, 0)
                }
            };

            var expected = new Result<bool> { IsSuccessful = false, Payload = false};
            //Act

            var actual = await _availabilityService.CheckIfOverlapBookedTimeFrames((int)checkedTimeFrames[0].ListingId, (int)checkedTimeFrames[0].AvailabilityId, checkedTimeFrames);
            
            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(!actual.IsSuccessful);
            Assert.AreEqual(expected.Payload, actual.Payload);
        }
        /// <summary>
        /// Add a new listing, a new booking, then check if a list of provided TimeFrames booked
        /// Test input has 2 valid timeframes
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AreTimeFramesBooked_ValidTimeFrames_Successful()
        {
            //Arrange
            //Add new listing
            ListingModel listing = new ListingModel()
            {
                OwnerId = 200,
                Title = "Test listing",
                Published = true
            };
            var addListing = await _listingDAO.CreateListing(listing).ConfigureAwait(false);
            //Add new booking
            BookedTimeFrame bookedTimeFrame = new BookedTimeFrame()
            {
                ListingId = addListing.Payload,
                AvailabilityId = 5,
                StartDateTime = new DateTime(2023, 9, 15, 8, 0, 0),
                EndDateTime = new DateTime(2023, 9, 15, 12, 0, 0)
            };
            Booking booking = new Booking()
            {
                UserId = 100,
                ListingId = addListing.Payload,
                FullPrice = (float)250.59,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreateDate = DateTime.Now,
                LastModifyUser = 100,
                TimeFrames = new List<BookedTimeFrame>() { bookedTimeFrame }
            };

            var addBooking = await _bookingService.AddNewBooking(booking).ConfigureAwait(false);
            booking.BookingId = addBooking.Payload;
            bookedTimeFrame.BookingId = addBooking.Payload;

            //set up timeframes
            List<BookedTimeFrame> checkedTimeFrames = new()
            {
                new BookedTimeFrame() //valid
                {
                    ListingId = addListing.Payload,
                    AvailabilityId = 5,
                    StartDateTime = new DateTime(2023, 9, 15, 12, 0, 0),
                    EndDateTime = new DateTime(2023, 9, 15, 13, 0, 0)
                },
                new BookedTimeFrame() //valid
                {
                    ListingId = addListing.Payload,
                    AvailabilityId = 5,
                    StartDateTime = new DateTime(2023, 9, 15, 14, 0, 0),
                    EndDateTime = new DateTime(2023, 9, 15, 16, 0, 0)
                },
            };

            var expected = new Result<bool> { IsSuccessful = true, Payload = true };
            //Act

            var actual = await _availabilityService.CheckIfOverlapBookedTimeFrames((int)checkedTimeFrames[0].ListingId, (int)checkedTimeFrames[0].AvailabilityId, checkedTimeFrames);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(expected.Payload, actual.Payload);
        }
    }
}
