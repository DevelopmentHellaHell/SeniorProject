using Development.Hubba.JWTHandler.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.Scheduling.Service.Abstractions;
using DevelopmentHell.Hubba.Scheduling.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using System.Configuration;
using System.Runtime.InteropServices;

namespace DevelopmentHell.Hubba.Scheduling.Test.Service
{
    [TestClass]
    public class BookingServiceTest
    {
        //Arrange
        private readonly IBookingService _bookingService;
        private readonly IBookingsDataAccess _bookingDAO;
        private readonly IBookedTimeFramesDataAccess _bookedTimeFrameDAO;
        private readonly ITestingService _testingService;

        private readonly string _bookingsConnectionString = ConfigurationManager.AppSettings["SchedulingsConnectionString"]!;
        private readonly string _bookingsTable = ConfigurationManager.AppSettings["BookingsTable"]!;
        private readonly string _bookedTimeFramesTable = ConfigurationManager.AppSettings["BookedTimeFrameTable"]!;

        private readonly string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;

        private static Booking validBooking1 = new Booking()
        {
            UserId = 1,
            ListingId = 10,
            FullPrice = 35,
            BookingStatusId = BookingStatus.CONFIRMED,
            CreationDate = DateTime.Now,
            LastEditUser = 1,
            TimeFrames = new List<BookedTimeFrame>()
            {
                new BookedTimeFrame()
                {
                    ListingId = 10,
                    AvailabilityId = 9,
                    StartDateTime = new DateTime(2023, 5,5,8,0,0),
                    EndDateTime = new DateTime(2023, 5,5,9,0,0)
                }
            }
        };
        
        public BookingServiceTest()
        {
            _bookingDAO = new BookingsDataAccess(_bookingsConnectionString, _bookingsTable);
            _bookedTimeFrameDAO = new BookedTimeFramesDataAccess(_bookingsConnectionString, _bookedTimeFramesTable);
            _bookingService = new BookingService(_bookingDAO, _bookedTimeFrameDAO);
            _testingService = new TestingService(_jwtKey, new TestsDataAccess());
        }

        [TestInitialize]
        [TestCleanup]
        public async Task CleanUp()
        {
            await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.LISTING_PROFILES).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task AddBooking_Successful()
        {
            //Arrange
            var expected = new Result<int>();

            //Act
            var actual = await _bookingService.AddNewBooking(validBooking1).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(expected.GetType(), actual.GetType());
        }

        [TestMethod]
        public async Task AddBooking_SameUserListingDifferentTime_Successful()
        {
            //Arrange
            var expected = new Result<int>();
            var addBooking1 = await _bookingService.AddNewBooking(validBooking1).ConfigureAwait(false);

            //Act
            var actual = await _bookingService.AddNewBooking(validBooking2).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(expected.GetType(), actual.GetType());
        }
        [TestMethod]
        public async Task AddBooking_DupTimeFrames_Failed()
        {
            //Arrange
            var addBooking1 = await _bookingService.AddNewBooking(validBooking1).ConfigureAwait(false);
            var expected = new Result()
            {
                IsSuccessful = false
            };
            //Act
            var actuall = await _bookingService.AddNewBooking(invalidBooking1).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actuall);
            Assert.IsTrue(!actuall.IsSuccessful);
            Assert.AreEqual(expected.IsSuccessful, actuall.IsSuccessful);
        }
        /// <summary>
        /// Return a List of 1 Booking
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetBooking_ByBookingId_Successful()
        {
            //Arrange
            var addBooking = await _bookingService.AddNewBooking(validBooking2).ConfigureAwait(false);
            var bookingId = ((Result<int>)addBooking).Payload;

            //Act
            var actual = await _bookingService.GetBookingByBookingId(bookingId).ConfigureAwait(false);
            var payload = ((Result<Booking>)actual).Payload;
            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.AreEqual(validBooking2.BookingId, payload.BookingId);
        }
        /// <summary>
        /// Should return BookingStatus in payload
        /// </summary>
        [TestMethod]
        public async Task GetBookingStatus_ByBookingId_Successful()
        {
            //Arrange
            var addBooking = await _bookingService.AddNewBooking(validBooking2).ConfigureAwait(false);
            var bookingId = ((Result<int>)addBooking).Payload;
            var expected = new BookingStatus();

            //Act
            var getBookingStatus = await _bookingService.GetBookingStatusByBookingId(bookingId).ConfigureAwait(false);
            var actual = ((Result<BookingStatus>)getBookingStatus).Payload;

            //Assert
            Assert.IsTrue(getBookingStatus.IsSuccessful);
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.GetType(), actual.GetType());
        }

        /// <summary>
        /// BookingStatusId change from CONFIRMED to CANCELLED
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CancelBooking_Successful()
        {
            //Arrange
            var addBooking = await _bookingService.AddNewBooking(validBooking1).ConfigureAwait(false);
            int bookingId = addBooking.Payload;

            //Act
            var actual = await _bookingService.CancelBooking(bookingId).ConfigureAwait(false);

            var doubleCheck = await _bookingService.GetBookingStatusByBookingId(bookingId).ConfigureAwait(false);


            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.IsTrue(doubleCheck.Payload == BookingStatus.CANCELLED);
        }

    }
}