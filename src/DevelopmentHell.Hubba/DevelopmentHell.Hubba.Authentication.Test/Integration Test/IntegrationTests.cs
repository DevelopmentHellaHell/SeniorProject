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

		[TestMethod]
		public async Task ShouldAuthenticate()
		{
			var otpDataAccess = new OTPDataAccess(_UsersConnectionString, _UserOTPsTable);
			var userAccountDataAccess = new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable);
			// Arrange
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
			string email = "test@gmail.com";
			string password = "12345678";
			string dummyIp = "192.0.2.0";
			Result<int> getExistingAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
			int accountId = getExistingAccountId.Payload;
			if (getExistingAccountId.Payload > 0)
			{
				await otpDataAccess.Delete(accountId).ConfigureAwait(false);
				await userAccountDataAccess.Delete(accountId).ConfigureAwait(false);
			}

			await registrationManager.Register(email, password).ConfigureAwait(false);
			await authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
			Result<int> getNewAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
			int newAccountId = getNewAccountId.Payload;
			Result<byte[]> getOtp = await otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
			string otp = EncryptionService.Decrypt(getOtp.Payload!);

			// Act
			var actual = await authenticationManager.AuthenticateOTP(getNewAccountId.Payload, otp, dummyIp);

			// Assert
			Assert.IsTrue(actual.IsSuccessful);
			Assert.IsTrue(actual.Payload is not null);
			Assert.IsTrue(actual.Payload.IsInRole("VerifiedUser"));
			Assert.IsTrue(actual.Payload.Identity.IsAuthenticated);
			Assert.IsTrue(actual.Payload.Identity.Name == newAccountId.ToString());
		}
	}
}