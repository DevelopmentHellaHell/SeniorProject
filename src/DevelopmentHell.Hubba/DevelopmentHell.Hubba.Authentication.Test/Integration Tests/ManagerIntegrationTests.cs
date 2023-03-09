﻿using DevelopmentHell.Hubba.Authentication.Manager.Abstractions;
using DevelopmentHell.Hubba.Authentication.Manager.Implementations;
using DevelopmentHell.Hubba.Authentication.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Email.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword.Service.Abstractions;
using DevelopmentHell.Hubba.OneTimePassword.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DevelopmentHell.Hubba.Authentication.Test
{
    [TestClass]
	public class ManagerIntegrationTests
	{
        private string _usersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private string _userOTPsTable = ConfigurationManager.AppSettings["UserOTPsTable"]!;
        private string _userLoginsTable = ConfigurationManager.AppSettings["UserLoginsTable"]!;
        private string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
		private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;

        // Class to test
        private readonly IAuthenticationManager _authenticationManager;
		// Helper classes
		private readonly IUserAccountDataAccess _userAccountDataAccess;
		private readonly IRegistrationService _registrationService;
		private readonly IOTPService _otpService;
		private readonly ITestingService _testingService;

		public ManagerIntegrationTests()
		{
			ILoggerService loggerService = new LoggerService(
				new LoggerDataAccess(
                    _logsConnectionString,
                    _logsTable
                )
			);
			_userAccountDataAccess = new UserAccountDataAccess(
                _usersConnectionString,
                _userAccountsTable
            );
			ICryptographyService cryptographyService = new CryptographyService(
				ConfigurationManager.AppSettings["CryptographyKey"]!
			);
			IValidationService validationService = new ValidationService();
			_otpService = new OTPService(
				new OTPDataAccess(
                    _usersConnectionString,
                    _userOTPsTable
                ),
				new EmailService(
					ConfigurationManager.AppSettings["SENDGRID_USERNAME"]!,
					ConfigurationManager.AppSettings["SENDGRID_API_KEY"]!,
					ConfigurationManager.AppSettings["COMPANY_EMAIL"]!,
					true
				),
				cryptographyService
			);
			_authenticationManager = new AuthenticationManager(
				new AuthenticationService(
					_userAccountDataAccess,
					new UserLoginDataAccess(
                        _usersConnectionString,
                        _userLoginsTable
                    ),
					cryptographyService,
					validationService,
					loggerService
				),
				_otpService,
				new AuthorizationService(
                    _jwtKey,
                    new UserAccountDataAccess(
                        _usersConnectionString,
                        _userAccountsTable
                    ),
					loggerService
				),
				cryptographyService,
				loggerService
			);
			_registrationService = new RegistrationService(
				_userAccountDataAccess,
				cryptographyService,
				validationService,
				loggerService
			);
            _testingService = new TestingService(
                new TestsDataAccess()
            );
        }

		[TestMethod]
		public void ShouldInstansiateCtor()
		{
			Assert.IsNotNull(_authenticationManager);
		}

		[DataTestMethod]
		[DataRow("", "")]
		[DataRow("test@gmail.com", "12345678")]
		[DataRow("invalidtest@gmail.com", "12345678")]
		[DataRow("test@gmail.com", "invalidpassword")]
		public async Task ShouldLoginWithCredentials(string email, string password)
		{
			// Arrange
			//  - Setup user and initial state
			var credentialEmail = "test@gmail.com";
			var credentialPassword = "12345678";
			var ipAddress = "1.1.1.1";
			await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
			var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
			var id = userIdResult.Payload;
			
			var expectedResultSuccess = credentialEmail == email && credentialPassword == password;
			var expectedRole = "DefaultUser";

			// Actual
			var actualLoginResult = await _authenticationManager.Login(email, password, ipAddress);
			ClaimsPrincipal? actualPrincipal = null;
			if (actualLoginResult.IsSuccessful)
			{
				_testingService.DecodeJWT(actualLoginResult.Payload!);
				actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
			}

			// Assert
			Assert.IsTrue(expectedResultSuccess == actualLoginResult.IsSuccessful);
			//  - Exists a principal if matching credentials
			if (expectedResultSuccess)
			{
				Assert.IsNotNull(actualLoginResult.Payload);
				Assert.IsTrue(actualPrincipal.FindFirstValue(ClaimTypes.Email)! == credentialEmail);
				Assert.IsTrue(actualPrincipal.FindFirstValue(ClaimTypes.Role)! == expectedRole);
				Assert.IsTrue(int.Parse(actualPrincipal.FindFirstValue("accountId")!) == id);
			} else
			{
				Assert.IsNull(actualLoginResult.Payload);
			}
		}

		[TestMethod]
		public async Task ShouldNotLoginWithDisabledAccount()
		{
			// Arrange
			//  - Setup user and initial state
			var credentialEmail = "test@gmail.com";
			var credentialPassword = "12345678";
			var ipAddress = "1.1.1.1";
			await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
			var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
			var id = userIdResult.Payload;
			await _userAccountDataAccess.Update(new UserAccount()
			{
				Id = id,
				Disabled = true,
			});

			var expectedResultSuccess = false;

			// Actual
			var actualLoginResult = await _authenticationManager.Login(credentialEmail, credentialPassword, ipAddress);

			// Assert
			Assert.IsTrue(expectedResultSuccess == actualLoginResult.IsSuccessful);
			Assert.IsNull(actualLoginResult.Payload);
		}

		[TestMethod]
		public async Task ShouldNotLoginWhileAlreadyLoggedIn()
		{
			// Arrange
			//  - Setup user and initial state
			var credentialEmail = "test@gmail.com";
			var credentialPassword = "12345678";
			var ipAddress = "1.1.1.1";
			var registrationResult = await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
			var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
			var id = userIdResult.Payload;

			var expectedResultSuccess = false;

			// Actual
			Result<string> actualLoginResult = new Result<string>();
			//  - Log in attempt #1
			actualLoginResult = await _authenticationManager.Login(credentialEmail, credentialPassword, ipAddress);
			_testingService.DecodeJWT(actualLoginResult.Payload!);
			//  - Get valid OTP from database
			var otpResult = await _otpService.GetOTP(id).ConfigureAwait(false);
			var authenticateOTPResult = await _authenticationManager.AuthenticateOTP(otpResult.Payload!, ipAddress).ConfigureAwait(false);
			_testingService.DecodeJWT(authenticateOTPResult.Payload!);
			
			//  - Log in attempt #2
			actualLoginResult = await _authenticationManager.Login(credentialEmail, credentialPassword, ipAddress);

			// Assert
			Assert.IsTrue(expectedResultSuccess == actualLoginResult.IsSuccessful);
			Assert.IsNull(actualLoginResult.Payload);
		}

		[DataTestMethod]
		[DataRow("")]
		[DataRow("invalid1")]
		[DataRow("invalidOtp!!!")]
		public async Task ShouldNotAuthenticateInvalidOTP(string otp)
		{
			// Arrange
			//  - Setup user and initial state
			var credentialEmail = "test@gmail.com";
			var credentialPassword = "12345678";
			var ipAddress = "1.1.1.1";
			await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
			var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
			var id = userIdResult.Payload;
			var actualLoginResult = await _authenticationManager.Login(credentialEmail, credentialPassword, ipAddress);
			_testingService.DecodeJWT(actualLoginResult.Payload!);

			var expectedResultSuccess = false;
			var expectedRole = "DefaultUser";

			// Actual
			var actualAuthenticateOTPResult = await _authenticationManager.AuthenticateOTP(otp, ipAddress).ConfigureAwait(false);
			ClaimsPrincipal? actualPrincipal = null;
			if (actualLoginResult.IsSuccessful)
			{
				_testingService.DecodeJWT(actualLoginResult.Payload!);
				actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
			}

			// Assert
			Assert.IsTrue(expectedResultSuccess == actualAuthenticateOTPResult.IsSuccessful);
			Assert.IsNotNull(actualPrincipal);
			Assert.IsTrue(actualPrincipal.FindFirstValue(ClaimTypes.Email)! == credentialEmail);
			Assert.IsTrue(actualPrincipal.FindFirstValue(ClaimTypes.Role)! == expectedRole);
			Assert.IsTrue(int.Parse(actualPrincipal.FindFirstValue("accountId")!) == id);
        }

		[TestMethod]
		public async Task ShouldAuthenticateCorrectOTP()
		{
			// Arrange
			//  - Setup user and initial state
			var credentialEmail = "test@gmail.com";
			var credentialPassword = "12345678";
			var ipAddress = "1.1.1.1";
			await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
			var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
			var id = userIdResult.Payload;
			var loginResult = await _authenticationManager.Login(credentialEmail, credentialPassword, ipAddress);
			_testingService.DecodeJWT(loginResult.Payload!);
			//  - Get valid OTP from database
			var otpResult = await _otpService.GetOTP(id).ConfigureAwait(false);

			var expectedResultSuccess = true;
			var expectedRole = "VerifiedUser";

			// Actual
			var actualAuthenticateOTPResult = await _authenticationManager.AuthenticateOTP(otpResult.Payload!, ipAddress).ConfigureAwait(false);
			ClaimsPrincipal? actualPrincipal = null;
			if (actualAuthenticateOTPResult.IsSuccessful)
			{
				_testingService.DecodeJWT(actualAuthenticateOTPResult.Payload!);
				actualPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
			}

			// Assert
			Assert.IsTrue(expectedResultSuccess == actualAuthenticateOTPResult.IsSuccessful);
			Assert.IsNotNull(actualPrincipal);
			Assert.IsTrue(actualPrincipal.FindFirstValue(ClaimTypes.Email)! == credentialEmail);
			Assert.IsTrue(actualPrincipal.FindFirstValue(ClaimTypes.Role)! == expectedRole);
			Assert.IsTrue(int.Parse(actualPrincipal.FindFirstValue("accountId")!) == id);
		}

		// login 3 times and disable account - shoulddisableaccount
		[TestMethod]
		public async Task ShouldDisableAccountAfter3LoginAttempts()
		{
			// Arrange
			//  - Setup user and initial state
			var credentialEmail = "test@gmail.com";
			var credentialPassword = "12345678";
			var invalidPassword = "invalidPassword";
			var ipAddress = "1.1.1.1";
			await _registrationService.RegisterAccount(credentialEmail, credentialPassword).ConfigureAwait(false);
			var userIdResult = await _userAccountDataAccess.GetId(credentialEmail).ConfigureAwait(false);
			var id = userIdResult.Payload;

			var expectedDisabled = true;
			var expectedResultSuccess = false;

			// Actual
			Result<string> actualLoginResult = new Result<string>();
			for (var i = 0; i < 3; i++)
			{
				actualLoginResult = await _authenticationManager.Login(credentialEmail, invalidPassword, ipAddress);
			}
			var disabledResult = await _userAccountDataAccess.GetDisabled(id).ConfigureAwait(false);

			// Assert
			Assert.IsTrue(expectedResultSuccess == actualLoginResult.IsSuccessful);
			Assert.IsNull(actualLoginResult.Payload);
			Assert.IsTrue(expectedDisabled == disabledResult.Payload);
		}

		// expiredOtp - shouldnotloginwithexpiredotp

		[TestCleanup]
		public async Task Cleanup()
		{
			await _testingService.DeleteAllRecords().ConfigureAwait(false);
		}
	}
}