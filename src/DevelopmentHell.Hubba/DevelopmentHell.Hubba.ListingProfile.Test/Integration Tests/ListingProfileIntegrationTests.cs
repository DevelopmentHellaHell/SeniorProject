using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.ListingProfile.Service.Abstractions;
using DevelopmentHell.Hubba.ListingProfile.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Notification.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Manager.Abstractions;
using DevelopmentHell.Hubba.Registration.Manager.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using DevelopmentHell.ListingProfile.Manager.Abstractions;
using DevelopmentHell.Hubba.ListingProfile.Manager.Implementations;
using DevelopmentHell.Hubba.Authentication.Manager.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.OneTimePassword.Service.Implementations;
using DevelopmentHell.Hubba.Email.Service.Implementations;
using DevelopmentHell.Hubba.Authentication.Service.Implementations;
using DevelopmentHell.Hubba.Authentication.Manager.Implementations;
using DevelopmentHell.Hubba.Models;
using System.Security.Claims;
using DevelopmentHell.Hubba.Models.DTO;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using DevelopmentHell.Hubba.Files.Service.Implementations;
using DevelopmentHell.Hubba.Files.Service.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace DevelopmentHell.Hubba.ListingProfile.Test.Integration_Tests
{
    [TestClass]
    public class ListingProfileIntegrationTests
    {
        private readonly IListingProfileManager _listingProfileManager;

        //helper
        private readonly IAuthenticationManager _authenticationManager;
        private readonly ITestingService _testingService;
        private readonly IListingsDataAccess _listingsDataAccess;
        private readonly IUserAccountDataAccess _userAccountDataAccess;
        private readonly IRegistrationManager _registrationManager;
        private readonly IListingAvailabilitiesDataAccess _listingAvailabilitiesDataAccess;
        private readonly IListingHistoryDataAccess _listingHistoryDataAccess;
        private readonly IOTPDataAccess _otpDataAccess;
        private readonly ICryptographyService _cryptographyService;
        private readonly IFileService _fileService;

        private readonly string _userConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private readonly string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;

        private readonly string _listingProfileConnectionString = ConfigurationManager.AppSettings["ListingProfileConnectionString"]!;
        private readonly string _listingsTable = ConfigurationManager.AppSettings["ListingsTable"]!;
        private readonly string _listingAvailabilitiesTable = ConfigurationManager.AppSettings["ListingAvailabilitiesTable"]!;
        private readonly string _listingHistoryTable = ConfigurationManager.AppSettings["ListingHistoryTable"]!;
        private readonly string _listingRatingsTable = ConfigurationManager.AppSettings["ListingRatingsTable"]!;
        private readonly string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;

        private readonly string _notificationConnectionString = ConfigurationManager.AppSettings["NotificationsConnectionString"]!;
        private readonly string _userNotificationsTable = ConfigurationManager.AppSettings["UserNotificationsTable"]!;
        private readonly string _userNotificationSettingsTable = ConfigurationManager.AppSettings["NotificationSettingsTable"]!;
        private readonly string _userLoginsTable = ConfigurationManager.AppSettings["UserLoginsTable"]!;
        private readonly string _userOTPsTable = ConfigurationManager.AppSettings["UserOTPsTable"]!;


        private readonly string _ftpServer = ConfigurationManager.AppSettings["FTPServer"]!;
        private readonly string _ftpUsername = ConfigurationManager.AppSettings["FTPUsername"]!;
        private readonly string _ftpPassword = ConfigurationManager.AppSettings["FTPPassword"]!;

        private readonly string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;
        private string _cryptographyKey = ConfigurationManager.AppSettings["CryptographyKey"]!;


        private readonly string dirPath = "ListingProfiles";

        public ListingProfileIntegrationTests()
        {
            LoggerService loggerService = new LoggerService(
                new LoggerDataAccess(
                    _logsConnectionString,
                    _logsTable
                )
            );

            _fileService = new FTPFileService
            (
                _ftpServer,
                _ftpUsername,
                _ftpPassword,
                loggerService
            );

            _listingProfileManager = new ListingProfileManager
            (
                new ListingProfileService
                (
                    new ListingsDataAccess(_listingProfileConnectionString, _listingsTable),
                    new ListingAvailabilitiesDataAccess(_listingProfileConnectionString, _listingAvailabilitiesTable),
                    new ListingHistoryDataAccess(_listingProfileConnectionString, _listingHistoryTable),
                    new RatingDataAccess(_listingProfileConnectionString, _listingRatingsTable),
                    new UserAccountDataAccess(_userConnectionString, _userAccountsTable),
                    loggerService
                ),
                _fileService,
                new AuthorizationService
                (
                     new UserAccountDataAccess
                    (
                        _userConnectionString,
                        _userAccountsTable
                    ),
                      new JWTHandlerService(
                        _jwtKey
                    ),
                    loggerService
                ),
                loggerService,
                new ValidationService(),
                new CryptographyService
                (
                    _cryptographyKey
                )
            );

            _authenticationManager = new AuthenticationManager(
                new AuthenticationService
                (
                    new UserAccountDataAccess
                    (
                        _userConnectionString,
                        _userAccountsTable
                    ),
                    new UserLoginDataAccess
                    (
                        _userConnectionString,
                        _userLoginsTable
                    ),
                    new CryptographyService
                    (
                        _cryptographyKey
                    ),
                    new JWTHandlerService
                    (
                        _jwtKey
                    ),
                    new ValidationService(),
                    loggerService
                ),
                new OTPService
                (
                    new OTPDataAccess(
                        _userConnectionString,
                        _userOTPsTable
                    ),
                    new EmailService
                    (
                        ConfigurationManager.AppSettings["SENDGRID_USERNAME"]!,
                        ConfigurationManager.AppSettings["SENDGRID_API_KEY"]!,
                        ConfigurationManager.AppSettings["COMPANY_EMAIL"]!,
                        true
                    ),
                    new CryptographyService
                    (
                        _cryptographyKey
                    )
                ),
                new AuthorizationService
                (
                     new UserAccountDataAccess
                    (
                        _userConnectionString,
                        _userAccountsTable
                    ),
                      new JWTHandlerService(
                        _jwtKey
                    ),
                    loggerService
                ),
                new CryptographyService
                (
                    _cryptographyKey
                ),
                loggerService
            );

            _registrationManager = new RegistrationManager(
                new RegistrationService
                (
                    new UserAccountDataAccess
                    (
                        _userConnectionString,
                        _userAccountsTable
                    ),
                    new CryptographyService(
                        _cryptographyKey
                    ),
                    new ValidationService(),
                    loggerService
                ),
                new AuthorizationService
                (
                     new UserAccountDataAccess
                    (
                        _userConnectionString,
                        _userAccountsTable
                    ),
                      new JWTHandlerService(
                        _jwtKey
                    ),
                    loggerService
                ),
                new CryptographyService
                (
                    _cryptographyKey
                ),
                new NotificationService(
                    new NotificationDataAccess(
                        _notificationConnectionString,
                        _userNotificationsTable
                    ),
                    new NotificationSettingsDataAccess(
                        _notificationConnectionString,
                        _userNotificationSettingsTable
                    ),
                    new UserAccountDataAccess
                    (
                        _userConnectionString,
                        _userAccountsTable
                    ),
                    loggerService
                ),
                loggerService
            );

            _testingService = new TestingService(_jwtKey, new TestsDataAccess());

            _cryptographyService = new CryptographyService(_cryptographyKey);

            _listingsDataAccess = new ListingsDataAccess(_listingProfileConnectionString, _listingsTable);

            _userAccountDataAccess = new UserAccountDataAccess(_userConnectionString, _userAccountsTable);

            _listingAvailabilitiesDataAccess = new ListingAvailabilitiesDataAccess(_listingProfileConnectionString, _listingAvailabilitiesTable);

            _listingHistoryDataAccess = new ListingHistoryDataAccess(_listingProfileConnectionString, _listingHistoryTable);

            _otpDataAccess = new OTPDataAccess(_userConnectionString, _userOTPsTable);
        }

        [TestInitialize]
        public async Task Setup()
        {
            await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.LISTING_PROFILES).ConfigureAwait(false);
            await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.USERS).ConfigureAwait(false);
            await _fileService.DeleteDir(dirPath).ConfigureAwait(false);
        }


        [TestMethod]
        public async Task CreateListing()
        {
            //Arrange
            var email = "jeffreyjones@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Best Popcorn Chicken NA";

            var expected = true;

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            //Act
            var actual = await _listingProfileManager.CreateListing(title).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task EditListingFilesAdd1()
        {
            //Arrange
            var email = "jettsonoda@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Best Popcorn Chicken NA";

            var expected = true;

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            await _listingProfileManager.CreateListing(title).ConfigureAwait(false);

            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;


            Dictionary<string, IFormFile> files = new Dictionary<string, IFormFile>();
            IFormFile file1 = CreateFormFileFromFilePath("C:\\Users\\goodg\\OneDrive\\Pictures\\Screenshots\\Screenshot 2023-03-25 190512.png");
            var file1Name = "Memed_pic_xdd.png";
            files.Add(file1Name, file1);

            //Act
            var actual = await _listingProfileManager.EditListingFiles(listingId, null, files).ConfigureAwait(false);
            var getFile1 = await _fileService.GetFileReference(dirPath + "/" + listingId + "/" + file1Name).ConfigureAwait(false);
            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(getFile1.Payload == $"http://{_ftpServer}/{dirPath}/{listingId}/{file1Name}");
        }

        [TestMethod]
        public async Task EditListingFilesAdd1Delete1()
        {
            //Arrange
            var email = "jeffreyjones@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Best Popcorn Chicken NA";

            var expected = true;

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            await _listingProfileManager.CreateListing(title).ConfigureAwait(false);

            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            Dictionary<string, IFormFile> deletefiles = new Dictionary<string, IFormFile>();
            IFormFile file1 = CreateFormFileFromFilePath("C:\\Users\\goodg\\OneDrive\\Pictures\\Screenshots\\Screenshot_20230213_010932.png");
            var file1Name = "dumb_slatt.png";
            deletefiles.Add(file1Name, file1);

            await _listingProfileManager.EditListingFiles(listingId, null, deletefiles).ConfigureAwait(false);


            Dictionary<string, IFormFile> addfiles = new Dictionary<string, IFormFile>();
            IFormFile file2 = CreateFormFileFromFilePath("C:\\Users\\goodg\\OneDrive\\Pictures\\Screenshots\\Screenshot_20230219_021808.png");
            var file2Name = "big_fax.png";
            addfiles.Add(file2Name, file2);

            List<string> deleteFileNames = new List<string> { file1Name };

            //Act
            var actual = await _listingProfileManager.EditListingFiles(listingId, deleteFileNames, addfiles).ConfigureAwait(false);
            Console.WriteLine(actual.ErrorMessage);
            var getFile1 = await _fileService.GetFileReference(dirPath + "/" + listingId + "/" + file1Name).ConfigureAwait(false);
            var getFile2 = await _fileService.GetFileReference(dirPath + "/" + listingId + "/" + file2Name).ConfigureAwait(false);
            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(getFile1.Payload is null);
            Assert.IsTrue(getFile2.Payload == $"http://{_ftpServer}/{dirPath}/{listingId}/{file2Name}");
        }

        [TestMethod]
        public async Task CreateListingFailureUnauthorized()
        {
            //Arrange
            var email = "jeffreyjones@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Best Popcorn Chicken+if(benchmark(3000000,MD5(1)),NULL,NULL),NULL,NULL,NULL,NULL,NULL,NULL)%20-- NA";

            var expected = false;
            var expectedErrorMessage = "Invalid characters or special characters. Note apostrophes are the only special characters allowed.";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            //Act
            var actual = await _listingProfileManager.CreateListing(title).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }

        [TestMethod]
        public async Task CreateListingFailureInvalidTitle()
        {
            //Arrange
            var title = "Best Popcorn Chicken NA";

            var expected = false;
            var expectedErrorMessage = "Unauthorized user.";

            //Act
            var actual = await _listingProfileManager.CreateListing(title).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }

        [TestMethod]
        public async Task ViewUserListingsTitlesOnly()
        {
            //Arrange
            var email = "jeffreyjones@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title1 = "Best Popcorn Chicken NA";
            var title2 = "Best Popcorn Chicken EU";

            var expected = true;

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            await _listingProfileManager.CreateListing(title1).ConfigureAwait(false);
            await _listingProfileManager.CreateListing(title2).ConfigureAwait(false);

            //Act
            var actual = await _listingProfileManager.ViewUserListings().ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload is not null);
            Assert.IsTrue(actual.Payload.Count == 2);
        }

        [TestMethod]
        public async Task ViewUserListingsNoListings()
        {
            //Arrange
            var email = "jeffreyjones@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";

            var expected = true;

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }


            //Act
            var actual = await _listingProfileManager.ViewUserListings().ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload is not null);
            Assert.IsTrue(actual.Payload.Count == 0);
        }

        [TestMethod]
        public async Task ViewUserListingsFailureUnauthorized()
        {
            //Arrange
            var expected = false;
            var expectedErrorMessage = "Unauthorized user.";

            //Act
            var actual = await _listingProfileManager.ViewUserListings().ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }

        [TestMethod]
        public async Task ViewListingUnpublishedAsOwner()
        {
            //Arrange
            var email = "jeffreyjones@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Best Popcorn Chicken NA";

            var expected = true;

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            await _listingProfileManager.CreateListing(title).ConfigureAwait(false);

            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            //Act
            var actual = await _listingProfileManager.ViewListing(listingId).ConfigureAwait(false);
            

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload.Count == 4);
            Assert.IsTrue(actual.Payload["Listing"] is ListingViewDTO);
            Assert.IsTrue((actual.Payload["Listing"] as ListingViewDTO).OwnerId.Equals(newAccountId));
            Assert.IsTrue((actual.Payload["Listing"] as ListingViewDTO).Title.Equals(title));
            Assert.IsTrue((actual.Payload["Listing"] as ListingViewDTO).Description is null);
            Assert.IsTrue((actual.Payload["Listing"] as ListingViewDTO).Location is null);
            Assert.IsTrue((actual.Payload["Listing"] as ListingViewDTO).Price is null);
            Assert.IsTrue((actual.Payload["Listing"] as ListingViewDTO).Published.Equals(false));
            Assert.IsTrue((actual.Payload["Listing"] as ListingViewDTO).AverageRating is null);
            Assert.IsTrue((actual.Payload["Availabilities"] is List<ListingAvailabilityDTO>));
            Assert.IsTrue((actual.Payload["Availabilities"] as List<ListingAvailabilityDTO>).Count == 0);
            Assert.IsTrue((actual.Payload["Ratings"] as List<ListingRatingViewDTO>).Count == 0);
        }

        [TestMethod]
        public async Task ViewListingPublishedAsRandom()
        {
            //Arrange
            var email1 = "jeffreyjones@gmail.com";
            var email2 = "jacoblol@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Best Popcorn Chicken NA";

            var expected = true;

            //register 2 accounts
            await _registrationManager.Register(email1, password).ConfigureAwait(false);
            await _registrationManager.Register(email2, password).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;

            //create unpublished listing with user 1
            await _listingsDataAccess.CreateListing(newAccountId, title).ConfigureAwait(false);

            //log into user 2
            var loginResult = await _authenticationManager.Login(email2, password, dummyIp).ConfigureAwait(false);
            Result<int> getUserId2 = await _userAccountDataAccess.GetId(email2).ConfigureAwait(false);
            int userId2 = getUserId2.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(userId2).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }


            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;
            _listingsDataAccess.PublishListing(listingId);

            //Act
            var actual = await _listingProfileManager.ViewListing(listingId).ConfigureAwait(false);


            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.Payload.Count == 4);
            Assert.IsTrue(actual.Payload["Listing"] is ListingViewDTO);
            Assert.IsTrue((actual.Payload["Listing"] as ListingViewDTO).OwnerId.Equals(newAccountId));
            Assert.IsTrue((actual.Payload["Listing"] as ListingViewDTO).Title.Equals(title));
            Assert.IsTrue((actual.Payload["Listing"] as ListingViewDTO).Description is null);
            Assert.IsTrue((actual.Payload["Listing"] as ListingViewDTO).Location is null);
            Assert.IsTrue((actual.Payload["Listing"] as ListingViewDTO).Price is null);
            Assert.IsTrue((actual.Payload["Listing"] as ListingViewDTO).Published.Equals(true));
            Assert.IsTrue((actual.Payload["Listing"] as ListingViewDTO).AverageRating is null);
            Assert.IsTrue((actual.Payload["Availabilities"] is List<ListingAvailabilityDTO>));
            Assert.IsTrue((actual.Payload["Availabilities"] as List<ListingAvailabilityDTO>).Count == 0);
            Assert.IsTrue((actual.Payload["Ratings"] as List<ListingRatingViewDTO>).Count == 0);
        }

        [TestMethod]
        public async Task ViewListingFailureUnauthorizedToken()
        {
            //Arrange
            var email = "jeffreyjones@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Best Popcorn Chicken NA";

            var expected = false;
            var expectedErrorMessage = "Unauthorized user.";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            
            await _listingsDataAccess.CreateListing(newAccountId, title).ConfigureAwait(false);

            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            //Act
            var actual = await _listingProfileManager.ViewListing(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }

        [TestMethod]
        public async Task ViewListingFailureUnauthorizedNotOwner()
        {
            //Arrange
            var email1 = "jeffreyjones@gmail.com";
            var email2 = "jacoblol@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Best Popcorn Chicken NA";

            var expected = false;
            var expectedErrorMessage = "Unauthorized user.";

            //register 2 accounts
            await _registrationManager.Register(email1, password).ConfigureAwait(false);
            await _registrationManager.Register(email2, password).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;

            //create unpublished listing with user 1
            await _listingsDataAccess.CreateListing(newAccountId, title).ConfigureAwait(false);

            //log into user 2
            var loginResult = await _authenticationManager.Login(email2, password, dummyIp).ConfigureAwait(false);
            Result<int> getUserId2 = await _userAccountDataAccess.GetId(email2).ConfigureAwait(false);
            int userId2 = getUserId2.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(userId2).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            
            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            //Act
            var actual = await _listingProfileManager.ViewListing(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }

        [TestMethod]
        public async Task ViewListingFailureNotFound()
        {
            //Arrange
            var email1 = "jeffreyjones@gmail.com";
            var email2 = "jacoblol@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var listingId = 1;

            var expected = false;
            var expectedErrorMessage = "Cannot retrieve specified listing.";

            //register 2 accounts
            await _registrationManager.Register(email1, password).ConfigureAwait(false);
            await _registrationManager.Register(email2, password).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;

            //log into user 2
            var loginResult = await _authenticationManager.Login(email2, password, dummyIp).ConfigureAwait(false);
            Result<int> getUserId2 = await _userAccountDataAccess.GetId(email2).ConfigureAwait(false);
            int userId2 = getUserId2.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(userId2).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            //Act
            var actual = await _listingProfileManager.ViewListing(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }

        [TestMethod]
        public async Task EditListingWithLocation()
        {
            //Arrange
            var email = "jeffreyjones@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Its 5am and I hate myself";
            var location = "911 hehe st.";
            var published = false;


            var expected = true;

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }


            await _listingProfileManager.CreateListing(title).ConfigureAwait(false);
            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            ListingEditorDTO listingEdit = new ListingEditorDTO()
            {
                ListingId = listingId,
                Title = title,
                OwnerId = newAccountId,
                Published = published,
                Location = location,
                Description = null,
                Price = null
            };

            //Act
            var actual = await _listingProfileManager.EditListing(listingEdit).ConfigureAwait(false);
            var getListing = await _listingsDataAccess.GetListing(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(getListing.Payload is not null);
            Assert.IsTrue(getListing.Payload.ListingId.Equals(listingId));
            Assert.IsTrue(getListing.Payload.OwnerId.Equals(newAccountId));
            Assert.IsTrue(getListing.Payload.Title.Equals(title));
            Assert.IsTrue(getListing.Payload.Published.Equals(published));
            Assert.IsTrue(getListing.Payload.Location.Equals(location));
            Assert.IsTrue(getListing.Payload.Description is null);
            Assert.IsTrue(getListing.Payload.Price is null);
        }

        [TestMethod]
        public async Task EditListingRemoveLocation()
        {
            //Arrange
            var email = "jeffreyjones@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Its 5am and I hate myself";
            var location = "911 hehe st.";
            var published = false;

            var expected = true;

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            //add listing with location
            await _listingProfileManager.CreateListing(title).ConfigureAwait(false);
            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            ListingEditorDTO listingEdit1 = new ListingEditorDTO()
            {
                ListingId = listingId,
                Title = title,
                OwnerId = newAccountId,
                Published = published,
                Location = location,
                Description = null,
                Price = null
            };
            await _listingProfileManager.EditListing(listingEdit1).ConfigureAwait(false);


            //new edit removing location
            ListingEditorDTO listingEdit2 = new ListingEditorDTO()
            {
                ListingId = listingId,
                Title = title,
                OwnerId = newAccountId,
                Published = published,
                Location = null,
                Description = null,
                Price = null
            };

            //Act
            var actual = await _listingProfileManager.EditListing(listingEdit2).ConfigureAwait(false);
            var getListing = await _listingsDataAccess.GetListing(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(getListing.Payload is not null);
            Assert.IsTrue(getListing.Payload.ListingId.Equals(listingId));
            Assert.IsTrue(getListing.Payload.OwnerId.Equals(newAccountId));
            Assert.IsTrue(getListing.Payload.Title.Equals(title));
            Assert.IsTrue(getListing.Payload.Published.Equals(published));
            Assert.IsTrue(getListing.Payload.Location is null);
            Assert.IsTrue(getListing.Payload.Description is null);
            Assert.IsTrue(getListing.Payload.Price is null);
        }

        [TestMethod]
        public async Task EditListingFailureInvalidDescription()
        {
            //Arrange
            var email = "jeffreyjones@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Its 5am and I hate myself";
            var location = "911 hehe st.";
            var desc = "This is a desc or admin'--";
            var published = false;

            var expected = false;
            var expectedErrorMessage = "Invalid characters or special characters. Note only apostrophes, commas, periods, exclamation marks, and parentheses are the only special characters allowed.";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            await _listingProfileManager.CreateListing(title).ConfigureAwait(false);
            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            ListingEditorDTO listingEdit = new ListingEditorDTO()
            {
                ListingId = listingId,
                Title = title,
                OwnerId = newAccountId,
                Published = published,
                Location = location,
                Description = desc,
                Price = null
            };

            //Act
            var actual = await _listingProfileManager.EditListing(listingEdit).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }

        [TestMethod]
        public async Task EditListingAvailabilities()
        {
            //Arrange
            var email = "jeffreyjones@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Its 5am and I hate myself";

            var expected = true;
            var expectedCount = 3;

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            await _listingProfileManager.CreateListing(title).ConfigureAwait(false);
            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;
            DateTime now = DateTime.Now.AddMinutes(30 - DateTime.Now.Minute % 30).AddSeconds(-DateTime.Now.Second).AddMilliseconds(-DateTime.Now.Millisecond);

            List<ListingAvailabilityDTO> temp = new List<ListingAvailabilityDTO>();
            ListingAvailabilityDTO temp1 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                OwnerId = newAccountId,
                StartTime = now.AddMinutes(30),
                EndTime = now.AddHours(5),
                Action = AvailabilityAction.Add,
            };
            ListingAvailabilityDTO temp2 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                OwnerId = newAccountId,
                StartTime = now.AddMinutes(30),
                EndTime = now.AddHours(9),
                Action = AvailabilityAction.Add,
            };
            temp.Add(temp1);
            temp.Add(temp2);

            await _listingAvailabilitiesDataAccess.AddListingAvailabilities(temp).ConfigureAwait(false);
            var getTempIds = await _listingAvailabilitiesDataAccess.GetListingAvailabilities(listingId);


            List<ListingAvailabilityDTO> listingAvailabilityDTOs = new List<ListingAvailabilityDTO>();
            ListingAvailabilityDTO avail1 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                OwnerId = newAccountId,
                StartTime = now.AddMinutes(30),
                EndTime = now.AddHours(1),
                Action = AvailabilityAction.Add,
            };

            ListingAvailabilityDTO avail2 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                OwnerId = newAccountId,
                StartTime = now.AddMinutes(30),
                EndTime = now.AddHours(2),
                Action = AvailabilityAction.Add,
            };

            ListingAvailabilityDTO avail3 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                OwnerId = newAccountId,
                AvailabilityId = getTempIds.Payload[0].AvailabilityId,
                StartTime = now.AddMinutes(30),
                EndTime = now.AddHours(1),
                Action = AvailabilityAction.Update,
            };

            ListingAvailabilityDTO avail4 = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                OwnerId = newAccountId,
                AvailabilityId = getTempIds.Payload[1].AvailabilityId,
                StartTime = now.AddMinutes(30),
                EndTime = now.AddHours(1),
                Action = AvailabilityAction.Delete,
            };

            listingAvailabilityDTOs.Add(avail1);
            listingAvailabilityDTOs.Add(avail2);
            listingAvailabilityDTOs.Add(avail3);
            listingAvailabilityDTOs.Add(avail4);


            //Act
            var actual = await _listingProfileManager.EditListingAvailabilities(listingAvailabilityDTOs).ConfigureAwait(false);
            var getAvailabilities = await _listingAvailabilitiesDataAccess.GetListingAvailabilities(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(getAvailabilities.Payload.Count == expectedCount);
        }

        [TestMethod]
        public async Task EditListingAvailabilitiesFailureInvalidDays()
        {
            //Arrange
            var email = "jeffreyjones@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Its 5am and I hate myself";

            var expected = false;
            var expectedErrorMessage = "Start time and end time may not be on different days.";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            await _listingProfileManager.CreateListing(title).ConfigureAwait(false);
            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            DateTime now = DateTime.Now.AddMinutes(30 - DateTime.Now.Minute % 30).AddSeconds(-DateTime.Now.Second).AddMilliseconds(-DateTime.Now.Millisecond);

            List<ListingAvailabilityDTO> listingAvailabilityDTOs = new List<ListingAvailabilityDTO>();

            ListingAvailabilityDTO avail = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                OwnerId = newAccountId,
                StartTime = now.AddDays(3),
                EndTime = now.AddHours(5),
                Action = AvailabilityAction.Add,
            };

            listingAvailabilityDTOs.Add(avail);

            //Act
            var actual = await _listingProfileManager.EditListingAvailabilities(listingAvailabilityDTOs).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage.Trim() == expectedErrorMessage);
        }

        [TestMethod]
        public async Task EditListingAvailabilitiesFailureInvalidStartTime1()
        {
            //Arrange
            var email = "jeffreyjones@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Its 5am and I hate myself";

            var expected = false;
            var expectedErrorMessage = "Start time must be on the hour or half hour.";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            await _listingProfileManager.CreateListing(title).ConfigureAwait(false);
            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            DateTime now = DateTime.Now.AddMinutes(30 - DateTime.Now.Minute % 30).AddSeconds(-DateTime.Now.Second).AddMilliseconds(-DateTime.Now.Millisecond);
            

            List<ListingAvailabilityDTO> listingAvailabilityDTOs = new List<ListingAvailabilityDTO>();

            ListingAvailabilityDTO avail = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                OwnerId = newAccountId,
                StartTime = now.AddMinutes(1),
                EndTime = now.AddHours(1),
                Action = AvailabilityAction.Add,
            };

            listingAvailabilityDTOs.Add(avail);

            //Act
            var actual = await _listingProfileManager.EditListingAvailabilities(listingAvailabilityDTOs).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage.Trim() == expectedErrorMessage);
        }

        [TestMethod]
        public async Task EditListingAvailabilitiesFailureInvalidStartTime2()
        {
            //Arrange
            var email = "jeffreyjones@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Its 5am and I hate myself";

            var expected = false;
            var expectedErrorMessage = "Start time must be more than 30 minutes of right now.";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            await _listingProfileManager.CreateListing(title).ConfigureAwait(false);
            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            DateTime now = DateTime.Now.AddMinutes(30 - DateTime.Now.Minute % 30).AddSeconds(-DateTime.Now.Second).AddMilliseconds(-DateTime.Now.Millisecond);

            List<ListingAvailabilityDTO> listingAvailabilityDTOs = new List<ListingAvailabilityDTO>();

            ListingAvailabilityDTO avail = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                OwnerId = newAccountId,
                StartTime = now.AddHours(-1), 
                EndTime = now.AddHours(1),
                Action = AvailabilityAction.Add,
            };

            listingAvailabilityDTOs.Add(avail);

            //Act
            var actual = await _listingProfileManager.EditListingAvailabilities(listingAvailabilityDTOs).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }

        [TestMethod]
        public async Task EditListingAvailabilitiesFailureInvalidEndTime1()
        {
            //Arrange
            var email = "jeffreyjones@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Its 5am and I hate myself";

            var expected = false;
            var expectedErrorMessage = "End time must be on the hour or half hour.";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            await _listingProfileManager.CreateListing(title).ConfigureAwait(false);
            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            DateTime now = DateTime.Now.AddMinutes(30 - DateTime.Now.Minute % 30).AddSeconds(-DateTime.Now.Second).AddMilliseconds(-DateTime.Now.Millisecond);


            List<ListingAvailabilityDTO> listingAvailabilityDTOs = new List<ListingAvailabilityDTO>();

            ListingAvailabilityDTO avail = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                OwnerId = newAccountId,
                StartTime = now,
                EndTime = now.AddMinutes(1),
                Action = AvailabilityAction.Add,
            };

            listingAvailabilityDTOs.Add(avail);

            //Act
            var actual = await _listingProfileManager.EditListingAvailabilities(listingAvailabilityDTOs).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage.Trim() == expectedErrorMessage);
        }

        [TestMethod]
        public async Task EditListingAvailabilitiesFailureInvalidEndTime2()
        {
            //Arrange
            var email = "jeffreyjones@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Its 5am and I hate myself";

            var expected = false;
            var expectedErrorMessage = "End time must be at least 30 mins past start time.";

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            await _listingProfileManager.CreateListing(title).ConfigureAwait(false);
            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            DateTime now = DateTime.Now.AddMinutes(30 - DateTime.Now.Minute % 30).AddSeconds(-DateTime.Now.Second).AddMilliseconds(-DateTime.Now.Millisecond);

            List<ListingAvailabilityDTO> listingAvailabilityDTOs = new List<ListingAvailabilityDTO>();

            ListingAvailabilityDTO avail = new ListingAvailabilityDTO()
            {
                ListingId = listingId,
                OwnerId = newAccountId,
                StartTime = now,
                EndTime = now,
                Action = AvailabilityAction.Add,
            };

            listingAvailabilityDTOs.Add(avail);

            //Act
            var actual = await _listingProfileManager.EditListingAvailabilities(listingAvailabilityDTOs).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }

        [TestMethod]
        public async Task DeleteListing()
        {
            //Arrange
            var email = "jeffreyjones@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Best Popcorn Chicken NA";

            var expected = true;

            await _registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await _authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            await _listingProfileManager.CreateListing(title).ConfigureAwait(false);
            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            //Act
            var actual = await _listingProfileManager.DeleteListing(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task DeleteListingFailureUnauthorizedOwner()
        {
            //Arrange
            var email1 = "jeffreyjones@gmail.com";
            var email2 = "jacoblol@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Best Popcorn Chicken NA";

            var expected = false;
            var expectedErrorMessage = "Unauthorized user.";

            //register 2 accounts
            await _registrationManager.Register(email1, password).ConfigureAwait(false);
            await _registrationManager.Register(email2, password).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;

            //create unpublished listing with user 1
            await _listingsDataAccess.CreateListing(newAccountId, title).ConfigureAwait(false);

            //log into user 2
            var loginResult = await _authenticationManager.Login(email2, password, dummyIp).ConfigureAwait(false);
            Result<int> getUserId2 = await _userAccountDataAccess.GetId(email2).ConfigureAwait(false);
            int userId2 = getUserId2.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(userId2).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }


            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            //Act
            var actual = await _listingProfileManager.DeleteListing(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }

        [TestMethod]
        public async Task DeleteListingFailureUnauthorizedToken()
        {
            //Arrange
            var ownerId = 1;
            var title = "Best Popcorn Chicken NA";

            var expected = false;
            var expectedErrorMessage = "Unauthorized user.";


            await _listingsDataAccess.CreateListing(ownerId, title).ConfigureAwait(false);
            var getListingId = await _listingsDataAccess.GetListingId(ownerId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            //Act
            var actual = await _listingProfileManager.DeleteListing(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }

        [TestMethod]
        public async Task AddRating()
        {
            //Arrange
            var email1 = "jeffreyjones@gmail.com";
            var email2 = "jacoblol@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Best Popcorn Chicken NA";

            var expected = true;

            //register 2 accounts
            await _registrationManager.Register(email1, password).ConfigureAwait(false);
            await _registrationManager.Register(email2, password).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;

            //create unpublished listing with user 1
            await _listingsDataAccess.CreateListing(newAccountId, title).ConfigureAwait(false);

            //log into user 2
            var loginResult = await _authenticationManager.Login(email2, password, dummyIp).ConfigureAwait(false);
            Result<int> getUserId2 = await _userAccountDataAccess.GetId(email2).ConfigureAwait(false);
            int userId2 = getUserId2.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(userId2).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            await _listingHistoryDataAccess.AddUser(listingId, userId2).ConfigureAwait(false);

            var rating = 5;
            var anonymous = true;

            //Act
            var actual = await _listingProfileManager.AddRating(listingId, rating, null, anonymous).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task AddRatingFailureInvalidRating()
        {
            //Arrange
            var email1 = "jeffreyjones@gmail.com";
            var email2 = "jacoblol@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Best Popcorn Chicken NA";

            var expected = false;
            var expectedErrorMessage = "Invalid rating.";

            //register 2 accounts
            await _registrationManager.Register(email1, password).ConfigureAwait(false);
            await _registrationManager.Register(email2, password).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;

            //create unpublished listing with user 1
            await _listingsDataAccess.CreateListing(newAccountId, title).ConfigureAwait(false);

            //log into user 2
            var loginResult = await _authenticationManager.Login(email2, password, dummyIp).ConfigureAwait(false);
            Result<int> getUserId2 = await _userAccountDataAccess.GetId(email2).ConfigureAwait(false);
            int userId2 = getUserId2.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(userId2).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            await _listingHistoryDataAccess.AddUser(listingId, userId2).ConfigureAwait(false);

            var rating = -1;
            var anonymous = true;

            //Act
            var actual = await _listingProfileManager.AddRating(listingId, rating, null, anonymous).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }

        [TestMethod]
        public async Task AddRatingFailureInvalidComment()
        {
            //Arrange
            var email1 = "jeffreyjones@gmail.com";
            var email2 = "jacoblol@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Best Popcorn Chicken NA";

            var expected = false;
            var expectedErrorMessage = "Invalid characters or special characters. Note only apostrophes, commas, periods, exclamation marks, and parentheses are the only special characters allowed.";

            //register 2 accounts
            await _registrationManager.Register(email1, password).ConfigureAwait(false);
            await _registrationManager.Register(email2, password).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;

            //create unpublished listing with user 1
            await _listingsDataAccess.CreateListing(newAccountId, title).ConfigureAwait(false);

            //log into user 2
            var loginResult = await _authenticationManager.Login(email2, password, dummyIp).ConfigureAwait(false);
            Result<int> getUserId2 = await _userAccountDataAccess.GetId(email2).ConfigureAwait(false);
            int userId2 = getUserId2.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(userId2).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            await _listingHistoryDataAccess.AddUser(listingId, userId2).ConfigureAwait(false);

            var rating = 4;
            var anonymous = true;
            var description = "I love SQL injections lol '--0 = 0";

            //Act
            var actual = await _listingProfileManager.AddRating(listingId, rating, description, anonymous).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }

        [TestMethod]
        public async Task AddRatingFailureNotBooked()
        {
            //Arrange
            var email1 = "jeffreyjones@gmail.com";
            var email2 = "jacoblol@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Best Popcorn Chicken NA";

            var expected = false;
            var expectedErrorMessage = "Unauthorized user.";

            //register 2 accounts
            await _registrationManager.Register(email1, password).ConfigureAwait(false);
            await _registrationManager.Register(email2, password).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;

            //create unpublished listing with user 1
            await _listingsDataAccess.CreateListing(newAccountId, title).ConfigureAwait(false);

            //log into user 2
            var loginResult = await _authenticationManager.Login(email2, password, dummyIp).ConfigureAwait(false);
            Result<int> getUserId2 = await _userAccountDataAccess.GetId(email2).ConfigureAwait(false);
            int userId2 = getUserId2.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(userId2).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;


            var rating = 4;
            var anonymous = true;

            //Act
            var actual = await _listingProfileManager.AddRating(listingId, rating, null, anonymous).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }


        [TestMethod]
        public async Task EditRating()
        {
            //Arrange
            var email1 = "jeffreyjones@gmail.com";
            var email2 = "jacoblol@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Best Popcorn Chicken NA";

            var expected = true;

            //register 2 accounts
            await _registrationManager.Register(email1, password).ConfigureAwait(false);
            await _registrationManager.Register(email2, password).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;

            //create unpublished listing with user 1
            await _listingsDataAccess.CreateListing(newAccountId, title).ConfigureAwait(false);

            //log into user 2
            var loginResult = await _authenticationManager.Login(email2, password, dummyIp).ConfigureAwait(false);
            Result<int> getUserId2 = await _userAccountDataAccess.GetId(email2).ConfigureAwait(false);
            int userId2 = getUserId2.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(userId2).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            await _listingHistoryDataAccess.AddUser(listingId, userId2).ConfigureAwait(false);

            var rating = 5;
            var anonymous = true;

            await _listingProfileManager.AddRating(listingId, rating, null, anonymous).ConfigureAwait(false);


            var newRating = 4;
            var newComment = "mid";
            var newAnon = false;
            ListingRatingEditorDTO ratingEditorDTO = new ListingRatingEditorDTO()
            {
                ListingId = listingId,
                Rating = newRating,
                Comment = newComment,
                Anonymous = newAnon,
            };

            //Act
            var actual = await _listingProfileManager.EditRating(ratingEditorDTO).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task EditRatingFailureInvalidRating()
        {
            //Arrange
            var email1 = "jeffreyjones@gmail.com";
            var email2 = "jacoblol@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Best Popcorn Chicken NA";

            var expected = false;
            var expectedErrorMessage = "Invalid rating.";

            //register 2 accounts
            await _registrationManager.Register(email1, password).ConfigureAwait(false);
            await _registrationManager.Register(email2, password).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;

            //create unpublished listing with user 1
            await _listingsDataAccess.CreateListing(newAccountId, title).ConfigureAwait(false);

            //log into user 2
            var loginResult = await _authenticationManager.Login(email2, password, dummyIp).ConfigureAwait(false);
            Result<int> getUserId2 = await _userAccountDataAccess.GetId(email2).ConfigureAwait(false);
            int userId2 = getUserId2.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(userId2).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            await _listingHistoryDataAccess.AddUser(listingId, userId2).ConfigureAwait(false);

            var rating = 5;
            var anonymous = true;

            await _listingProfileManager.AddRating(listingId, rating, null, anonymous).ConfigureAwait(false);


            var newRating = -1;
            var newComment = "mid";
            var newAnon = false;
            ListingRatingEditorDTO ratingEditorDTO = new ListingRatingEditorDTO()
            {
                ListingId = listingId,
                Rating = newRating,
                Comment = newComment,
                Anonymous = newAnon,
            };

            //Act
            var actual = await _listingProfileManager.EditRating(ratingEditorDTO).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }

        [TestMethod]
        public async Task EditRatingFailureInvalidComment()
        {
            //Arrange
            var email1 = "jeffreyjones@gmail.com";
            var email2 = "jacoblol@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Best Popcorn Chicken NA";

            var expected = false;
            var expectedErrorMessage = "Invalid characters or special characters. Note only apostrophes, commas, periods, exclamation marks, and parentheses are the only special characters allowed.";

            //register 2 accounts
            await _registrationManager.Register(email1, password).ConfigureAwait(false);
            await _registrationManager.Register(email2, password).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;

            //create unpublished listing with user 1
            await _listingsDataAccess.CreateListing(newAccountId, title).ConfigureAwait(false);

            //log into user 2
            var loginResult = await _authenticationManager.Login(email2, password, dummyIp).ConfigureAwait(false);
            Result<int> getUserId2 = await _userAccountDataAccess.GetId(email2).ConfigureAwait(false);
            int userId2 = getUserId2.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(userId2).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            await _listingHistoryDataAccess.AddUser(listingId, userId2).ConfigureAwait(false);

            var rating = 5;
            var anonymous = true;

            await _listingProfileManager.AddRating(listingId, rating, null, anonymous).ConfigureAwait(false);


            var newRating = 5;
            var newComment = "I love SQL injections lol '--0 = 0";
            var newAnon = false;
            ListingRatingEditorDTO ratingEditorDTO = new ListingRatingEditorDTO()
            {
                ListingId = listingId,
                Rating = newRating,
                Comment = newComment,
                Anonymous = newAnon,
            };

            //Act
            var actual = await _listingProfileManager.EditRating(ratingEditorDTO).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }

        [TestMethod]
        public async Task DeleteRating()
        {
            //Arrange
            var email1 = "jeffreyjones@gmail.com";
            var email2 = "jacoblol@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Best Popcorn Chicken NA";

            var expected = true;

            //register 2 accounts
            await _registrationManager.Register(email1, password).ConfigureAwait(false);
            await _registrationManager.Register(email2, password).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;

            //create unpublished listing with user 1
            await _listingsDataAccess.CreateListing(newAccountId, title).ConfigureAwait(false);

            //log into user 2
            var loginResult = await _authenticationManager.Login(email2, password, dummyIp).ConfigureAwait(false);
            Result<int> getUserId2 = await _userAccountDataAccess.GetId(email2).ConfigureAwait(false);
            int userId2 = getUserId2.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(userId2).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            await _listingHistoryDataAccess.AddUser(listingId, userId2).ConfigureAwait(false);

            var rating = 5;
            var anonymous = true;
            await _listingProfileManager.AddRating(listingId, rating, null, anonymous).ConfigureAwait(false);

            //Act
            var actual = await _listingProfileManager.DeleteRating(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
        }

        [TestMethod]
        public async Task DeleteRatingFailureInvalidRating()
        {
            //Arrange
            var email1 = "jeffreyjones@gmail.com";
            var email2 = "jacoblol@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Best Popcorn Chicken NA";

            var expected = false;
            var expectedErrorMessage = "Unauthorized user.";

            //register 2 accounts
            await _registrationManager.Register(email1, password).ConfigureAwait(false);
            await _registrationManager.Register(email2, password).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;

            //create unpublished listing with user 1
            await _listingsDataAccess.CreateListing(newAccountId, title).ConfigureAwait(false);

            //log into user 2
            var loginResult = await _authenticationManager.Login(email2, password, dummyIp).ConfigureAwait(false);
            Result<int> getUserId2 = await _userAccountDataAccess.GetId(email2).ConfigureAwait(false);
            int userId2 = getUserId2.Payload;
            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(userId2).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);
            _testingService.DecodeJWT(loginResult.Payload!);
            var authenticatedResult = await _authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            ClaimsPrincipal? actualPrincipal = null;
            if (authenticatedResult.IsSuccessful)
            {
                _testingService.DecodeJWT(authenticatedResult.Payload!.Item1, authenticatedResult.Payload!.Item2);
                actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            }

            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            await _listingHistoryDataAccess.AddUser(listingId, userId2).ConfigureAwait(false);


            //Act
            var actual = await _listingProfileManager.DeleteRating(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }

        [TestMethod]
        public async Task DeleteRatingFailureInvalidToken()
        {
            //Arrange
            var email1 = "jeffreyjones@gmail.com";
            var email2 = "jacoblol@gmail.com";
            var password = "12345678";
            var dummyIp = "192.0.2.0";
            var title = "Best Popcorn Chicken NA";

            var expected = false;
            var expectedErrorMessage = "Unauthorized user.";

            //register 2 accounts
            await _registrationManager.Register(email1, password).ConfigureAwait(false);
            await _registrationManager.Register(email2, password).ConfigureAwait(false);
            Result<int> getNewAccountId = await _userAccountDataAccess.GetId(email1).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;

            //create unpublished listing with user 1
            await _listingsDataAccess.CreateListing(newAccountId, title).ConfigureAwait(false);


            var getListingId = await _listingsDataAccess.GetListingId(newAccountId, title).ConfigureAwait(false);
            int listingId = getListingId.Payload;

            await _listingHistoryDataAccess.AddUser(listingId, 1).ConfigureAwait(false);


            //Act
            var actual = await _listingProfileManager.DeleteRating(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage == expectedErrorMessage);
        }


        public IFormFile CreateFormFileFromFilePath(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var stream = new FileStream(filePath, FileMode.Open);
            var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = GetContentType(fileName)
            };

            return formFile;
        }
        private string GetContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
