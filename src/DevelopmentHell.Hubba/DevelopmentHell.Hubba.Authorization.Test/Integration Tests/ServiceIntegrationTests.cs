using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using System.Configuration;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using System.Security.Claims;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;

namespace DevelopmentHell.Hubba.Authorization.Test
{
    [TestClass]
	public class ServiceIntegrationTests
	{
        private string _usersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;

        // Class to test
        private readonly IAuthorizationService _authorizationService;
		// Helper classes
		private readonly IRegistrationService _registrationService;
		private readonly IUserAccountDataAccess _userAccountDataAccess;
		private readonly ITestingService _testingService;

		public ServiceIntegrationTests()
		{
			_userAccountDataAccess = new UserAccountDataAccess(
				_usersConnectionString,
				_userAccountsTable
			);
			ILoggerService loggerService = new LoggerService(
				new LoggerDataAccess(
                    _logsConnectionString,
                    _logsTable
                )
			);
			ICryptographyService cryptographyService = new CryptographyService(
				ConfigurationManager.AppSettings["CryptographyKey"]!
			);
			_authorizationService = new AuthorizationService(
				ConfigurationManager.AppSettings["JwtKey"]!,
				_userAccountDataAccess,
				cryptographyService,
				loggerService
			);
			_registrationService = new RegistrationService(
				_userAccountDataAccess,
				cryptographyService,
				new ValidationService(),
				loggerService
			);
			_testingService = new TestingService(
				new TestsDataAccess()
			);
		}

		[TestMethod]
		public void ShouldInstansiateCtor()
		{
			Assert.IsNotNull(_authorizationService);
		}

		[DataTestMethod]
		[DataRow(false, "VerifiedUser")]
		[DataRow(true, "DefaultUser")]
		public async Task ShouldGenerateJwtToken(bool defaultUser, string role)
		{
			// Arrange
			//  - Setup user and initial state
			var email = "test@gmail.com";
			var password = "12345678";
			await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
			var userIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
			var id = userIdResult.Payload;

			var expectedResultSuccess = true;
			var expectedRole = role;

			// Actual
			var actualTokenResult = await _authorizationService.GenerateAccessToken(id, defaultUser).ConfigureAwait(false);
			ClaimsPrincipal? actualPrincipal = null;
			if (actualTokenResult.IsSuccessful)
			{
				_testingService.DecodeJWT(actualTokenResult.Payload!);
				actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
			}

			// Assert
			Assert.IsTrue(expectedResultSuccess == actualTokenResult.IsSuccessful);
			Assert.IsNotNull(actualPrincipal);
			Assert.IsTrue(actualPrincipal.FindFirstValue("azp")! == email);
			Assert.IsTrue(actualPrincipal.FindFirstValue("role")! == expectedRole);
			Assert.IsTrue(int.Parse(actualPrincipal.FindFirstValue("sub")!) == id);

			//  - Cleanup
			await _userAccountDataAccess.Delete(id).ConfigureAwait(false);
		}

		[DataTestMethod]
		[DataRow("AdminUser")]
		[DataRow("DefaultUser", "VerifiedUser")]
		[DataRow("")]
		public async Task ShouldAuthorize(params string[] roles)
		{
			// Arrange
			//  - Setup user and initial state
			var email = "test@gmail.com";
			var password = "12345678";
			await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
			var userIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
			var id = userIdResult.Payload;
			var actualTokenResult = await _authorizationService.GenerateAccessToken(id, true).ConfigureAwait(false);
			if (actualTokenResult.IsSuccessful)
			{
                _testingService.DecodeJWT(actualTokenResult.Payload!);
			}

			var expectedRole = "VerifiedUser";
			var expectedResultSuccess = roles.Contains(expectedRole);

			// Actual
			var actual = _authorizationService.Authorize(roles);

			// Assert
			Assert.IsTrue(expectedResultSuccess == actual.IsSuccessful);

			//  - Cleanup
			await _userAccountDataAccess.Delete(id).ConfigureAwait(false);
		}

		[TestCleanup]
		public async Task Cleanup()
		{
			await _testingService.DeleteAllRecords().ConfigureAwait(false);
		}
	}
}