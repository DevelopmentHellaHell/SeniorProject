using DevelopmentHell.Hubba.Authentication.Service.Implementation;
using DevelopmentHell.Hubba.Logging.Service.Implementation;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Manager;
using DevelopmentHell.Hubba.Registration.Service.Implementation;
using DevelopmentHell.Hubba.SqlDataAccess;
using System.Configuration;

namespace DevelopmentHell.Hubba.Authorization.Test
{
	[TestClass]
	public class AuthorizationDataAccessUnitTest
	{
		private string _UsersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
		private string _UserAccountsTable = ConfigurationManager.AppSettings["UserAccounts"]!;
		private string _UserRolesTable = ConfigurationManager.AppSettings["UserRolesTable"]!;

		private string _LogsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
		private string _LogsTable = ConfigurationManager.AppSettings["LogsTable"]!;

		[TestMethod]
		public void ShouldCreateNewInstanceWithParameterCtor()
		{
			// Arrange
			var expected = typeof(AuthorizationDataAccess);

			// Act
			var actual = new AuthorizationDataAccess(_UsersConnectionString, _UserRolesTable);

			// Assert
			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.GetType() == expected);

		}

		[TestMethod]
		public async Task ShouldGiveRoleForUser()
		{
			// Arrange
			var loggerService = new LoggerService(new LoggerDataAccess(_LogsConnectionString, _LogsTable));
			var registrationManager = new RegistrationManager(
				new RegistrationService(
					new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
					loggerService),
				loggerService);
			var authenticationService = new AuthenticationService(
				new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable),
				loggerService);
			Console.WriteLine("TEST" + _UsersConnectionString);
			var authorizationDataAccess = new AuthorizationDataAccess(_UsersConnectionString, _UserRolesTable);

			string dummyEmail = "test@gmail.com";
			string dummyPassword = "12345678";
			string dummyIp = "192.0.2.0";
			Result registerResult = await registrationManager.Register(dummyEmail, dummyPassword).ConfigureAwait(false);
			Result<int> authenticationResult = await authenticationService.AuthenticateCredentials(dummyEmail, dummyPassword, dummyIp);
			int accountId = authenticationResult.Payload;

			// Act
			Result authorizationResult = await authorizationDataAccess.GiveRole(accountId, Role.ADMIN).ConfigureAwait(false);

			// Assert
			Console.WriteLine(authorizationResult.ErrorMessage);
			Assert.IsTrue(authorizationResult.IsSuccessful);

			// Cleanup
			var userAccountDataAccess = new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable);
			Result deleteResult = await userAccountDataAccess.Delete(accountId).ConfigureAwait(false);
			
		}
	}
}