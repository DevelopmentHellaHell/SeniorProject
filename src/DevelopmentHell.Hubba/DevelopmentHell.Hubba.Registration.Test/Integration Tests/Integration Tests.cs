using DevelopmentHell.Hubba.Authentication.Manager;
using DevelopmentHell.Hubba.Authentication.Manager.Implementations;
using DevelopmentHell.Hubba.Authentication.Service.Implementation;
using DevelopmentHell.Hubba.Authorization.Service.Implementation;
using DevelopmentHell.Hubba.Cryptography.Service;
using DevelopmentHell.Hubba.Logging.Service.Implementation;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword.Service.Implementation;
using DevelopmentHell.Hubba.Registration.Manager.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Implementation;
using DevelopmentHell.Hubba.SqlDataAccess;
using System.Configuration;
using System.Diagnostics;
using System.Security.Principal;

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

		/* Success Case
		 * Goal: Login Successfully
		 * Process: Register Account Successfully, Authenticate Username and Password Credentials, Authenticate One Time Password
		 */
		[TestMethod]
		public async Task TestCase01()
		{

			// Arrange
			var expected = new Result()
			{
				IsSuccessful = true
			};
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
			string email = "registration-test01@gmail.com";
			string password = "12345678";
			var stopwatch = new Stopwatch();
			var expectedTime = 5;

			//Cleanup
			Result<int> existingAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
			int accountId = existingAccountId.Payload;
			if (existingAccountId.Payload > 0)
			{
				await otpDataAccess.Delete(accountId).ConfigureAwait(false);
				await userAccountDataAccess.Delete(accountId).ConfigureAwait(false);
			}

			// Act
			stopwatch.Start();
			var actual = await registrationManager.Register(email, password).ConfigureAwait(false);
			stopwatch.Stop();
			var actualTime = stopwatch.ElapsedMilliseconds / 1000;

			// Assert
			Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
			Assert.IsTrue(actualTime <= expectedTime);
		}


		/* 
		 * Failure Case
		 * Goal: Register for a new account after already being logged in
		 * Process: Register Account Successfully, Log in Successfully, Attempt to Register Account
		 */
		[TestMethod]
		public async Task TestCase02()
		{

			// Arrange
			var expected = new Result()
			{
				IsSuccessful = false,
				ErrorMessage = "Error, user already logged in."
			};
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
				new AuthorizationService(),
				loggerService
			);
			string email = "registration-test02@gmail.com";
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

			await registrationManager.Register(email, password).ConfigureAwait(false);
			await authenticationManager.Login(email, password, dummyIp, null, false).ConfigureAwait(false);
			Result<int> getNewAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
			int newAccountId = getNewAccountId.Payload;
			Result<byte[]> getOtp = await otpDataAccess.GetOTP(newAccountId).ConfigureAwait(false);
			string otp = EncryptionService.Decrypt(getOtp.Payload!);

			// Act
			Result<GenericPrincipal> authenticationResult = await authenticationManager.AuthenticateOTP(getNewAccountId.Payload, otp, dummyIp).ConfigureAwait(false);
			var actual = await registrationManager.Register(email, password, authenticationResult.Payload).ConfigureAwait(false);

			// Assert
			Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
			Assert.IsTrue(actual.ErrorMessage == expected.ErrorMessage);
		}

		/*
		 * Failure Case 
		 * Goal: Invalid Email input
		 * Process: Register for a new account using invalid input of an empty email
		 */
		[TestMethod]
		public async Task TestCase03()
		{

			// Arrange
			var expected = new Result()
			{
				IsSuccessful = false,
				ErrorMessage = "Invalid email provided. Retry again or contact system administrator."
			};
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
			string email = "";
			string password = "12345678";

			//Cleanup
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
			Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
			Assert.IsTrue(actual.ErrorMessage == expected.ErrorMessage);
		}



		/* 
        * Failure Case 
        * Goal: Invalid Email input
        * Process: Register for a new account using invalid input of email less than eight characters
        */
		[TestMethod]
		public async Task TestCase04()
		{

			// Arrange
			var expected = new Result()
			{
				IsSuccessful = false,
				ErrorMessage = "Invalid email provided. Retry again or contact system administrator."
			};
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
			string email = "r@g.c";
			string password = "12345678";

			//Cleanup
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
			Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
			Assert.IsTrue(actual.ErrorMessage == expected.ErrorMessage);
		}


		/* 
        * Failure Case 
        * Goal: Invalid Email input
        * Process: Register for a new account using invalid input of a non-lowercase email
        */
		[TestMethod]
		public async Task TestCase05()
		{

			// Arrange
			var expected = new Result()
			{
				IsSuccessful = false,
				ErrorMessage = "Invalid email provided. Retry again or contact system administrator."
			};
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
			string email = "registration-Test05@gmail.com";
			string password = "12345678";

			//Cleanup
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
			Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
			Assert.IsTrue(actual.ErrorMessage == expected.ErrorMessage);
		}

		/* 
        * Failure Case 
        * Goal: Invalid Email input
        * Process: Register for a new account using invalid input of an email containing an invalid special character
        */
		[TestMethod]
		public async Task TestCase06()
		{

			// Arrange
			var expected = new Result()
			{
				IsSuccessful = false,
				ErrorMessage = "Invalid email provided. Retry again or contact system administrator."
			};
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
			string email = "registration-test06@gmail.c$om";
			string password = "12345678";

			//Cleanup
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
			Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
			Assert.IsTrue(actual.ErrorMessage == expected.ErrorMessage);
		}


		/*
        * Failure Case
        * Goal: Invalid Email input
        * Process: Register for a new account using an incorrectly formatted email
        */
		[TestMethod]
		public async Task TestCase07()
		{

			// Arrange
			var expected = new Result()
			{
				IsSuccessful = false,
				ErrorMessage = "Invalid email provided. Retry again or contact system administrator."
			};
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
			string email = "@registration-test07.gmail.com";
			string password = "12345678";

			//Cleanup
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
			Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
			Assert.IsTrue(actual.ErrorMessage == expected.ErrorMessage);
		}


		/* 
        * Failure Case
        * Goal: Invalid Password input
        * Process: Register for a new account using an invalid input of an empty password
        */
		[TestMethod]
		public async Task TestCase08()
		{

			// Arrange
			var expected = new Result()
			{
				IsSuccessful = false,
				ErrorMessage = "Invalid password provided. Retry again or contact system administrator."
			};
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
			string email = "registration-test08@gmail.com";
			string password = "";

			//Cleanup
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
			Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
			Console.WriteLine(actual.ErrorMessage);
		}

		/* 
        * Failure Case
        * Goal: Invalid Password input
        * Process: Register for a new account using an invalid input of a password less than eight characters
        */
		[TestMethod]
		public async Task TestCase09()
		{

			// Arrange
			var expected = new Result()
			{
				IsSuccessful = false,
				ErrorMessage = "Invalid password provided. Retry again or contact system administrator."
			};
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
			string email = "registration-test09@gmail.com";
			string password = "12345";

			//Cleanup
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
			Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
			Assert.IsTrue(actual.ErrorMessage == expected.ErrorMessage);
		}


		/* 
        * Failure Case
        * Goal: Invalid Password input
        * Process: Register for a new account using an invalid input of a password containing an invalid special character
        */
		[TestMethod]
		public async Task TestCase10()
		{

			// Arrange
			var expected = new Result()
			{
				IsSuccessful = false,
				ErrorMessage = "Invalid password provided. Retry again or contact system administrator."
			};
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
			string email = "registration-test10@gmail.com";
			string password = "$SELECT * FROM UserAccounts";

			//Cleanup
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
			Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
			Assert.IsTrue(actual.ErrorMessage == expected.ErrorMessage);
		}

		/*
         * Failure Case
         * Goal: Attempt to register an account with an existing username
         * Process: Register Account Successfully, Attempt to register an account using the same username
         */
		[TestMethod]
		public async Task TestCase11()
		{

			// Arrange
			var expected = new Result()
			{
				IsSuccessful = false,
				ErrorMessage = "An account with that email already exists."
			};
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
			string email = "registration-test11@gmail.com";
			string password = "12345678";
			string password2 = "87654321";


			//Cleanup
			Result<int> existingAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
			int accountId = existingAccountId.Payload;
			if (existingAccountId.Payload > 0)
			{
				await otpDataAccess.Delete(accountId).ConfigureAwait(false);
				await userAccountDataAccess.Delete(accountId).ConfigureAwait(false);
			}

			// Act
			await registrationManager.Register(email, password).ConfigureAwait(false);
			var actual = await registrationManager.Register(email, password2).ConfigureAwait(false);


			// Assert
			Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
			Assert.IsTrue(actual.ErrorMessage == expected.ErrorMessage);
		}

		/*
         * Failure Case
         * Goal: Attempt to create an account with UserAccounts data store being offline
         * Process: Do not set connection string, attempt to register account
         */
		[TestMethod]
		public async Task TestCase12()
		{

			// Arrange
			var expected = new Result()
			{
				IsSuccessful = false,
				ErrorMessage = "Unable to assign username. Retry again or contact system administrator"
			};
			string dummyUserConnectionString = "dummyConnectionString";
			var otpDataAccess = new OTPDataAccess(_UsersConnectionString, _UserOTPsTable);
			var userAccountDataAccess = new UserAccountDataAccess(dummyUserConnectionString, _UserAccountsTable);
			var loggerService = new LoggerService(
				new LoggerDataAccess(_LogsConnectionString, _LogsTable)
			);
			var registrationManager = new RegistrationManager(
				new RegistrationService(
					new UserAccountDataAccess(dummyUserConnectionString, _UserAccountsTable),
					loggerService
				),
				loggerService
			);
			string email = "registration-test12@gmail.com";
			string password = "12345678";
			string password2 = "87654321";


			//Cleanup
			Result<int> existingAccountId = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
			int accountId = existingAccountId.Payload;
			if (existingAccountId.Payload > 0)
			{
				await otpDataAccess.Delete(accountId).ConfigureAwait(false);
				await userAccountDataAccess.Delete(accountId).ConfigureAwait(false);
			}

			// Act
			await registrationManager.Register(email, password).ConfigureAwait(false);
			var actual = await registrationManager.Register(email, password2).ConfigureAwait(false);


			// Assert
			Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
			//Console.WriteLine(actual.ErrorMessage);
			Assert.IsTrue(actual.ErrorMessage == expected.ErrorMessage);
		}
	}
}