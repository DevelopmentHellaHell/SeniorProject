using DevelopmentHell.Hubba.Authentication.Service.Implementation;
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
using DevelopmentHell.Hubba.Logging.Service.Abstractions;

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

        /*
		 * Success Case
		 * Goal: Successfully login, Thread is updated with VerifiedUser principal
		 * Process: Register Account Successfully, Authenticate credentials successfully, Authenticate OTP successfully
		 */
        [TestMethod]
        public async Task Test01()
        {
            // Arrange
            var otpDataAccess = new OTPDataAccess(_UsersConnectionString, _UserOTPsTable);
            var userAccountDataAccess = new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable);
            var userLoginDataAccess = new UserLoginDataAccess(_UsersConnectionString, _UserLoginsTable);
            var recoveryRequestDataAccess = new RecoveryRequestDataAccess(_UsersConnectionString, _RecoveryRequestsTable);
            var loggerService = new LoggerService(
                new LoggerDataAccess(_LogsConnectionString, _LogsTable)
            );
            //IAccountRecoveryService accountRecoveryService, IOTPService otpService, IAuthenticationService authenticationService, IAuthorizationService authorizationService, ILoggerService loggerService
            var accountRecoveryManager = new AccountRecoveryManager(
                //IUserAccountDataAccess userAccountDao, ILoggerService loggerService, IUserLoginDataAccess userLoginDao, IRecoveryRequestDataAccess recoveryRequestDao
                new AccountRecoveryService(
                    new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
                    loggerService,
                    new UserLoginDataAccess(_UsersConnectionString, _UserLoginsTable),
                    new RecoveryRequestDataAccess( _UsersConnectionString, _RecoveryRequestsTable)
                ),
                new OTPService(
                    new OTPDataAccess(_UsersConnectionString, _UserOTPsTable)
                ),
                new AuthenticationService(
                    new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
                    loggerService
                ),
                new AuthorizationService(),
                loggerService
            );

            new AuthenticationService(
                    new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
                    loggerService
                );

            string email = "authentication-test01@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";

            //Cleanup
            //Result<int> getExistingAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
            //int accountId = getExistingAccountId.Payload;
            //if (getExistingAccountId.Payload > 0)
            //{
            //    await otpDataAccess.Delete(accountId).ConfigureAwait(false);
            //    await userAccountDataAccess.Delete(accountId).ConfigureAwait(false);
            //}

            //Arrange Continued


            //Result<int> getNewAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
            //int newAccountId = getNewAccountId.Payload;
            //Result<byte[]> getOtp = await otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            //string otp = EncryptionService.Decrypt(getOtp.Payload!);
            //string expectedRole = null;
            //var expectedIdentity = new GenericIdentity(newAccountId.ToString());
            //var expectedPrincipal = new GenericPrincipal(expectedIdentity, new string[] { expectedRole });
            //var expected = new Result<GenericPrincipal>()
            //{
            //    IsSuccessful = true,
            //    Payload = expectedPrincipal
            //};
            var expected = new Result<GenericPrincipal>()
            {
                IsSuccessful = true,
            };

            // Act
            //var actual = await authenticationManager.AuthenticateOTP(getNewAccountId.Payload, otp, dummyIp).ConfigureAwait(false);
            var verificationResult = await accountRecoveryManager.Verification(email);
            Result<byte[]> getOtp = await otpDataAccess.GetOTP(verificationResult.Payload).ConfigureAwait(false);
            string otp = EncryptionService.Decrypt(getOtp.Payload!);
            await accountRecoveryManager.AuthenticateOTP(verificationResult.Payload, otp, dummyIp);
            var actual = await accountRecoveryManager.AccountAccess(verificationResult.Payload, dummyIp);

            // Assert
            //Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
            //Assert.IsTrue(actual.Payload is not null);
            //Assert.IsTrue(actual.Payload.IsInRole(expectedRole));
            //Assert.IsTrue(actual.Payload.Identity.IsAuthenticated);
            //Assert.IsTrue(actual.Payload.Identity.Name == expected.Payload.Identity.Name);
            Console.WriteLine(verificationResult.IsSuccessful);
            Assert.IsTrue( actual.IsSuccessful == expected.IsSuccessful);
        }
    }
}
