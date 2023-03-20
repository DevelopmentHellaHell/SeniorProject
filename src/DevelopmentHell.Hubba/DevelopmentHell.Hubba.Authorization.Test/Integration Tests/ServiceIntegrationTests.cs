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
using Development.Hubba.JWTHandler.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.Authentication.Service.Implementations;
using DevelopmentHell.Hubba.Authentication.Service.Abstractions;

namespace DevelopmentHell.Hubba.Authorization.Test
{
    [TestClass]
	public class ServiceIntegrationTests
	{
        private string _usersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
		private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;

		// Class to test
		private readonly IAuthorizationService _authorizationService;
		// Helper classes
		private readonly IRegistrationService _registrationService;
		private readonly IUserAccountDataAccess _userAccountDataAccess;
		private readonly IAuthenticationService _authenticationService;
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
			IJWTHandlerService jwtHandlerService = new JWTHandlerService(
				_jwtKey
			);

			_authorizationService = new AuthorizationService(
				_userAccountDataAccess,
				jwtHandlerService,
				loggerService
			);
			_registrationService = new RegistrationService(
				_userAccountDataAccess,
				cryptographyService,
				new ValidationService(),
				loggerService
			);
			_authenticationService = new AuthenticationService(
				_userAccountDataAccess,
				new UserLoginDataAccess(
					_usersConnectionString,
					ConfigurationManager.AppSettings["UserLoginsTable"]!
				),
				cryptographyService,
				jwtHandlerService,
				new ValidationService(),
				loggerService
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
			var accessTokenResult = await _authorizationService.GenerateAccessToken(id, defaultUser).ConfigureAwait(false);
			var idTokenResult = _authenticationService.GenerateIdToken(id, accessTokenResult.Payload!);
			ClaimsPrincipal? actualPrincipal = null;
			if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
			{
				_testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
				actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
			}

			// Assert
			Assert.IsTrue(expectedResultSuccess == accessTokenResult.IsSuccessful);
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
			var accessTokenResult = await _authorizationService.GenerateAccessToken(id).ConfigureAwait(false);
			var idTokenResult = _authenticationService.GenerateIdToken(id, accessTokenResult.Payload!);
			if (accessTokenResult.IsSuccessful && idTokenResult.IsSuccessful)
			{
				_testingService.DecodeJWT(accessTokenResult.Payload!, idTokenResult.Payload!);
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