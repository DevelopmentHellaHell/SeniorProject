//using DevelopmentHell.Hubba.Authentication.Manager;
//using DevelopmentHell.Hubba.Authentication.Service.Implementation;
//using DevelopmentHell.Hubba.Logging.Service.Implementation;
//using DevelopmentHell.Hubba.Models;
//using DevelopmentHell.Hubba.OneTimePassword.Service.Implementation;
//using DevelopmentHell.Hubba.Registration.Manager;
//using DevelopmentHell.Hubba.Registration.Service.Implementation;
//using DevelopmentHell.Hubba.SqlDataAccess;
//using System.Configuration;

//namespace DevelopmentHell.Hubba.Client
//{
//	public class DemoClass
//	{
		

//		public static async Task Main(string[] args)
//		{
//			string AccountServer             = ConfigurationManager.AppSettings["AccountServer"]!;
//			string AccountDatabase           = ConfigurationManager.AppSettings["AccountDatabase"]!;
//			string AccountDataAccessUser     = ConfigurationManager.AppSettings["AccountDataAccessUser"]!;
//			string AccountDataAccessPass     = ConfigurationManager.AppSettings["AccountDataAccessPass"]!;
//			string UserAccountsTable         = ConfigurationManager.AppSettings["UserAccountsTable"]!;
//			string OTPTable                  = ConfigurationManager.AppSettings["OTPTable"]!;
//			string LoggingServer             = ConfigurationManager.AppSettings["LoggingServer"]!;
//			string LoggingDatabase           = ConfigurationManager.AppSettings["LoggingDatabase"]!;
//			string LoggingDataAccessUser     = ConfigurationManager.AppSettings["LoggingDataAccessUser"]!;
//			string LoggingDataAccessPass     = ConfigurationManager.AppSettings["LoggingDataAccessPass"]!;
//			string LogsTable                 = ConfigurationManager.AppSettings["LogsTable"]!;
//			string OTPExpirationOffsetSeconds= ConfigurationManager.AppSettings["OTPExpirationOffsetSeconds"]!;
//			string HubbaEmailAddress         = ConfigurationManager.AppSettings["HubbaEmailAddress"]!;
//			string HubbaEmailPassword        = ConfigurationManager.AppSettings["HubbaEmailPassword"]!;
//			string AESKey                    = ConfigurationManager.AppSettings["AESKey"]!;
//            string usersConnectionString     = $"Server={AccountServer};Database={AccountDatabase};Encrypt=false;User Id={AccountDataAccessUser};Password={AccountDataAccessPass}";
//			string logsConnectionString      = $"Server={LoggingServer};Database={LoggingDatabase};Encrypt=false;User Id={LoggingDataAccessUser};Password={LoggingDataAccessPass}";
//			LoggerService loggerService = new LoggerService(new LoggerDataAccess(logsConnectionString, LogsTable));
//			RegistrationManager registrationmanager = new RegistrationManager(
//				new RegistrationService(
//					new UserAccountDataAccess(usersConnectionString, UserAccountsTable),
//					loggerService),
//				loggerService);
//			AuthenticationManager authenticationManager = new AuthenticationManager(
//				new AuthenticationService(
//					new UserAccountDataAccess(usersConnectionString, UserAccountsTable),
//					loggerService),
//				new OTPService(
//					new OTPDataAccess(usersConnectionString, OTPTable)
//					),
//				loggerService);
//			// TEMP: To delete data
//			UserAccountDataAccess userAccountDataAccess = new UserAccountDataAccess(usersConnectionString, UserAccountsTable);
//			OTPDataAccess otpDataAccess = new OTPDataAccess(usersConnectionString, OTPTable);

//			string dummyIp = "192.0.2.0";
//			string email;
//			string password;
//			while (true)
//			{
//				Console.WriteLine();
//				Surround("Registration View");
//				Console.Write("Email: ");
//				email = Console.ReadLine() ?? "";
//				Console.Write("Password: ");
//				password = Console.ReadLine() ?? "";

//				var registerResult = await registrationmanager.Register(email, password).ConfigureAwait(false);
//				if (registerResult.IsSuccessful)
//				{
//					Console.WriteLine("Registeration Success!");
//					break;
//				}
//				else
//				{
//					Console.WriteLine(registerResult.ErrorMessage);
//					Result<int> getUserAccountResult = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
//					if (getUserAccountResult.IsSuccessful)
//					{
//						await Delete(getUserAccountResult.Payload, otpDataAccess, userAccountDataAccess).ConfigureAwait(false);
//					}
//					else
//					{
//						Console.WriteLine($"[ERROR]: {getUserAccountResult.ErrorMessage}");
//					}
//				}
//			}

//			int accountId;
//			while (true)
//			{
//				Console.WriteLine();
//				Surround("Login View");
//				Console.Write("Email: ");
//				email = Console.ReadLine() ?? "";
//				Console.Write("Password: ");
//				password = Console.ReadLine() ?? "";
//				Result<int> loginResult = await authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
//				if (loginResult.IsSuccessful)
//				{
//					accountId = loginResult.Payload;
//					Console.WriteLine("Login Success! Sending OTP...");
//					break;
//				}
//				else
//				{
//					Console.WriteLine($"[ERROR]: {loginResult.ErrorMessage}");
//				}
//			}

//			string otp;
//			while (true)
//			{
//				Console.WriteLine();
//				Surround("OTP Entry View");
//				Console.Write("OTP: ");
//				otp = Console.ReadLine() ?? "";
//				Result otpresult = await authenticationManager.AuthenticateOTP(accountId, otp);
//				if (otpresult.IsSuccessful)
//				{
//					Console.WriteLine("OTP Login Success!");
//					break;
//				}
//				else
//				{
//					Console.WriteLine($"[ERROR]: {otpresult.ErrorMessage}");
//				}
//			}

//			await Delete(accountId, otpDataAccess, userAccountDataAccess).ConfigureAwait(false);
//		}

//		public static async Task<bool> Delete(int accountId, OTPDataAccess otpDataAccess,UserAccountDataAccess userAccountDataAccess)
//		{
//			Result deleteOTPResult = await otpDataAccess.Delete(accountId).ConfigureAwait(false);
//			Result deleteAccountResult = await userAccountDataAccess.Delete(accountId).ConfigureAwait(false);
//			if (deleteOTPResult.IsSuccessful && deleteAccountResult.IsSuccessful)
//			{
//				Console.WriteLine("Delete Success!");
//			}
//			else
//			{
//				Console.WriteLine($"[ERROR]: {deleteOTPResult.ErrorMessage}");
//				Console.WriteLine($"[ERROR]: {deleteAccountResult.ErrorMessage}");
//			}
//			return true;
//		}
//		public static void Surround(string text, char character = '-', int spacing = 1)
//		{
//			int textLength = text.Length;
//			int barLength = textLength + spacing * 2 + 2;
//			Console.WriteLine(new String(character, barLength));
//			Console.WriteLine($"{character}{new String(' ', spacing)}{text}{new String(' ', spacing)}{character}");
//			Console.WriteLine(new String(character, barLength));
//		}
//	}
//}