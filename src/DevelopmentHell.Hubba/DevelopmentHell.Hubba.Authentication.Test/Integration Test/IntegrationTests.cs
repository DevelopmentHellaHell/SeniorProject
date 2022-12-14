using DevelopmentHell.Hubba.Authentication.Manager;
using DevelopmentHell.Hubba.Authentication.Service.Implementation;
using DevelopmentHell.Hubba.Cryptography.Service;
using DevelopmentHell.Hubba.Logging.Service.Implementation;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword.Service.Implementation;
using DevelopmentHell.Hubba.Registration.Manager;
using DevelopmentHell.Hubba.Registration.Service.Implementation;
using DevelopmentHell.Hubba.SqlDataAccess;
using System.Configuration;
using System.Security.Principal;

namespace DevelopmentHell.Hubba.Authentication.Test
{
	[TestClass]
	public class IntegrationTests
	{
		private string _UsersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
		private string _UserAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
		private string _UserOTPsTable = ConfigurationManager.AppSettings["UserOTPsTable"]!;

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
            var loggerService = new LoggerService(
				new LoggerDataAccess(_LogsConnectionString, _LogsTable)
			);
			var registrationManager = new RegistrationManager(
				new RegistrationService(
					new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
					loggerService
				),
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
				loggerService
			);
			string email = "authentication-test01@gmail.com";
			string password = "12345678";
			string dummyIp = "192.0.2.0";

			//Cleanup
			Result<int> getExistingAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
			int accountId = getExistingAccountId.Payload;
			if (getExistingAccountId.Payload > 0)
			{
				await otpDataAccess.Delete(accountId).ConfigureAwait(false);
				await userAccountDataAccess.Delete(accountId).ConfigureAwait(false);
			}

			//Arrange Continued
			await registrationManager.Register(email, password).ConfigureAwait(false);
            await authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<int> getNewAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
			int newAccountId = getNewAccountId.Payload;
            Result<byte[]> getOtp = await otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = EncryptionService.Decrypt(getOtp.Payload!);
            string expectedRole = "VerifiedUser";
            var expectedIdentity = new GenericIdentity(newAccountId.ToString());
            var expectedPrincipal = new GenericPrincipal(expectedIdentity, new string[] { expectedRole });
			var expected = new Result<GenericPrincipal>()
			{
				IsSuccessful = true,
				Payload = expectedPrincipal
            };

            // Act
            
            
            var actual = await authenticationManager.AuthenticateOTP(getNewAccountId.Payload, otp, dummyIp).ConfigureAwait(false);

			// Assert
			Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
			Assert.IsTrue(actual.Payload is not null);
			Assert.IsTrue(actual.Payload.IsInRole(expectedRole));
			Assert.IsTrue(actual.Payload.Identity.IsAuthenticated);
			Assert.IsTrue(actual.Payload.Identity.Name == expected.Payload.Identity.Name);
		}

        /*
		 * Success Case
		 * Goal: Prevent authenticated user from reaching login view
		 * Process: Register Account Successfully, Authenticate credentials successfully, Authenticate OTP successfully, Attempt to reach login view
		 */
        [TestMethod]
        public async Task Test02()
        {
            // Arrange
            var otpDataAccess = new OTPDataAccess(_UsersConnectionString, _UserOTPsTable);
            var userAccountDataAccess = new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable);
            var loggerService = new LoggerService(
                new LoggerDataAccess(_LogsConnectionString, _LogsTable)
            );
            var registrationManager = new RegistrationManager(
                new RegistrationService(
                    new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
                    loggerService
                ),
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
                loggerService
            );
            string email = "authentication-test02@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";

            //Cleanup
            Result<int> getExistingAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = getExistingAccountId.Payload;
            if (getExistingAccountId.Payload > 0)
            {
                await otpDataAccess.Delete(accountId).ConfigureAwait(false);
                await userAccountDataAccess.Delete(accountId).ConfigureAwait(false);
            }

            //Arrange Continued
            await registrationManager.Register(email, password).ConfigureAwait(false);
            Result<int> getNewAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int newAccountId = getNewAccountId.Payload;
            string expectedRole = "VerifiedUser";
            var expectedIdentity = new GenericIdentity(newAccountId.ToString());
            var expectedPrincipal = new GenericPrincipal(expectedIdentity, new string[] { expectedRole });
            var expected = new Result<GenericPrincipal>()
            {
                IsSuccessful = false,
                ErrorMessage = "Error, user already logged in."
            };

            // Act
            await authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
            Result<byte[]> getOtp = await otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
            string otp = EncryptionService.Decrypt(getOtp.Payload!);
            Result<GenericPrincipal> actualPrincipal = await authenticationManager.AuthenticateOTP(getNewAccountId.Payload, otp, dummyIp).ConfigureAwait(false);
            var actual = await authenticationManager.Login(email, password, dummyIp, actualPrincipal.Payload);



            // Assert
            Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
            Assert.IsTrue(actual.ErrorMessage == expected.ErrorMessage);
        }

        /* 
		 * Failure Case
		 * Goal: Prevent log in using invalid email with invalid special character
		 * Process: Attempt to log in using valid password and invalid email with an invalid special character
		 */
        [TestMethod]
        public async Task Test03()
        {
            // Arrange
            var otpDataAccess = new OTPDataAccess(_UsersConnectionString, _UserOTPsTable);
            var userAccountDataAccess = new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable);
            var loggerService = new LoggerService(
                new LoggerDataAccess(_LogsConnectionString, _LogsTable)
            );
            var registrationManager = new RegistrationManager(
                new RegistrationService(
                    new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
                    loggerService
                ),
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
                loggerService
            );
            string email = "authentication$test03@gmail.com";
            string password = "12345678";
            string dummyIp = "192.0.2.0";

            //Cleanup
            Result<int> getExistingAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = getExistingAccountId.Payload;
            if (getExistingAccountId.Payload > 0)
            {
                await otpDataAccess.Delete(accountId).ConfigureAwait(false);
                await userAccountDataAccess.Delete(accountId).ConfigureAwait(false);
            }

            //Arrange Continued
            var expected = new Result()
            {
                IsSuccessful = false,
                ErrorMessage = "Invalid username or password provided. Retry again or contact system admin"
        };

            // Act
            var actual = await authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
            Assert.IsTrue(actual.ErrorMessage == expected.ErrorMessage);
        }



        /* 
		 * Failure Case
		 * Goal: Disable account after three failed login attempts within 24 hours 
		 * Process: Attempt to log in using a valid email with an account and invalid password three different times
		 */
        [TestMethod]
        public async Task Test04()
        {
            // Arrange
            var otpDataAccess = new OTPDataAccess(_UsersConnectionString, _UserOTPsTable);
            var userAccountDataAccess = new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable);
            var loggerService = new LoggerService(
                new LoggerDataAccess(_LogsConnectionString, _LogsTable)
            );
            var registrationManager = new RegistrationManager(
                new RegistrationService(
                    new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
                    loggerService
                ),
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
                loggerService
            );
            string email = "authentication-test04@gmail.com";
            string realPassword = "12345678";
            string password1 = "lol";
            string password2 = "$SELECT * FROM UserAccounts";
            string password3 = "blatant failure";
            string password4 = "12345678";
            string dummyIp = "192.0.2.0";


            //Cleanup
            Result<int> getExistingAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
            int accountId = getExistingAccountId.Payload;
            if (getExistingAccountId.Payload > 0)
            {
                await otpDataAccess.Delete(accountId).ConfigureAwait(false);
                await userAccountDataAccess.Delete(accountId).ConfigureAwait(false);
            }

            //Arrange Continued
            await registrationManager.Register(email, realPassword).ConfigureAwait(false);

            //Arrange Continued
            var expectedInvalidCredentials = new Result()
            {
                IsSuccessful = false,
                ErrorMessage = "Invalid username or password provided. Retry again or contact system admin"
            };
            var expectedDisabled = new Result()
            {
                IsSuccessful = false,
                ErrorMessage = "Account disabled. Perform account recovery or contact system admin."
            };

            // Act
            await authenticationManager.Login(email, password1, dummyIp).ConfigureAwait(false);
            var actualInvalidCredentials = await authenticationManager.Login(email, password2, dummyIp).ConfigureAwait(false);
            await authenticationManager.Login(email, password3, dummyIp).ConfigureAwait(false);
            var actualDisabled = await authenticationManager.Login(email, password4, dummyIp).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(actualInvalidCredentials.IsSuccessful == expectedInvalidCredentials.IsSuccessful);
            Assert.IsTrue(actualInvalidCredentials.ErrorMessage == expectedInvalidCredentials.ErrorMessage);
            Assert.IsTrue(actualDisabled.IsSuccessful == expectedDisabled.IsSuccessful);
            Assert.IsTrue(actualDisabled.ErrorMessage == expectedDisabled.ErrorMessage);
        }
    }
}