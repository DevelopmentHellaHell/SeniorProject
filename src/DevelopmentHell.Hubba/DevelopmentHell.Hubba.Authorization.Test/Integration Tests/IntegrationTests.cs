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

namespace DevelopmentHell.Hubba.Authorization.Test
{
	[TestClass]
	public class IntegrationTests
	{
		// Service to test
		private IAuthorizationService _authorizationService;
		// Helper services
		private IRegistrationService _registrationService;
		private IUserAccountDataAccess _userAccountDataAccess;

		public IntegrationTests()
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
		}

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

		[TestMethod]
		public void ShouldInstansiateCtor()
		{
			Assert.IsNotNull(_authorizationService);
		}

		[TestMethod]
		public async Task ShouldGenerateJwtToken()
		{
			// Arrange
			//    Setup user
			var email = "test@gmail.com";
			var password = "12345678";
			var registrationResult = await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
			var userIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
			var id = userIdResult.Payload;
			var userResult = await _userAccountDataAccess.GetUser(id).ConfigureAwait(false);

			var expectedId = id;
			var expectedEmail = email;
			var expectedRole = userResult.Payload!.Role;

			// Actual
			var actualToken = await _authorizationService.GenerateToken(id).ConfigureAwait(false);
			ClaimsPrincipal? actualPrincipal = null;
			if (actualToken.IsSuccessful)
			{
				decodeJWT(actualToken.Payload!);
				actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
			}

			// Assert
			Assert.IsTrue(actualToken.IsSuccessful);
			Assert.IsNotNull(actualPrincipal);
			Assert.IsTrue(actualPrincipal.FindFirstValue(ClaimTypes.Email)! == expectedEmail);
			Assert.IsTrue(actualPrincipal.FindFirstValue(ClaimTypes.Role)! == expectedRole);
			Assert.IsTrue(int.Parse(actualPrincipal.FindFirstValue("accountId")!) == expectedId);

			// Cleanup
			await _userAccountDataAccess.Delete(id).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task ShouldGenerateDefaultJwtToken()
		{
			// Arrange
			//    Setup user
			var email = "test@gmail.com";
			var password = "12345678";
			var registrationResult = await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
			var userIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
			var id = userIdResult.Payload;

			var expectedId = id;
			var expectedEmail = email;
			var expectedRole = "DefaultUser";

			// Actual
			var actualToken = await _authorizationService.GenerateToken(id, true).ConfigureAwait(false);
			ClaimsPrincipal? actualPrincipal = null;
			if (actualToken.IsSuccessful)
			{
				decodeJWT(actualToken.Payload!);
				actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
			}

			// Assert
			Assert.IsTrue(actualToken.IsSuccessful);
			Assert.IsNotNull(actualPrincipal);
			Assert.IsTrue(actualPrincipal.FindFirstValue(ClaimTypes.Email)! == expectedEmail);
			Assert.IsTrue(actualPrincipal.FindFirstValue(ClaimTypes.Role)! == expectedRole);
			Assert.IsTrue(int.Parse(actualPrincipal.FindFirstValue("accountId")!) == expectedId);

			// Cleanup
			await _userAccountDataAccess.Delete(id).ConfigureAwait(false);
		}

		[DataTestMethod]
		[DataRow("AdminUser")]
		[DataRow("DefaultUser", "VerifiedUser")]
		public async Task ShouldAuthorize(params string[] roles)
		{
			// Arrange
			var email = "test@gmail.com";
			var password = "12345678";
			var registrationResult = await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
			var userIdResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
			var id = userIdResult.Payload;
			var userResult = await _userAccountDataAccess.GetUser(id).ConfigureAwait(false);
			var actualToken = await _authorizationService.GenerateToken(id, true).ConfigureAwait(false);
			if (actualToken.IsSuccessful)
			{
				decodeJWT(actualToken.Payload!);
			}

			var expectedRole = userResult.Payload!.Role;

			// Actual
			var actual = _authorizationService.Authorize(roles);

			// Assert
			Assert.IsTrue(roles.Contains(expectedRole) == actual.IsSuccessful);

			// Cleanup
			await _userAccountDataAccess.Delete(id).ConfigureAwait(false);
		}
	}
}