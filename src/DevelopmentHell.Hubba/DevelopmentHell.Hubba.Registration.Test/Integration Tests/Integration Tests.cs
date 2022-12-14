using DevelopmentHell.Hubba.Logging.Service.Implementation;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Manager;
using DevelopmentHell.Hubba.Registration.Service.Implementation;
using DevelopmentHell.Hubba.SqlDataAccess;
using System.Configuration;

namespace DevelopmentHell.Hubba.Registration.Test
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
		public async Task RegisteredAccount()
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
			string email = "test@gmail.com";
			string password = "12345678";
			Result<int> existingAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
			int accountId = existingAccountId.Payload;
			if (existingAccountId.Payload > 0)
			{
				await otpDataAccess.Delete(accountId).ConfigureAwait(false);
				await userAccountDataAccess.Delete(accountId).ConfigureAwait(false);
			}

			// Act
			var actual = await registrationManager.Register(email, password).ConfigureAwait(false);

			// Assert
			Assert.IsTrue(actual.IsSuccessful);
		}
	}
}