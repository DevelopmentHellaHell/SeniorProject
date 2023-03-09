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
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevelopmentHell.Hubba.Testing.Service.Implementations;

namespace DevelopmentHell.Hubba.Authorization.Test
{
    [TestClass]
	public class ServiceIntegrationTests
	{
		// Class to test
		private readonly IAuthorizationService _authorizationService;
		// Helper classes
		private readonly IRegistrationService _registrationService;
		private readonly IUserAccountDataAccess _userAccountDataAccess;
		private readonly TestingService _testingService;

		public ServiceIntegrationTests()
		{
			_userAccountDataAccess = new UserAccountDataAccess(
				ConfigurationManager.AppSettings["UsersConnectionString"]!,
				ConfigurationManager.AppSettings["UserAccountsTable"]!
			);
			ILoggerService loggerService = new LoggerService(
				new LoggerDataAccess(
					ConfigurationManager.AppSettings["LogsConnectionString"]!,
					ConfigurationManager.AppSettings["LogsTable"]!
				)
			);
			_authorizationService = new AuthorizationService(
				ConfigurationManager.AppSettings["JwtKey"]!,
				_userAccountDataAccess,
				loggerService
			);
			_registrationService = new RegistrationService(
				_userAccountDataAccess,
				new CryptographyService(
					ConfigurationManager.AppSettings["CryptographyKey"]!
				),
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
			var actualTokenResult = await _authorizationService.GenerateToken(id, defaultUser).ConfigureAwait(false);
			ClaimsPrincipal? actualPrincipal = null;
			if (actualTokenResult.IsSuccessful)
			{
				_testingService.DecodeJWT(actualTokenResult.Payload!);
				actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
			}

			// Assert
			Assert.IsTrue(expectedResultSuccess == actualTokenResult.IsSuccessful);
			Assert.IsNotNull(actualPrincipal);
			Assert.IsTrue(actualPrincipal.FindFirstValue(ClaimTypes.Email)! == email);
			Assert.IsTrue(actualPrincipal.FindFirstValue(ClaimTypes.Role)! == expectedRole);
			Assert.IsTrue(int.Parse(actualPrincipal.FindFirstValue("accountId")!) == id);

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
			var actualTokenResult = await _authorizationService.GenerateToken(id, true).ConfigureAwait(false);
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