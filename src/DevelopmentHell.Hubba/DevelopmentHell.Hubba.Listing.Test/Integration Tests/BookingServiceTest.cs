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
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using System.Configuration;
using System.Runtime.InteropServices;

namespace DevelopmentHell.Hubba.Scheduling.Test.Integration_Tests
{
    [TestClass]
    public class BookingServiceTest
    {
        //Arrange
        private readonly IRegistrationService _registrationService;
        private readonly IBookingService _bookingService;

        private readonly IUserAccountDataAccess _userAccountDAO;
        private readonly IBookingDataAccess _bookingDAO;
        private readonly IBookedTimeFrameDataAccess _bookedTimeFrameDAO;

        private readonly string _userConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private readonly string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private readonly string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private readonly string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        private readonly string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;

        private readonly string _bookingsConnectionString = ConfigurationManager.AppSettings["BookingsConnectionString"]!;
        private readonly string _bookingsTable = "Bookings";
        private readonly string _bookedTimeFramesTable = "BookedTimeFrames";


        private static List<int> newBookingIds = new(); //to cleanup after test
        private static int _userId;
        private static int _listingId = 10;

        private static Booking validBooking1 = new Booking()
        {
            UserId = 1,
            ListingId = 10,
            FullPrice = 35,
            BookingStatusId = BookingStatus.CONFIRMED,
            CreateDate = DateTime.Now,
            LastModifyUser = 1,
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
        //different user, same Listing, same timeframes
        private static Booking invalidBooking1 = new Booking()
        {
            UserId = 2,
            ListingId = 10,
            FullPrice = 35,
            BookingStatusId = BookingStatus.CONFIRMED,
            CreateDate = DateTime.Now,
            LastModifyUser = 2,
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
        //same user, same listing, different timeframes
        private static Booking validBooking2 = new Booking()
        {
            UserId = 1,
            ListingId = 10,
            FullPrice = 35,
            BookingStatusId = BookingStatus.CONFIRMED,
            CreateDate = DateTime.Now,
            LastModifyUser = 1,
            TimeFrames = new List<BookedTimeFrame>()
            {
                new BookedTimeFrame()
                {
                    ListingId = 10,
                    AvailabilityId = 9,
                    StartDateTime = new DateTime(2023, 5,5,9,0,0),
                    EndDateTime = new DateTime(2023, 5,5,10,0,0)
                },
                new BookedTimeFrame()
                {
                    ListingId = 10,
                    AvailabilityId = 9,
                    StartDateTime = new DateTime(2023, 5,5,10,0,0),
                    EndDateTime = new DateTime(2023, 5,5,11,0,0)
                }
            }
        };
        public BookingServiceTest()
        {
            LoggerService loggerService = new LoggerService(
                new LoggerDataAccess(
                    _logsConnectionString,
                    _logsTable
                )
            );
            ICryptographyService cryptographyService = new CryptographyService(
                ConfigurationManager.AppSettings["CryptographyKey"]!
            );
            IValidationService validationService = new ValidationService();
            IJWTHandlerService jwtHandlerService = new JWTHandlerService(
                _jwtKey
            );

            _userAccountDAO = new UserAccountDataAccess(_userConnectionString, _userAccountsTable);
            _bookingDAO = new BookingDataAccess(_bookingsConnectionString, _bookingsTable);
            _bookedTimeFrameDAO = new BookedTimeFrameDataAccess(_bookingsConnectionString, _bookedTimeFramesTable);

            _registrationService = new RegistrationService(
                _userAccountDAO,
                cryptographyService,
                validationService,
                loggerService
            );
            _bookingService = new BookingService
            (
                _bookingDAO,
                _bookedTimeFrameDAO
            );
        }
        [TestInitialize]
        [TestCleanup]
        public async Task CleanUp()
        {
            await _bookingDAO.DeleteBooking
            (
                new List<Tuple<string,object>>() { new Tuple<string,object>("UserId", validBooking1.UserId)}
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// Successful result: 1 row added to Bookings table, 
        /// rows added to BookedTimeFrames table with the same BookingId
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AddBooking_Successful()
        {
            //Arrange
            var expected = true;

            //Act
            var actual = await _bookingService.AddNewBooking(validBooking1).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
        }
        //TODO add another use case of same user, same listing, diff timeframes

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
            Assert.AreEqual(validBooking2.BookingId,payload.BookingId);
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
            var bookingId = ((Result<int>)addBooking).Payload;
            validBooking1.BookingId = bookingId;
            validBooking1.BookingStatusId = BookingStatus.CANCELLED;

            //Act
            var actual = await _bookingService.CancelBooking(validBooking1).ConfigureAwait(false);
            
            var getBooking = await _bookingService.GetBookingStatusByBookingId(bookingId).ConfigureAwait(false);
            var doubleCheck = (Result<BookingStatus>)getBooking;

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful);
            Assert.IsTrue(doubleCheck.Payload == BookingStatus.CANCELLED);
        }
    }
}