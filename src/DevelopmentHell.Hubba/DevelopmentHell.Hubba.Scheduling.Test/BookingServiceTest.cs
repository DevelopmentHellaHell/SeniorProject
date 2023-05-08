﻿using System.Configuration;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Scheduling.Service.Abstractions;
using DevelopmentHell.Hubba.Scheduling.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;

namespace DevelopmentHell.Hubba.Scheduling.Test.Service
{
    [TestClass]
    public class BookingServiceTest
    {
        //Arrange
        private readonly IListingsDataAccess _listingDAO;
        private readonly IListingAvailabilitiesDataAccess _listingAvailabilityDAO;
        private readonly IListingHistoryDataAccess _listingHistoryDAO;

        private readonly IBookingsDataAccess _bookingDAO;
        private readonly IBookedTimeFramesDataAccess _bookedTimeFrameDAO;

        private readonly IBookingService _bookingService;
        private readonly ITestingService _testingService;

        private readonly string _listingsConnectionString = ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!;
        private readonly string _listingsTable = ConfigurationManager.AppSettings["ListingsTable"]!;
        private readonly string _listingAvailabilitiesTable = ConfigurationManager.AppSettings["ListingAvailabilitiesTable"]!;
        private readonly string _listingHistoryTable = ConfigurationManager.AppSettings["ListingHistoryTable"]!;

        private readonly string _bookingsConnectionString = ConfigurationManager.AppSettings["SchedulingsConnectionString"]!;
        private readonly string _bookingsTable = ConfigurationManager.AppSettings["BookingsTable"]!;
        private readonly string _bookedTimeFramesTable = ConfigurationManager.AppSettings["BookedTimeFramesTable"]!;

        private readonly string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;

        public BookingServiceTest()
        {
            _listingDAO = new ListingsDataAccess(_listingsConnectionString, _listingsTable);
            _listingAvailabilityDAO = new ListingAvailabilitiesDataAccess(_listingsConnectionString, _listingAvailabilitiesTable);
            _listingHistoryDAO = new ListingHistoryDataAccess(_listingsConnectionString, _listingHistoryTable);

            _bookingDAO = new BookingsDataAccess(_bookingsConnectionString, _bookingsTable);
            _bookedTimeFrameDAO = new BookedTimeFramesDataAccess(_bookingsConnectionString, _bookedTimeFramesTable);

            _bookingService = new BookingService(_bookingDAO, _bookedTimeFrameDAO, _listingHistoryDAO);
            _testingService = new TestingService(_jwtKey, new TestsDataAccess());
        }

        [TestInitialize]
        [TestCleanup]
        public async Task CleanUp()
        {
            await _testingService.DeleteTableRecords(Models.Tests.Databases.LISTING_PROFILES, Models.Tests.Tables.BOOKINGS).ConfigureAwait(false);
        }
        /// <summary>
        /// Set up dependencies, add a new Listing, add new Listing Availability
        /// </summary>
        /// <returns><Tuple<int, List<ListingAvailability>>(listingId, availId)</returns>
        private async Task<Tuple<int, List<ListingAvailability>>> SetUp()
        {
            int ownerId = 200;
            string title = "Booked time frame test";
            // create a listing then get the listingId from inserted row
            var createListing = await _listingDAO.CreateListing(ownerId, title).ConfigureAwait(false);
            Result<int> listingId = await _listingDAO.GetListingId(ownerId, title).ConfigureAwait(false);

            var listingAvailabilities = new List<Models.DTO.ListingAvailabilityDTO>()
                {
                    new Models.DTO.ListingAvailabilityDTO()
                    {
                        ListingId = listingId.Payload,
                        StartTime = DateTime.Today.AddDays(2),
                        EndTime = DateTime.Today.AddDays(2).AddHours(20)
                    },
                    new Models.DTO.ListingAvailabilityDTO()
                    {
                        ListingId = listingId.Payload,
                        StartTime = DateTime.Today.AddDays(3),
                        EndTime = DateTime.Today.AddDays(3).AddHours(20)
                    }
                };


            // create Listing Availability
            var createAvailability = await _listingAvailabilityDAO.AddListingAvailabilities(listingAvailabilities).ConfigureAwait(false);
            var getAvailabilities = await _listingAvailabilityDAO.GetListingAvailabilities(listingId.Payload).ConfigureAwait(false);

            return new Tuple<int, List<ListingAvailability>>(listingId.Payload, getAvailabilities.Payload!);
        }

        /// <summary>
        /// Same user, same listing, different booking, same timeframes
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AddBooking_DupTimeFrames_Failed()
        {
            //Arrange
            var createListingAvail = await SetUp().ConfigureAwait(false);
            int listingId = createListingAvail.Item1;
            List<ListingAvailability> listingAvailabilities = createListingAvail.Item2;

            Booking booking = new Booking()
            {
                UserId = 1,
                ListingId = listingId,
                FullPrice = 150,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreationDate = DateTime.Now,
                LastEditUser = 1,
                TimeFrames = new List<BookedTimeFrame>()
                {
                    new BookedTimeFrame()
                    {
                        ListingId = listingId,
                        AvailabilityId = (int)listingAvailabilities[0].AvailabilityId!,
                        StartDateTime = DateTime.Today.AddDays(2).AddHours(1),
                        EndDateTime = DateTime.Today.AddDays(2).AddHours(2)
                    }
                }
            };
            var addBooking = await _bookingService.AddNewBooking(booking).ConfigureAwait(false);

            //Act
            var actual = await _bookingService.AddNewBooking(booking).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(!actual.IsSuccessful);
        }
        /// <summary>
        /// Same user, same listing availability, diff timeframes
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AddBooking_SameUserListingDifferentTime_Successful()
        {
            //Arrange
            var createListingAvail = await SetUp().ConfigureAwait(false);
            int listingId = createListingAvail.Item1;
            List<ListingAvailability> listingAvailabilities = createListingAvail.Item2;

            Booking booking = new Booking()
            {
                UserId = 1,
                ListingId = listingId,
                FullPrice = 150,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreationDate = DateTime.Now,
                LastEditUser = 1,
                TimeFrames = new List<BookedTimeFrame>()
                {
                    new BookedTimeFrame()
                    {
                        ListingId = listingId,
                        AvailabilityId = (int)listingAvailabilities[0].AvailabilityId!,
                        StartDateTime = DateTime.Today.AddDays(2).AddHours(1),
                        EndDateTime = DateTime.Today.AddDays(2).AddHours(2)
                    }
                }
            };
            var actual = await _bookingService.AddNewBooking(booking).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
        }
        [TestMethod]
        public async Task GetBookingStatusByBookingId_Successful()
        {
            //Arrange
            var createListingAvail = await SetUp().ConfigureAwait(false);
            int listingId = createListingAvail.Item1;
            List<ListingAvailability> listingAvailabilities = createListingAvail.Item2;

            Booking booking = new Booking()
            {
                UserId = 1,
                ListingId = listingId,
                FullPrice = 150,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreationDate = DateTime.Now,
                LastEditUser = 1,
                TimeFrames = new List<BookedTimeFrame>()
                {
                    new BookedTimeFrame()
                    {
                        ListingId = listingId,
                        AvailabilityId = (int)listingAvailabilities[0].AvailabilityId!,
                        StartDateTime = DateTime.Today.AddDays(2).AddHours(1),
                        EndDateTime = DateTime.Today.AddDays(2).AddHours(2)
                    }
                }
            };
            var addBooking = await _bookingService.AddNewBooking(booking).ConfigureAwait(false);
            var actual = await _bookingService.GetBookingStatusByBookingId(addBooking.Payload).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(actual.Payload, BookingStatus.CONFIRMED);
        }
        [TestMethod]
        public async Task GetBookingByBookingId_Successful()
        {
            //Arrange
            var createListingAvail = await SetUp().ConfigureAwait(false);
            int listingId = createListingAvail.Item1;
            List<ListingAvailability> listingAvailabilities = createListingAvail.Item2;

            Booking booking = new Booking()
            {
                UserId = 1,
                ListingId = listingId,
                FullPrice = 150,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreationDate = DateTime.Now,
                LastEditUser = 1,
                TimeFrames = new List<BookedTimeFrame>()
                {
                    new BookedTimeFrame()
                    {
                        ListingId = listingId,
                        AvailabilityId = (int)listingAvailabilities[0].AvailabilityId!,
                        StartDateTime = DateTime.Today.AddDays(2).AddHours(1),
                        EndDateTime = DateTime.Today.AddDays(2).AddHours(2)
                    }
                }
            };
            var addBooking = await _bookingService.AddNewBooking(booking).ConfigureAwait(false);
            var actual = await _bookingService.GetBookingByBookingId(addBooking.Payload).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(actual.Payload!.GetType(), booking.GetType());
        }
        [TestMethod]
        public async Task GetBookedTimeFramesByBookingId_Successful()
        {
            //Arrange
            var createListingAvail = await SetUp().ConfigureAwait(false);
            int listingId = createListingAvail.Item1;
            List<ListingAvailability> listingAvailabilities = createListingAvail.Item2;

            Booking booking = new Booking()
            {
                UserId = 1,
                ListingId = listingId,
                FullPrice = 150,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreationDate = DateTime.Now,
                LastEditUser = 1,
            };
            List<BookedTimeFrame> expected = new()
            {
                new BookedTimeFrame()
                {
                    ListingId = listingId,
                    AvailabilityId = (int)listingAvailabilities[0].AvailabilityId!,
                    StartDateTime = DateTime.Today.AddDays(2).AddHours(1),
                    EndDateTime = DateTime.Today.AddDays(2).AddHours(2)
                }
            };
            booking.TimeFrames = expected;
            var addBooking = await _bookingService.AddNewBooking(booking).ConfigureAwait(false);
            var actual = await _bookingService.GetBookedTimeFramesByBookingId(addBooking.Payload).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(actual.Payload!.GetType(), expected.GetType());
        }
        [TestMethod]
        public async Task CancelBooking_Successful()
        {
            //Arrange
            var createListingAvail = await SetUp().ConfigureAwait(false);
            int listingId = createListingAvail.Item1;
            List<ListingAvailability> listingAvailabilities = createListingAvail.Item2;

            Booking booking = new Booking()
            {
                UserId = 1,
                ListingId = listingId,
                FullPrice = 150,
                BookingStatusId = BookingStatus.CONFIRMED,
                CreationDate = DateTime.Now,
                LastEditUser = 1,
                TimeFrames = new List<BookedTimeFrame>()
                {
                    new BookedTimeFrame()
                    {
                        ListingId = listingId,
                        AvailabilityId = (int)listingAvailabilities[0].AvailabilityId !,
                        StartDateTime = DateTime.Today.AddDays(2).AddHours(1),
                        EndDateTime = DateTime.Today.AddDays(2).AddHours(2)
                    }
                }
            };
            var addBooking = await _bookingService.AddNewBooking(booking).ConfigureAwait(false);
            var actual = await _bookingService.CancelBooking(addBooking.Payload).ConfigureAwait(false);
            var doubleCheck = await _bookingService.GetBookingStatusByBookingId(addBooking.Payload).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.IsTrue(doubleCheck.IsSuccessful);
            Assert.AreEqual(doubleCheck.Payload, BookingStatus.CANCELLED);
        }
    }
}