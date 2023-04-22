using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.Scheduling.Service.Abstractions;
using DevelopmentHell.Hubba.Scheduling.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Scheduling.Test.Service
{
    [TestClass]
    public class AvailabilityServiceTest
    {
        private readonly IListingsDataAccess _listingDAO;
        private readonly IListingAvailabilitiesDataAccess _listingavailabilityDAO;

        private readonly IBookingsDataAccess _bookingDAO;
        private readonly IBookedTimeFramesDataAccess _bookedTimeFrameDAO;

        private readonly IAvailabilityService _availabilityService;
        private readonly ITestingService _testingService;

        private readonly string _listingsConnectionString = ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!;
        private readonly string _listingsTable = ConfigurationManager.AppSettings["ListingsTable"]!;
        private readonly string _listingavailabilitiesTable = ConfigurationManager.AppSettings["ListingAvailabilitiesTable"]!;

        private readonly string _bookingsConnectionString = ConfigurationManager.AppSettings["SchedulingsConnectionString"]!;
        private readonly string _bookingsTable = ConfigurationManager.AppSettings["BookingsTable"]!;
        private readonly string _bookedTimeFramesTable = ConfigurationManager.AppSettings["BookedTimeFramesTable"]!;

        private readonly string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;

        public AvailabilityServiceTest()
        {
            _listingDAO = new ListingsDataAccess(_listingsConnectionString, _listingsTable);
            _listingavailabilityDAO = new ListingAvailabilitiesDataAccess(_listingsConnectionString, _listingavailabilitiesTable);

            _bookingDAO = new BookingsDataAccess(_bookingsConnectionString, _bookingsTable);
            _bookedTimeFrameDAO = new BookedTimeFramesDataAccess(_bookingsConnectionString, _bookedTimeFramesTable);

            _availabilityService = new AvailabilityService(_listingDAO, _listingavailabilityDAO, _bookedTimeFrameDAO);
            _testingService = new TestingService(_jwtKey, new TestsDataAccess());
        }
        [TestInitialize]
        [TestCleanup]
        public async Task CleanUp()
        {
            await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.LISTING_PROFILES).ConfigureAwait(false);
        }

        private async Task<Dictionary<string, object>> SetUp()
        {
            Dictionary<string, object> result = new();
            // add listing
            int ownerId = 100;
            string title = "Get open time slots test";
            Result addListing = await _listingDAO.CreateListing(ownerId, title).ConfigureAwait(false);
            Result<int> getlistingId = await _listingDAO.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = getlistingId.Payload;

            result[nameof(Listing.OwnerId)] = ownerId;
            result[nameof(Listing.Title)] = title;
            result[nameof(Listing.ListingId)] = listingId;

            // add listing availability
            ListingAvailabilityDTO avail1 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = DateTime.Now.Date.AddHours(8),
                EndTime = DateTime.Now.Date.AddHours(16)
            };
            ListingAvailabilityDTO avail2 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                StartTime = DateTime.Now.Date.AddDays(1).AddHours(8),
                EndTime = DateTime.Now.Date.AddDays(1).AddHours(16)
            };
            Result addAvailability = await _listingavailabilityDAO.AddListingAvailabilities(
                new List<ListingAvailabilityDTO>() { avail1, avail2 }
                ).ConfigureAwait(false);
            Result<List<ListingAvailability>> getAvailabilityId = await _listingavailabilityDAO.GetListingAvailabilities(listingId).ConfigureAwait(false);
            int availabilityId_1 = (int)getAvailabilityId.Payload[0].AvailabilityId;
            int availabilityId_2 = (int)getAvailabilityId.Payload[1].AvailabilityId;

            result["availabilityId_1"] = availabilityId_1;
            result["availabilityId_2"] = availabilityId_2;
            // add Booking
            Booking booking = new Booking()
            {
                UserId = 1,
                ListingId = listingId,
                FullPrice = 150,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreationDate = DateTime.Now,
                LastEditUser = 1,
            };
            Result<int> addBooking = await _bookingDAO.CreateBooking(booking).ConfigureAwait(false);
            int bookingId = addBooking.Payload;
            result[nameof(Booking.BookingId)] = bookingId;

            // add BookedTimeFrame
            List<BookedTimeFrame> timeFrame = new List<BookedTimeFrame>()
            {
                new BookedTimeFrame()
                {
                    BookingId = bookingId,
                    ListingId = listingId,
                    AvailabilityId = availabilityId_1,
                    StartDateTime = DateTime.Now.Date.AddHours(8),
                    EndDateTime = DateTime.Now.Date.AddHours(11)
                },
                new BookedTimeFrame()
                {
                    BookingId = bookingId,
                    ListingId = listingId,
                    AvailabilityId = availabilityId_1,
                    StartDateTime = DateTime.Now.Date.AddHours(13),
                    EndDateTime = DateTime.Now.Date.AddHours(15)
                }
            };
            Result<bool> addBookedTimeFrames = await _bookedTimeFrameDAO.CreateBookedTimeFrames(bookingId, timeFrame).ConfigureAwait(false);

            int month = avail1.StartTime.Month;
            int year = avail1.StartTime.Year;

            result["Month"] = month;
            result["Year"] = year;

            return result;
        }

        [TestMethod]
        public async Task GetOpenTimeSlotsByMonth_Successful()
        {
            //Arrange
            var testData = await SetUp();

            List<Tuple<DateTime, DateTime>> expected = new List<Tuple<DateTime, DateTime>>()
            {
                new Tuple<DateTime, DateTime>(DateTime.Now.Date.AddHours(11), DateTime.Now.Date.AddHours(13)),
                new Tuple<DateTime, DateTime>(DateTime.Now.Date.AddHours(15), DateTime.Now.Date.AddHours(16)),
                new Tuple<DateTime, DateTime>(DateTime.Now.Date.AddDays(1).AddHours(8), DateTime.Now.Date.AddDays(1).AddHours(16)),
            };
            //Act
            Result<List<Tuple<DateTime, DateTime>>> actual = await _availabilityService.GetOpenTimeSlotsByMonth(
                (int)testData[nameof(Listing.ListingId)],
                (int)testData["Month"],
                (int)testData["Year"]
                ).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(actual.Payload.Count, expected.Count);
            Assert.IsTrue(actual.Payload.SequenceEqual(expected));
        }
        [TestMethod]
        public async Task GetOpenTimeSlotsByMonth_InvalidMonth_Failed()
        {
            //Arrange
            var testData = await SetUp();
            int month = 5;
            //Act
            Result<List<Tuple<DateTime, DateTime>>> actual = await _availabilityService.GetOpenTimeSlotsByMonth(
                (int)testData[nameof(Listing.ListingId)],
                month,
                (int)testData["Year"]
                ).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(!actual.IsSuccessful);
        }
        [TestMethod]
        public async Task GetOpenTimeSlots_AfterAddingMoreBookedTimeFrames_Successful()
        {
            //Arrange
            var testData = await SetUp();
            Result<List<Tuple<DateTime, DateTime>>> getOpenTimeSlots = await _availabilityService.GetOpenTimeSlotsByMonth(
                (int)testData[nameof(Listing.ListingId)],
                (int)testData["Month"],
                (int)testData["Year"]
                ).ConfigureAwait(false);
            int beforeCount = getOpenTimeSlots.Payload.Count;

            // add more booking
            Booking anotherBooking = new Booking()
            {
                UserId = 2,
                ListingId = (int)testData[nameof(Listing.ListingId)],
                FullPrice = 150,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreationDate = DateTime.Now,
                LastEditUser = 1,
            };
            var addNewBooking = await _bookingDAO.CreateBooking(anotherBooking).ConfigureAwait(false);

            //add more booked time frames
            List<BookedTimeFrame> timeFrame = new List<BookedTimeFrame>()
            {
                new BookedTimeFrame()
                {
                    BookingId = (int) addNewBooking.Payload,
                    ListingId = (int) testData[nameof(Listing.ListingId)],
                    AvailabilityId = (int) testData["availabilityId_2"],
                    StartDateTime = DateTime.Now.Date.AddDays(1).AddHours(11),
                    EndDateTime = DateTime.Now.Date.AddDays(1).AddHours(12)
                },
                new BookedTimeFrame()
                {
                    BookingId = (int) addNewBooking.Payload,
                    ListingId = (int) testData[nameof(Listing.ListingId)],
                    AvailabilityId = (int) testData["availabilityId_1"],
                    StartDateTime = DateTime.Now.Date.AddHours(11),
                    EndDateTime = DateTime.Now.Date.AddHours(12)
                }
            };
            var addBookedTimeFrames = await _bookedTimeFrameDAO.CreateBookedTimeFrames(addNewBooking.Payload, timeFrame).ConfigureAwait(false);
            // expected
            List<Tuple<DateTime, DateTime>> expected = new List<Tuple<DateTime, DateTime>>()
            {
                new Tuple<DateTime, DateTime>(DateTime.Now.Date.AddHours(12), DateTime.Now.Date.AddHours(13)),
                new Tuple<DateTime, DateTime>(DateTime.Now.Date.AddHours(15), DateTime.Now.Date.AddHours(16)),
                new Tuple<DateTime, DateTime>(DateTime.Now.Date.AddDays(1).AddHours(8), DateTime.Now.Date.AddDays(1).AddHours(11)),
                new Tuple<DateTime, DateTime>(DateTime.Now.Date.AddDays(1).AddHours(12), DateTime.Now.Date.AddDays(1).AddHours(16)),
            };
            //Act
            Result<List<Tuple<DateTime, DateTime>>> actual = await _availabilityService.GetOpenTimeSlotsByMonth(
                (int)testData[nameof(Listing.ListingId)],
                (int)testData["Month"],
                (int)testData["Year"]
                ).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(actual.Payload.Count, expected.Count);
            Assert.IsTrue(actual.Payload.SequenceEqual(expected));
        }
        [TestMethod]
        public async Task ValidateChosenTimeFrames_OverlappedBookedTimeFrames_Failed()
        {
            //Arrange
            var testData = await SetUp().ConfigureAwait(false);
            int listingId = (int)testData[nameof(Listing.ListingId)];
            int availabilityId = (int)testData["availabilityId_1"];

            List<BookedTimeFrame> invalidTimeFrames = new List<BookedTimeFrame>()
            {
                new BookedTimeFrame()
                {
                    ListingId = listingId,
                    AvailabilityId = availabilityId,
                    StartDateTime = DateTime.Now.Date.AddHours(9),
                    EndDateTime = DateTime.Now.Date.AddHours(10)
                },
                new BookedTimeFrame()
                {
                    ListingId = listingId,
                    AvailabilityId = availabilityId,
                    StartDateTime = DateTime.Now.Date.AddHours(14),
                    EndDateTime = DateTime.Now.Date.AddHours(15)
                },
            };

            //Act
            Result<bool> actual = new();
            foreach(var timeFrame in invalidTimeFrames)
            {
                actual = await _availabilityService.ValidateChosenTimeFrames(listingId, availabilityId, timeFrame).ConfigureAwait(false);
            }
            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsSuccessful);
        }
        [TestMethod]
        public async Task ValidateChosenTimeFrames_Successful()
        {
            //Arrange
            var testData = await SetUp().ConfigureAwait(false);
            int listingId = (int)testData[nameof(Listing.ListingId)];
            int availabilityId = (int)testData["availabilityId_1"];

            List<BookedTimeFrame> validTimeFrames = new List<BookedTimeFrame>()
            {
                new BookedTimeFrame()
                {
                    ListingId = listingId,
                    AvailabilityId = availabilityId,
                    StartDateTime = DateTime.Now.Date.AddHours(12),
                    EndDateTime = DateTime.Now.Date.AddHours(13)
                },
                new BookedTimeFrame()
                {
                    ListingId = listingId,
                    AvailabilityId = availabilityId,
                    StartDateTime = DateTime.Now.Date.AddHours(15),
                    EndDateTime = DateTime.Now.Date.AddHours(16)
                },
            };

            //Act
            Result<bool> actual = new();
            foreach (var timeFrame in validTimeFrames)
            {
                actual = await _availabilityService.ValidateChosenTimeFrames(listingId, availabilityId, timeFrame).ConfigureAwait(false);
            }

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
        }
        [TestMethod]
        public async Task GetOwnerId_Successful()
        {
            //Arrange
            var testData = await SetUp().ConfigureAwait(false);
            int listingId = (int)testData[nameof(Listing.ListingId)];
            int ownerId = (int)testData[nameof(Listing.OwnerId)];
            //Act
            var actual = await _availabilityService.GetOwnerId(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(actual.Payload, ownerId);
        }
    }
}
