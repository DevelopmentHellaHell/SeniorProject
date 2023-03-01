using DevelopmentHell.Hubba.Authentication.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Implementation;
using DevelopmentHell.Hubba.Cryptography.Service;
using DevelopmentHell.Hubba.Logging.Service.Implementation;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword.Service.Implementation;
using DevelopmentHell.Hubba.SqlDataAccess;
using System.Configuration;
using System.Security.Principal;
using DevelopmentHell.Hubba.AccountRecovery.Service.Implementation;
using DevelopmentHell.Hubba.AccountRecovery.Manager.Implementation;
using DevelopmentHell.Hubba.Registration.Manager.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Implementation;
using DevelopmentHell.Hubba.Authentication.Manager.Implementations;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace DevelopmentHell.Hubba.AccountRecovery.Test.Integration_Tests
{
    [TestClass]
    public class IntegrationTests
    {
        private string _UsersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private string _UserAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private string _UserOTPsTable = ConfigurationManager.AppSettings["UserOTPsTable"]!;
        private string _UserLoginsTable = ConfigurationManager.AppSettings["UserLoginsTable"]!;
        private string _RecoveryRequestsTable = ConfigurationManager.AppSettings["RecoveryRequestsTable"]!;

        private string _LogsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private string _LogsTable = ConfigurationManager.AppSettings["LogsTable"]!;

        private void decodeJWT(string token)
        {

            if (token is not null)
            {
                // Parse the JWT token and extract the principal
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["JwtKey"]!);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                try
                {
                    SecurityToken validatedToken;
                    var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                    Thread.CurrentPrincipal = principal;
                    return;
                }
                catch (Exception)
                {
                    // Handle token validation errors
                    Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "DefaultUser") }));
                    return;
                }
            }
        }


        /*
		 * Success Case
		 * Goal: Successfully add account to RecoveryRequest Datastore, user does not recevie authorization
		 * Process: Register Account Successfully, Attempt Account Recovery
		 */
        //[TestMethod]
        //public async Task ManualRecovery()
        //{
        //    // Arrange
        //    var otpDataAccess = new OTPDataAccess(_UsersConnectionString, _UserOTPsTable);
        //    var userAccountDataAccess = new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable);
        //    var userLoginDataAccess = new UserLoginDataAccess(_UsersConnectionString, _UserLoginsTable);
        //    var recoveryRequestDataAccess = new RecoveryRequestDataAccess(_UsersConnectionString, _RecoveryRequestsTable);
        //    var loggerService = new LoggerService(
        //        new LoggerDataAccess(_LogsConnectionString, _LogsTable)
        //    );
        //    var accountRecoveryManager = new AccountRecoveryManager(
        //        new AccountRecoveryService(
        //            new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
        //            loggerService,
        //            new UserLoginDataAccess(_UsersConnectionString, _UserLoginsTable),
        //            new RecoveryRequestDataAccess(_UsersConnectionString, _RecoveryRequestsTable)
        //        ),
        //        new OTPService(
        //            new OTPDataAccess(_UsersConnectionString, _UserOTPsTable)
        //        ),
        //        new AuthenticationService(
        //            new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
        //            loggerService
        //        ),
        //        new AuthorizationService(ConfigurationManager.AppSettings,
        //            new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
        //            loggerService),
        //        loggerService);
        //    var registrationManager = new RegistrationManager(
        //        new RegistrationService(
        //            new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
        //            loggerService
        //        ),
        //        loggerService
        //    );
        //    string email = "accountrecovery-manualsuccess@gmail.com";
        //    string password = "12345678";
        //    string dummyIp = "192.0.2.0";

        //    //Cleanup
        //    Result<int> getExistingAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
        //    int accountId = getExistingAccountId.Payload;
        //    if (getExistingAccountId.Payload > 0)
        //    {
        //        await otpDataAccess.Delete(accountId).ConfigureAwait(false);
        //        await userAccountDataAccess.Delete(accountId).ConfigureAwait(false);
        //        await userLoginDataAccess.Delete(accountId).ConfigureAwait(false);
        //        await recoveryRequestDataAccess.Delete(accountId).ConfigureAwait(false);
        //    }

        //    //Arrange Continued
        //    await registrationManager.Register(email, password).ConfigureAwait(false);
        //    var expected = new Result<GenericPrincipal>()
        //    {
        //        IsSuccessful = true,
        //    };

        //    // Act
        //    var verificationResult = await accountRecoveryManager.EmailVerification(email, false);
        //    Result<byte[]> getOtp = await otpDataAccess.GetOTP(verificationResult.Payload).ConfigureAwait(false);
        //    string otp = EncryptionService.Decrypt(getOtp.Payload!);
        //    await accountRecoveryManager.AuthenticateOTP(verificationResult.Payload, otp, dummyIp);
        //    var actual = await accountRecoveryManager.AccountAccess(verificationResult.Payload, dummyIp);

        //    //Arrange to check recoveryrequests has new request
        //    var recoveryRequestResult = await recoveryRequestDataAccess.GetId(verificationResult.Payload);


        //    // Assert
        //    Console.WriteLine(verificationResult.IsSuccessful);
        //    Assert.IsTrue( actual.IsSuccessful == expected.IsSuccessful);
        //    Assert.IsTrue(recoveryRequestResult.IsSuccessful);
        //    Assert.IsTrue(recoveryRequestResult.Payload == verificationResult.Payload );
        //}


        /*
		 * Success Case
		 * Goal: Successfully recover user account,user recevies authorization
		 * Process: Register Account Successfully, Log into account, Attempt Account Recovery
		 */
        [TestMethod]
        public async Task AutomatedRecovery01()
        {
            // Arrange
            var otpDataAccess = new OTPDataAccess(_UsersConnectionString, _UserOTPsTable);
            var userAccountDataAccess = new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable);
            var userLoginDataAccess = new UserLoginDataAccess(_UsersConnectionString, _UserLoginsTable);
            var recoveryRequestDataAccess = new RecoveryRequestDataAccess(_UsersConnectionString, _RecoveryRequestsTable);
            var loggerService = new LoggerService(
                new LoggerDataAccess(_LogsConnectionString, _LogsTable)
            );
            var accountRecoveryManager = new AccountRecoveryManager(
                new AccountRecoveryService(
                    new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
                    loggerService,
                    new UserLoginDataAccess(_UsersConnectionString, _UserLoginsTable),
                    new RecoveryRequestDataAccess(_UsersConnectionString, _RecoveryRequestsTable)
                ),
                new OTPService(
                    new OTPDataAccess(_UsersConnectionString, _UserOTPsTable)
                ),
                new AuthenticationService(
                    new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
                    loggerService
                ),
                new AuthorizationService(ConfigurationManager.AppSettings,
                    new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
                    loggerService),
                loggerService);
            var registrationManager = new RegistrationManager(
                new RegistrationService(
                    new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
                    loggerService
                ), 
                new AuthorizationService(ConfigurationManager.AppSettings,
                    new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
                    loggerService),
                loggerService
            );
            var authenticationManager = new AuthenticationManager(
                new AuthenticationService(
                    new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
                    loggerService
                ),
                new OTPService(
                    new OTPDataAccess(_UsersConnectionString, _UserOTPsTable)
                ),
                new AuthorizationService(ConfigurationManager.AppSettings,
                    new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
                    loggerService),
                loggerService
            );
            string email = "accountrecovery-automatedsuccess01@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";
            string expectedRole = "VerifiedUser";

            //Cleanup
            Result<int> getExistingAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = getExistingAccountId.Payload;
            if (getExistingAccountId.Payload > 0)
            {
                await otpDataAccess.Delete(accountId).ConfigureAwait(false);
                await userAccountDataAccess.Delete(accountId).ConfigureAwait(false);
                await userLoginDataAccess.Delete(accountId).ConfigureAwait(false);
                await recoveryRequestDataAccess.Delete(accountId).ConfigureAwait(false);
            }

            //Arrange Continued
            await registrationManager.Register(email, password).ConfigureAwait(false);
            var loginResult = await authenticationManager.Login(email, password, dummyIp, false).ConfigureAwait(false);
            Result<int> getNewAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = EncryptionService.Decrypt(getOtp.Payload!);
            decodeJWT(loginResult.Payload!);
            await authenticationManager.AuthenticateOTP(otp, dummyIp).ConfigureAwait(false);

            //temp
            await userLoginDataAccess.AddLogin(newAccountId, dummyIp);
            await userLoginDataAccess.AddLogin(newAccountId, "192.0.3.0");
            await userLoginDataAccess.AddLogin(newAccountId, "192.0.4.0");


            // Act
            var verificationResult = await accountRecoveryManager.EmailVerification(email, false);
            decodeJWT(verificationResult.Payload!);
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("accountId");

            getOtp = await otpDataAccess.GetOTP(int.Parse(stringAccountId!)).ConfigureAwait(false);
            otp = EncryptionService.Decrypt(getOtp.Payload!);

            await accountRecoveryManager.AuthenticateOTP(otp, dummyIp);
            var actual = await accountRecoveryManager.AccountAccess(dummyIp);
            decodeJWT(actual.Payload!);


            // Assert
            Assert.IsTrue(actual.IsSuccessful);
            Assert.IsTrue(actual.Payload is not null);
            Assert.IsTrue(Thread.CurrentPrincipal!.IsInRole(expectedRole));
        }


        /*
		 * Success Case
		 * Goal: Successfully recover user account,user recevies authorization
		 * Process: Register Account Successfully, Log into account, Fail 3 logins to disable account, Attempt Account Recovery
		 */
        //[TestMethod]
        //public async Task AutomatedRecovery02()
        //{
        //    // Arrange
        //    var otpDataAccess = new OTPDataAccess(_UsersConnectionString, _UserOTPsTable);
        //    var userAccountDataAccess = new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable);
        //    var userLoginDataAccess = new UserLoginDataAccess(_UsersConnectionString, _UserLoginsTable);
        //    var recoveryRequestDataAccess = new RecoveryRequestDataAccess(_UsersConnectionString, _RecoveryRequestsTable);
        //    var loggerService = new LoggerService(
        //        new LoggerDataAccess(_LogsConnectionString, _LogsTable)
        //    );
        //    var accountRecoveryManager = new AccountRecoveryManager(
        //        new AccountRecoveryService(
        //            new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
        //            loggerService,
        //            new UserLoginDataAccess(_UsersConnectionString, _UserLoginsTable),
        //            new RecoveryRequestDataAccess(_UsersConnectionString, _RecoveryRequestsTable)
        //        ),
        //        new OTPService(
        //            new OTPDataAccess(_UsersConnectionString, _UserOTPsTable)
        //        ),
        //        new AuthenticationService(
        //            new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
        //            loggerService
        //        ),
        //        new AuthorizationService(ConfigurationManager.AppSettings,
        //            new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
        //            loggerService),
        //        loggerService);
        //    var registrationManager = new RegistrationManager(
        //        new RegistrationService(
        //            new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
        //            loggerService
        //        ),
        //        loggerService
        //    );
        //    var authenticationManager = new AuthenticationManager(
        //        new AuthenticationService(
        //            new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
        //            loggerService
        //        ),
        //        new OTPService(
        //            new OTPDataAccess(_UsersConnectionString, _UserOTPsTable)
        //        ),
        //        new AuthorizationService(ConfigurationManager.AppSettings,
        //            new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
        //            loggerService),
        //        loggerService
        //    );
        //    string email = "accountrecovery-automatedsuccess02@gmail.com";
        //    string password = "12345678";
        //    string dummyIp = "192.0.2.0";

        //    //Cleanup
        //    Result<int> getExistingAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
        //    int accountId = getExistingAccountId.Payload;
        //    if (getExistingAccountId.Payload > 0)
        //    {
        //        await otpDataAccess.Delete(accountId).ConfigureAwait(false);
        //        await userAccountDataAccess.Delete(accountId).ConfigureAwait(false);
        //        await userLoginDataAccess.Delete(accountId).ConfigureAwait(false);
        //        await recoveryRequestDataAccess.Delete(accountId).ConfigureAwait(false);
        //    }

        //    //Arrange Continued
        //    await registrationManager.Register(email, password).ConfigureAwait(false);
        //    await authenticationManager.Login(email, password, dummyIp, false).ConfigureAwait(false);
        //    Result<int> getNewAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
        //    int newAccountId = getNewAccountId.Payload;
        //    Result<byte[]> getOtp = await otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
        //    string otp = EncryptionService.Decrypt(getOtp.Payload!);
        //    await authenticationManager.AuthenticateOTP(newAccountId, otp, dummyIp).ConfigureAwait(false);
        //    //temp
        //    await userLoginDataAccess.AddLogin(newAccountId, dummyIp);

        //    //Arrange Continued
        //    string wrongPassword = "whoops";
        //    for (int i = 0; i < 3; i++)
        //    {
        //        await authenticationManager.Login(email, wrongPassword, dummyIp, false);
        //    }
        //    var checkDisabledBefore = await userAccountDataAccess.GetDisabled(newAccountId).ConfigureAwait(false);
        //    var checkLoginAttemptsBefore = await userAccountDataAccess.GetAttempt(newAccountId).ConfigureAwait(false);

        //    string expectedRole = "VerifiedUser";
        //    var expectedIdentity = new GenericIdentity(newAccountId.ToString());
        //    var expectedPrincipal = new GenericPrincipal(expectedIdentity, new string[] { expectedRole });
        //    var expected = new Result<GenericPrincipal>()
        //    {
        //        IsSuccessful = true,
        //        Payload = expectedPrincipal
        //    };

        //    // Act
        //    var verificationResult = await accountRecoveryManager.EmailVerification(email, null, false);
        //    getOtp = await otpDataAccess.GetOTP(verificationResult.Payload).ConfigureAwait(false);
        //    otp = EncryptionService.Decrypt(getOtp.Payload!);
        //    await accountRecoveryManager.AuthenticateOTP(verificationResult.Payload, otp, dummyIp);
        //    var actual = await accountRecoveryManager.AccountAccess(verificationResult.Payload, dummyIp);
        //    var checkDisabledAfter = await userAccountDataAccess.GetDisabled(newAccountId).ConfigureAwait(false);
        //    var checkLoginAttemptsAfter = await userAccountDataAccess.GetAttempt(newAccountId).ConfigureAwait(false);

        //    // Assert
        //    Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
        //    Assert.IsTrue(actual.Payload is not null);
        //    //Assert.IsTrue(actual.Payload.IsInRole(expectedRole));
        //    //Assert.IsTrue(actual.Payload.Identity.IsAuthenticated);
        //    //Assert.IsTrue(actual.Payload.Identity.Name == expected.Payload.Identity.Name);
        //    Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
        //    Assert.IsTrue(checkDisabledBefore.Payload == true);
        //    Assert.IsTrue(checkDisabledAfter.Payload == false);
        //    Assert.IsTrue(checkLoginAttemptsBefore.Payload!.LoginAttempts == 3);
        //    Assert.IsTrue(checkLoginAttemptsAfter.Payload!.LoginAttempts == 0);
        //}


   //     /*
		 //* Failure Case
		 //* Goal: Successfully add account to RecoveryRequest Datastore, user does not recevie authorization
		 //* Process: Register Account Successfully, Attempt Account Recovery
         //check if verification result is false
		 //*/
   //     [TestMethod]
   //     public async Task InvalidUsername()
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
   //                 new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable)
   //             ), 
   //             loggerService);

   //         var registrationManager = new RegistrationManager(
   //             new RegistrationService(
   //                 new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
   //                 loggerService
   //             ),
   //             loggerService
   //         );
   //         string email = "accountrecovery-manualsuccess@gmail.com";
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

   //         //Arrange Continued
   //         await registrationManager.Register(email, password).ConfigureAwait(false);
   //         var expected = new Result<GenericPrincipal>()
   //         {
   //             IsSuccessful = true,
   //         };

   //         // Act
   //         var verificationResult = await accountRecoveryManager.EmailVerification(email, null, false);
   //         Result<byte[]> getOtp = await otpDataAccess.GetOTP(verificationResult.Payload).ConfigureAwait(false);
   //         string otp = EncryptionService.Decrypt(getOtp.Payload!);
   //         await accountRecoveryManager.AuthenticateOTP(verificationResult.Payload, otp, dummyIp);
   //         var actual = await accountRecoveryManager.AccountAccess(verificationResult.Payload, dummyIp);

   //         //Arrange to check recoveryrequests has new request
   //         var recoveryRequestResult = await recoveryRequestDataAccess.GetId(verificationResult.Payload);


   //         // Assert
   //         Console.WriteLine(verificationResult.IsSuccessful);
   //         Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
   //         Assert.IsTrue(recoveryRequestResult.IsSuccessful);
   //         Assert.IsTrue(recoveryRequestResult.Payload == verificationResult.Payload);
   //     }
    }
} //"Invalid username or OTP provided. Retry again or contact system admin"
