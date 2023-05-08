using System.Configuration;
using System.Security.Claims;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.Authentication.Manager.Abstractions;
using DevelopmentHell.Hubba.Authentication.Manager.Implementations;
using DevelopmentHell.Hubba.Authentication.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Email.Service.Implementations;
using DevelopmentHell.Hubba.Files.Service.Abstractions;
using DevelopmentHell.Hubba.Files.Service.Implementations;
using DevelopmentHell.Hubba.ListingProfile.Manager.Implementations;
using DevelopmentHell.Hubba.ListingProfile.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.Notification.Service.Implementations;
using DevelopmentHell.Hubba.OneTimePassword.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Manager.Abstractions;
using DevelopmentHell.Hubba.Registration.Manager.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using DevelopmentHell.ListingProfile.Manager.Abstractions;
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

        private readonly string _listingProfileConnectionString = ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!;
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
        private readonly string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;
        private readonly string _cryptographyKey = ConfigurationManager.AppSettings["CryptographyKey"]!;


        private readonly string dirPath = "ListingProfiles";
        //private readonly string 

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


            List<Tuple<string, string>> files = new List<Tuple<string, string>>();
            string file1bytes = "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";
            var file1Name = "Memed_pic_xdd.png";
            files.Add(new Tuple<string, string>(file1Name, file1bytes));

            //Act
            var actual = await _listingProfileManager.EditListingFiles(listingId, null, files).ConfigureAwait(false);
            var getFile1 = await _fileService.GetFileReference(dirPath + "/" + listingId + "/Pictures/" + file1Name).ConfigureAwait(false);
            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(getFile1.Payload == $"http://{_ftpServer}/{dirPath}/{listingId}/Pictures/{file1Name}");
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

            List<Tuple<string, string>> deletefiles = new List<Tuple<string, string>>();
            string file1 = "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";
            var file1Name = "dumb_slatt.png";
            deletefiles.Add(new Tuple<string, string>(file1Name, file1));

            await _listingProfileManager.EditListingFiles(listingId, null, deletefiles).ConfigureAwait(false);


            List<Tuple<string, string>> addfiles = new List<Tuple<string, string>>();
            string file2 = "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";
            var file2Name = "big_fax.png";
            addfiles.Add(new Tuple<string, string>(file2Name, file2));

            List<string> deleteFileNames = new List<string> { file1Name };

            //Act
            var actual = await _listingProfileManager.EditListingFiles(listingId, deleteFileNames, addfiles).ConfigureAwait(false);
            //Console.WriteLine(actual.ErrorMessage);
            var getFile1 = await _fileService.GetFileReference(dirPath + "/" + listingId + "/Pictures/" + file1Name).ConfigureAwait(false);
            var getFile2 = await _fileService.GetFileReference(dirPath + "/" + listingId + "/Pictures/" + file2Name).ConfigureAwait(false);
            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(getFile1.Payload is null);
            Assert.IsTrue(getFile2.Payload == $"http://{_ftpServer}/{dirPath}/{listingId}/Pictures/{file2Name}");
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
            Assert.IsTrue((actual.Payload["Availabilities"] is List<ListingAvailabilityViewDTO>));
            Assert.IsTrue((actual.Payload["Availabilities"] as List<ListingAvailabilityViewDTO>).Count == 0);
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
            Assert.IsTrue((actual.Payload["Availabilities"] is List<ListingAvailabilityViewDTO>));
            Assert.IsTrue((actual.Payload["Availabilities"] as List<ListingAvailabilityViewDTO>).Count == 0);
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

            List<ListingAvailabilityReactDTO> tempList = new List<ListingAvailabilityReactDTO>();

            ListingAvailabilityReactDTO temp1 = new ListingAvailabilityReactDTO()
            {
                ListingId = listingId,
                OwnerId = newAccountId,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(3)).ToString(),
                StartTime = "10:00",
                EndTime = "13:00",
                Action = AvailabilityAction.Add
            };

            ListingAvailabilityReactDTO temp2 = new ListingAvailabilityReactDTO()
            {
                ListingId = listingId,
                OwnerId = newAccountId,
                StartTime = "11:30",
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(3)).ToString(),
                EndTime = "12:30",
                Action = AvailabilityAction.Add,
            };
            tempList.Add(temp1);
            tempList.Add(temp2);

            ListingAvailabilitiesReactDTO tempAddList = new ListingAvailabilitiesReactDTO()
            {
                reactAvailabilities = tempList
            };

            await _listingProfileManager.EditListingAvailabilities(tempAddList).ConfigureAwait(false);
            var getTempIds = await _listingAvailabilitiesDataAccess.GetListingAvailabilities(listingId);

            List<ListingAvailabilityReactDTO> listingAvailabilityReactDTOs = new List<ListingAvailabilityReactDTO>();

            ListingAvailabilityReactDTO avail1 = new ListingAvailabilityReactDTO()
            {
                ListingId = listingId,
                OwnerId = newAccountId,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)).ToString(),
                StartTime = "09:00",
                EndTime = "12:00",
                Action = AvailabilityAction.Add
            };

            ListingAvailabilityReactDTO avail2 = new ListingAvailabilityReactDTO()
            {
                ListingId = listingId,
                OwnerId = newAccountId,
                StartTime = "13:30",
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)).ToString(),
                EndTime = "16:30",
                Action = AvailabilityAction.Add,
            };

            ListingAvailabilityReactDTO avail3 = new ListingAvailabilityReactDTO()
            {
                ListingId = listingId,
                OwnerId = newAccountId,
                AvailabilityId = getTempIds.Payload[0].AvailabilityId,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(5)).ToString(),
                StartTime = "10:00",
                EndTime = "13:30",
                Action = AvailabilityAction.Update,
            };

            ListingAvailabilityReactDTO avail4 = new ListingAvailabilityReactDTO()
            {
                ListingId = listingId,
                OwnerId = newAccountId,
                AvailabilityId = getTempIds.Payload[1].AvailabilityId,
                StartTime = "11:30",
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(3)).ToString(),
                EndTime = "12:30",
                Action = AvailabilityAction.Delete,
            };

            listingAvailabilityReactDTOs.Add(avail1);
            listingAvailabilityReactDTOs.Add(avail2);
            listingAvailabilityReactDTOs.Add(avail3);
            listingAvailabilityReactDTOs.Add(avail4);

            ListingAvailabilitiesReactDTO listingAvailabilityDTOs = new ListingAvailabilitiesReactDTO()
            {
                reactAvailabilities = listingAvailabilityReactDTOs
            };


            //Act
            var actual = await _listingProfileManager.EditListingAvailabilities(listingAvailabilityDTOs).ConfigureAwait(false);

            var getAvailabilities = await _listingAvailabilitiesDataAccess.GetListingAvailabilities(listingId).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(getAvailabilities.Payload.Count == expectedCount);
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

            List<ListingAvailabilityReactDTO> listingAvailabilityReactDTOs = new List<ListingAvailabilityReactDTO>();

            ListingAvailabilityReactDTO avail1 = new ListingAvailabilityReactDTO()
            {
                ListingId = listingId,
                OwnerId = newAccountId,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)).ToString(),
                StartTime = "09:01",
                EndTime = "12:00",
                Action = AvailabilityAction.Add,
            };

            listingAvailabilityReactDTOs.Add(avail1);

            ListingAvailabilitiesReactDTO listingAvailabilityDTOs = new ListingAvailabilitiesReactDTO()
            {
                reactAvailabilities = listingAvailabilityReactDTOs
            };

            //Act
            var actual = await _listingProfileManager.EditListingAvailabilities(listingAvailabilityDTOs).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage.Trim() == expectedErrorMessage);
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

            List<ListingAvailabilityReactDTO> listingAvailabilityReactDTOs = new List<ListingAvailabilityReactDTO>();

            ListingAvailabilityReactDTO avail1 = new ListingAvailabilityReactDTO()
            {
                ListingId = listingId,
                OwnerId = newAccountId,
                Date = DateOnly.FromDateTime(DateTime.Now).ToString(),
                StartTime = "09:00",
                EndTime = "12:20",
                Action = AvailabilityAction.Add,
            };

            listingAvailabilityReactDTOs.Add(avail1);

            ListingAvailabilitiesReactDTO listingAvailabilityDTOs = new ListingAvailabilitiesReactDTO()
            {
                reactAvailabilities = listingAvailabilityReactDTOs
            };

            //Act
            var actual = await _listingProfileManager.EditListingAvailabilities(listingAvailabilityDTOs).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(actual.IsSuccessful == expected);
            Assert.IsTrue(actual.ErrorMessage.Trim() == expectedErrorMessage);
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