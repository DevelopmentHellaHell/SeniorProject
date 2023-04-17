using Development.Hubba.JWTHandler.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.AccountRecovery.Manager.Abstractions;
using DevelopmentHell.Hubba.AccountRecovery.Manager.Implementations;
using DevelopmentHell.Hubba.AccountRecovery.Service.Implementations;
using DevelopmentHell.Hubba.Authentication.Manager.Abstractions;
using DevelopmentHell.Hubba.Authentication.Manager.Implementations;
using DevelopmentHell.Hubba.Authentication.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Email.Service.Abstractions;
using DevelopmentHell.Hubba.Email.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Notification.Service.Abstractions;
using DevelopmentHell.Hubba.Notification.Service.Implementations;
using DevelopmentHell.Hubba.OneTimePassword.Service.Abstractions;
using DevelopmentHell.Hubba.OneTimePassword.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Manager.Abstractions;
using DevelopmentHell.Hubba.Registration.Manager.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace DevelopmentHell.Hubba.AccountRecovery.Test.Integration_Tests
{
    [TestClass]
    public class IntegrationTests
    {
        private string _usersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private string _userOTPsTable = ConfigurationManager.AppSettings["UserOTPsTable"]!;
        private string _userLoginsTable = ConfigurationManager.AppSettings["UserLoginsTable"]!;
        private string _recoveryRequestsTable = ConfigurationManager.AppSettings["RecoveryRequestsTable"]!;

        private string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;

        private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;
        private string _cryptographyKey = ConfigurationManager.AppSettings["CryptographyKey"]!;

        // Class to test
        private readonly IAccountRecoveryManager _accountRecoveryManager;

        // Helper classes
        private readonly IRegistrationManager _registrationManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly INotificationService _notificationService;
        private readonly IOTPDataAccess _otpDataAccess;
        private readonly IUserAccountDataAccess _userAccountDataAccess;
        private readonly IUserLoginDataAccess _userLoginDataAccess;
        private readonly IRecoveryRequestDataAccess _recoveryRequestDataAccess;
        private readonly IRegistrationService _registrationService;
        private readonly IOTPService _otpService;
        private readonly ITestingService _testingService;
        private readonly ILoggerService _loggerService;
        private readonly ICryptographyService _cryptographyService;
        private readonly IValidationService _validationService;
        private readonly IEmailService _emailService;

        public IntegrationTests()
        {
            _loggerService = new LoggerService(
                new LoggerDataAccess(
                    _logsConnectionString,
                    _logsTable
                )
            );

            _userAccountDataAccess = new UserAccountDataAccess(
                _usersConnectionString,
                _userAccountsTable
            );

            _userLoginDataAccess = new UserLoginDataAccess(
                _usersConnectionString,
                _userLoginsTable
            );

            _recoveryRequestDataAccess = new RecoveryRequestDataAccess(
                _usersConnectionString,
                _recoveryRequestsTable
            );
            _otpDataAccess = new OTPDataAccess(
                _usersConnectionString,
                _userOTPsTable
            );
            _cryptographyService = new CryptographyService(
                _cryptographyKey
            );
            _emailService = new EmailService(
                    ConfigurationManager.AppSettings["SENDGRID_USERNAME"]!,
                    ConfigurationManager.AppSettings["SENDGRID_API_KEY"]!,
                    ConfigurationManager.AppSettings["COMPANY_EMAIL"]!,
                    false);

            _validationService = new ValidationService();
            _otpService = new OTPService(
                _otpDataAccess,
                new EmailService(
                    ConfigurationManager.AppSettings["SENDGRID_USERNAME"]!,
                    ConfigurationManager.AppSettings["SENDGRID_API_KEY"]!,
                    ConfigurationManager.AppSettings["COMPANY_EMAIL"]!,
                    true
                ),
                _cryptographyService
            );
            IJWTHandlerService jwtHandlerService = new JWTHandlerService(
                _jwtKey
            );

            _authorizationService = new AuthorizationService(
                _userAccountDataAccess,
                jwtHandlerService,
                _loggerService
            );

            _accountRecoveryManager = new AccountRecoveryManager(
                new AccountRecoveryService(
                    _userAccountDataAccess, 
                    _userLoginDataAccess, 
                    _recoveryRequestDataAccess, 
                    _validationService, 
                    _loggerService
                ),
                new OTPService(
                    new OTPDataAccess(
                        _usersConnectionString,
                        _userOTPsTable
                    ),
                    _emailService,
                    _cryptographyService),
                    new AuthenticationService(
                        _userAccountDataAccess, 
                        _userLoginDataAccess, 
                        _cryptographyService, 
                        jwtHandlerService, 
                        _validationService, 
                        _loggerService), 
                    _authorizationService, 
                    _loggerService
                );

            
            
            
            
            _notificationService = new NotificationService(
                    new NotificationDataAccess(
                        ConfigurationManager.AppSettings["NotificationsConnectionString"]!,
                        ConfigurationManager.AppSettings["UserNotificationsTable"]!
                    ),
                    new NotificationSettingsDataAccess(
                        ConfigurationManager.AppSettings["NotificationsConnectionString"]!,
                        ConfigurationManager.AppSettings["NotificationSettingsTable"]!
                    ),
                    _userAccountDataAccess,
                    _loggerService
            );
            _registrationService = new RegistrationService(
                _userAccountDataAccess,
                _cryptographyService,
                _validationService,
                _loggerService
            );
            _registrationManager = new RegistrationManager(
                _registrationService,
                _authorizationService,
                _cryptographyService,
                _notificationService,
                _loggerService
            );
            _testingService = new TestingService(
                _jwtKey,
                new TestsDataAccess()
            );
        }

        [TestInitialize]
        public async Task Setup()
        {
            await _testingService.DeleteAllRecords().ConfigureAwait(false);
        }

        //private void decodeJWT(string token)
        //{

        //    if (token is not null)
        //    {
        //        // Parse the JWT token and extract the principal
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var key = Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["JwtKey"]!);
        //        var validationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuer = false,
        //            ValidateAudience = false,
        //            ValidateLifetime = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(key)
        //        };

        //        try
        //        {
        //            SecurityToken validatedToken;
        //            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

        //            Thread.CurrentPrincipal = principal;
        //            return;
        //        }
        //        catch (Exception)
        //        {
        //            // Handle token validation errors
        //            Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("role", "DefaultUser") }));
        //            return;
        //        }
        //    }
        //}


        /*
		 * Success Case
		 * Goal: Successfully add account to RecoveryRequest Datastore, user does not recevie authorization
		 * Process: Register Account Successfully, Attempt Account Recovery
		 */
        [TestMethod]
        public async Task ManualRecovery()
        {
            // Arrange
            string email = "accountrecovery-manualsuccess@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";

            Result<string> expected = new Result<string>
            {
                IsSuccessful = true,
                Payload = null
            };
            string expectedRole = "DefaultUser";

            //Arrange Continued
            await _registrationManager.Register(email, password).ConfigureAwait(false);

            // Act
            var verificationResult = await _accountRecoveryManager.EmailVerification(email);

            _testingService.DecodeJWT(verificationResult.Payload!.Item1, verificationResult.Payload!.Item2);

            var userIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            var id = userIdResult.Payload;

            //var accessTokenResult = await _authorizationService.GenerateAccessToken(id, false).ConfigureAwait(false);
            //var idTokenResult = _authenticationService.GenerateIdToken(id, accessTokenResult.Payload!);
            //if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
            //{
            //    _testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);

            //}

            //decodeJWT(verificationResult.Payload!);
            //var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            //var stringAccountId = claimsPrincipal?.FindFirstValue("accountId");

            Result<byte[]> getOtp = await _otpDataAccess.GetOTP(id).ConfigureAwait(false);
            string otp = _cryptographyService.Decrypt(getOtp.Payload!);

            await _accountRecoveryManager.AuthenticateOTP(otp, dummyIp);
            var actual = await _accountRecoveryManager.AccountAccess(dummyIp);
            
            
            _testingService.DecodeJWT(actual.Payload!.Item1, actual.Payload!.Item2);

            //Arrange to check recoveryrequests has new request
            var recoveryRequestResult = await _recoveryRequestDataAccess.GetId(id);


            // Assert
            Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
            Assert.IsTrue(actual.Payload is null);
            Assert.IsTrue(Thread.CurrentPrincipal!.IsInRole(expectedRole));
            Assert.IsTrue(recoveryRequestResult.IsSuccessful);
            Assert.IsTrue(recoveryRequestResult.Payload == id);
        }


   //     /*
		 //* Success Case
		 //* Goal: Successfully recover user account,user recevies authorization
		 //* Process: Register Account Successfully, Log into account, Attempt Account Recovery
		 //*/
   //     [TestMethod]
   //     public async Task AutomatedRecovery01()
   //     {
   //         // Arrange
   //         var otpDataAccess = new OTPDataAccess(_UsersConnectionString, _UserOTPsTable);
   //         var userAccountDataAccess = new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable);
   //         var userLoginDataAccess = new UserLoginDataAccess(_UsersConnectionString, _UserLoginsTable);
   //         var recoveryRequestDataAccess = new RecoveryRequestDataAccess(_UsersConnectionString, _RecoveryRequestsTable);
   //         var loggerService = new LoggerService(
   //             new LoggerDataAccess(_LogsConnectionString, _LogsTable)
   //         );
   //         var accountRecoveryManager = new AccountRecoveryManager(
   //             new AccountRecoveryService(
   //                 new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
   //                 loggerService,
   //                 new UserLoginDataAccess(_UsersConnectionString, _UserLoginsTable),
   //                 new RecoveryRequestDataAccess(_UsersConnectionString, _RecoveryRequestsTable)
   //             ),
   //             new OTPService(
   //                 new OTPDataAccess(_UsersConnectionString, _UserOTPsTable)
   //             ),
   //             new AuthenticationService(
   //                 new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
   //                 loggerService
   //             ),
   //             new AuthorizationService(ConfigurationManager.AppSettings,
   //                 new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
   //                 loggerService),
   //             loggerService);
   //         var registrationManager = new RegistrationManager(
   //             new RegistrationService(
   //                 new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
   //                 loggerService
   //             ),
   //             new AuthorizationService(ConfigurationManager.AppSettings,
   //                 new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
   //                 loggerService),
   //             loggerService
   //         );
   //         var authenticationManager = new AuthenticationManager(
   //             new AuthenticationService(
   //                 new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
   //                 loggerService
   //             ),
   //             new OTPService(
   //                 new OTPDataAccess(_UsersConnectionString, _UserOTPsTable)
   //             ),
   //             new AuthorizationService(ConfigurationManager.AppSettings,
   //                 new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
   //                 loggerService),
   //             loggerService
   //         );
   //         string email = "accountrecovery-automatedsuccess01@gmail.com";
   //         string password = "12345678";
   //         string dummyIp = "192.0.2.0";

   //         Result<string> expected = new Result<string>
   //         {
   //             IsSuccessful = true
   //         };
   //         string expectedRole = "VerifiedUser";

   //         //Cleanup
   //         Result<int> getExistingAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
   //         int accountId = getExistingAccountId.Payload;
   //         if (getExistingAccountId.Payload > 0)
   //         {
   //             await otpDataAccess.Delete(accountId).ConfigureAwait(false);
   //             await userAccountDataAccess.Delete(accountId).ConfigureAwait(false);
   //             await userLoginDataAccess.Delete(accountId).ConfigureAwait(false);
   //             await recoveryRequestDataAccess.Delete(accountId).ConfigureAwait(false);
   //         }

   //         //Arrange Continued
   //         await registrationManager.Register(email, password).ConfigureAwait(false);
   //         var loginResult = await authenticationManager.Login(email, password, dummyIp, false).ConfigureAwait(false);
   //         Result<int> getNewAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
   //         int newAccountId = getNewAccountId.Payload;
   //         Result<byte[]> getOtp = await otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
   //         string otp = CryptographyService.Decrypt(getOtp.Payload!);
   //         decodeJWT(loginResult.Payload!);
   //         await authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

   //         //temp
   //         await userLoginDataAccess.AddLogin(newAccountId, dummyIp);
   //         await userLoginDataAccess.AddLogin(newAccountId, "192.0.3.0");
   //         await userLoginDataAccess.AddLogin(newAccountId, "192.0.4.0");


   //         // Act
   //         var verificationResult = await accountRecoveryManager.EmailVerification(email, false);
   //         decodeJWT(verificationResult.Payload!);
   //         var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
   //         var stringAccountId = claimsPrincipal?.FindFirstValue("accountId");

   //         getOtp = await otpDataAccess.GetOTP(int.Parse(stringAccountId!)).ConfigureAwait(false);
   //         otp = CryptographyService.Decrypt(getOtp.Payload!);

   //         await accountRecoveryManager.AuthenticateOTP(otp, dummyIp);
   //         var actual = await accountRecoveryManager.AccountAccess(dummyIp);
   //         decodeJWT(actual.Payload!);


   //         // Assert
   //         Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
   //         Assert.IsTrue(actual.Payload is not null);
   //         Assert.IsTrue(actual.Payload.GetType == expected.Payload!.GetType);
   //         Assert.IsTrue(Thread.CurrentPrincipal!.IsInRole(expectedRole));
   //     }


   //     /*
		 //* Success Case
		 //* Goal: Successfully recover user account,user recevies authorization
		 //* Process: Register Account Successfully, Log into account, Fail 3 logins to disable account, Attempt Account Recovery
		 //*/
   //     [TestMethod]
   //     public async Task AutomatedRecovery02()
   //     {
   //         // Arrange
   //         var otpDataAccess = new OTPDataAccess(_UsersConnectionString, _UserOTPsTable);
   //         var userAccountDataAccess = new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable);
   //         var userLoginDataAccess = new UserLoginDataAccess(_UsersConnectionString, _UserLoginsTable);
   //         var recoveryRequestDataAccess = new RecoveryRequestDataAccess(_UsersConnectionString, _RecoveryRequestsTable);
   //         var loggerService = new LoggerService(
   //             new LoggerDataAccess(_LogsConnectionString, _LogsTable)
   //         );
   //         var accountRecoveryManager = new AccountRecoveryManager(
   //             new AccountRecoveryService(
   //                 new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
   //                 loggerService,
   //                 new UserLoginDataAccess(_UsersConnectionString, _UserLoginsTable),
   //                 new RecoveryRequestDataAccess(_UsersConnectionString, _RecoveryRequestsTable)
   //             ),
   //             new OTPService(
   //                 new OTPDataAccess(_UsersConnectionString, _UserOTPsTable)
   //             ),
   //             new AuthenticationService(
   //                 new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
   //                 loggerService
   //             ),
   //             new AuthorizationService(ConfigurationManager.AppSettings,
   //                 new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
   //                 loggerService),
   //             loggerService);
   //         var registrationManager = new RegistrationManager(
   //             new RegistrationService(
   //                 new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
   //                 loggerService
   //             ),
   //             new AuthorizationService(ConfigurationManager.AppSettings,
   //                 new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
   //                 loggerService),
   //             loggerService
   //         );
   //         var authenticationManager = new AuthenticationManager(
   //             new AuthenticationService(
   //                 new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
   //                 loggerService
   //             ),
   //             new OTPService(
   //                 new OTPDataAccess(_UsersConnectionString, _UserOTPsTable)
   //             ),
   //             new AuthorizationService(ConfigurationManager.AppSettings,
   //                 new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
   //                 loggerService),
   //             loggerService
   //         );
   //         string email = "accountrecovery-automatedsuccess02@gmail.com";
   //         string password = "12345678";
   //         string dummyIp = "192.0.2.0";

   //         //Cleanup
   //         Result<int> getExistingAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
   //         int accountId = getExistingAccountId.Payload;
   //         if (getExistingAccountId.Payload > 0)
   //         {
   //             await otpDataAccess.Delete(accountId).ConfigureAwait(false);
   //             await userAccountDataAccess.Delete(accountId).ConfigureAwait(false);
   //             await userLoginDataAccess.Delete(accountId).ConfigureAwait(false);
   //             await recoveryRequestDataAccess.Delete(accountId).ConfigureAwait(false);
   //         }

   //         Result<string> expected = new Result<string>
   //         {
   //             IsSuccessful = true
   //         };
   //         string expectedRole = "VerifiedUser";

   //         // Act
   //         await registrationManager.Register(email, password).ConfigureAwait(false);
   //         var loginResult = await authenticationManager.Login(email, password, dummyIp, false).ConfigureAwait(false);
   //         Result<int> getNewAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
   //         int newAccountId = getNewAccountId.Payload;
   //         Result<byte[]> getOtp = await otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
   //         string otp = CryptographyService.Decrypt(getOtp.Payload!);
   //         decodeJWT(loginResult.Payload!);
   //         await authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

   //         //temp
   //         await userLoginDataAccess.AddLogin(newAccountId, dummyIp);
   //         await userLoginDataAccess.AddLogin(newAccountId, "192.0.3.0");
   //         await userLoginDataAccess.AddLogin(newAccountId, "192.0.4.0");

   //         //Arrange Continued
   //         string wrongPassword = "whoops";
   //         for (int i = 0; i < 3; i++)
   //         {
   //             await authenticationManager.Login(email, wrongPassword, dummyIp, false);
   //         }
   //         var checkDisabledBefore = await userAccountDataAccess.GetDisabled(newAccountId).ConfigureAwait(false);
   //         var checkLoginAttemptsBefore = await userAccountDataAccess.GetAttempt(newAccountId).ConfigureAwait(false);


   //         // Act
   //         var verificationResult = await accountRecoveryManager.EmailVerification(email, false);
   //         decodeJWT(verificationResult.Payload!);
   //         var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
   //         var stringAccountId = claimsPrincipal?.FindFirstValue("accountId");

   //         getOtp = await otpDataAccess.GetOTP(int.Parse(stringAccountId!)).ConfigureAwait(false);
   //         otp = CryptographyService.Decrypt(getOtp.Payload!);

   //         await accountRecoveryManager.AuthenticateOTP(otp, dummyIp);
   //         var actual = await accountRecoveryManager.AccountAccess(dummyIp);
   //         decodeJWT(actual.Payload!);
   //         var checkDisabledAfter = await userAccountDataAccess.GetDisabled(newAccountId).ConfigureAwait(false);
   //         var checkLoginAttemptsAfter = await userAccountDataAccess.GetAttempt(newAccountId).ConfigureAwait(false);

   //         // Assert
   //         Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
   //         Assert.IsTrue(actual.Payload is not null);
   //         Assert.IsTrue(actual.Payload.GetType == expected.Payload!.GetType);
   //         Assert.IsTrue(checkDisabledBefore.Payload == true);
   //         Assert.IsTrue(checkLoginAttemptsBefore.Payload!.LoginAttempts == 3);
   //         Assert.IsTrue(checkDisabledAfter.Payload == false);
   //         Assert.IsTrue(checkLoginAttemptsAfter.Payload!.LoginAttempts == 0);
   //         Assert.IsTrue(Thread.CurrentPrincipal!.IsInRole(expectedRole));
   //     }


   //     //     /*
   //     //* Failure Case
   //     //* Goal: Successfully add account to RecoveryRequest Datastore, user does not recevie authorization
   //     //* Process: Register Account Successfully, Attempt Account Recovery
   //     //check if verification result is false
   //     //*/
   //     //     [TestMethod]
   //     //     public async Task InvalidUsername()
   //     //     {
   //     //         // Arrange
   //     //         var otpDataAccess = new OTPDataAccess(_UsersConnectionString, _UserOTPsTable);
   //     //         var userAccountDataAccess = new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable);
   //     //         var userLoginDataAccess = new UserLoginDataAccess(_UsersConnectionString, _UserLoginsTable);
   //     //         var recoveryRequestDataAccess = new RecoveryRequestDataAccess(_UsersConnectionString, _RecoveryRequestsTable);
   //     //         var loggerService = new LoggerService(
   //     //             new LoggerDataAccess(_LogsConnectionString, _LogsTable)
   //     //         );
   //     //         var accountRecoveryManager = new AccountRecoveryManager(
   //     //             new AccountRecoveryService(
   //     //                 new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
   //     //                 loggerService,
   //     //                 new UserLoginDataAccess(_UsersConnectionString, _UserLoginsTable),
   //     //                 new RecoveryRequestDataAccess(_UsersConnectionString, _RecoveryRequestsTable)
   //     //             ),
   //     //             new OTPService(
   //     //                 new OTPDataAccess(_UsersConnectionString, _UserOTPsTable)
   //     //             ),
   //     //             new AuthenticationService(
   //     //                 new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
   //     //                 loggerService
   //     //             ),
   //     //             new AuthorizationService(ConfigurationManager.AppSettings,
   //     //                 new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable)
   //     //             ), 
   //     //             loggerService);

   //     //         var registrationManager = new RegistrationManager(
   //     //             new RegistrationService(
   //     //                 new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
   //     //                 loggerService
   //     //             ),
   //     //             loggerService
   //     //         );
   //     //         string email = "accountrecovery-manualsuccess@gmail.com";
   //     //         string password = "12345678";
   //     //         string dummyIp = "192.0.2.0";

   //     //         //Cleanup
   //     //         Result<int> getExistingAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
   //     //         int accountId = getExistingAccountId.Payload;
   //     //         if (getExistingAccountId.Payload > 0)
   //     //         {
   //     //             await otpDataAccess.Delete(accountId).ConfigureAwait(false);
   //     //             await userAccountDataAccess.Delete(accountId).ConfigureAwait(false);
   //     //             await userLoginDataAccess.Delete(accountId).ConfigureAwait(false);
   //     //             await recoveryRequestDataAccess.Delete(accountId).ConfigureAwait(false);
   //     //         }

   //     //         //Arrange Continued
   //     //         await registrationManager.Register(email, password).ConfigureAwait(false);
   //     //         var expected = new Result<GenericPrincipal>()
   //     //         {
   //     //             IsSuccessful = true,
   //     //         };

   //     //         // Act
   //     //         var verificationResult = await accountRecoveryManager.EmailVerification(email, null, false);
   //     //         Result<byte[]> getOtp = await otpDataAccess.GetOTP(verificationResult.Payload).ConfigureAwait(false);
   //     //         string otp = EncryptionService.Decrypt(getOtp.Payload!);
   //     //         await accountRecoveryManager.AuthenticateOTP(verificationResult.Payload, otp, dummyIp);
   //     //         var actual = await accountRecoveryManager.AccountAccess(verificationResult.Payload, dummyIp);

   //     //         //Arrange to check recoveryrequests has new request
   //     //         var recoveryRequestResult = await recoveryRequestDataAccess.GetId(verificationResult.Payload);


   //     //         // Assert
   //     //         Console.WriteLine(verificationResult.IsSuccessful);
   //     //         Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
   //     //         Assert.IsTrue(recoveryRequestResult.IsSuccessful);
   //     //         Assert.IsTrue(recoveryRequestResult.Payload == verificationResult.Payload);
   //     //     }
    }
} //"Invalid email or OTP provided. Retry again or contact system admin"
