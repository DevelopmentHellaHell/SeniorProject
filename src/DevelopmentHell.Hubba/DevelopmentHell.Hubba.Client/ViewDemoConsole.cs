using DevelopmentHell.Hubba.Authentication.Manager.Implementations;
using DevelopmentHell.Hubba.Authentication.Service.Implementation;
using DevelopmentHell.Hubba.Authorization.Service.Implementation;
using DevelopmentHell.Hubba.Logging.Service.Implementation;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.OneTimePassword.Service.Implementation;
using DevelopmentHell.Hubba.Registration.Manager.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Implementation;
using DevelopmentHell.Hubba.SqlDataAccess;
using System.Configuration;
using System.Security.Principal;

namespace DevelopmentHell.Hubba.Client
{
    public class ViewDemoConsole
	{
		private IPrincipal? _principal;
		public async Task Run()
		{
			string UserAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
			string UserOTPsTable = ConfigurationManager.AppSettings["UserOTPsTable"]!;
			string LogsTable = ConfigurationManager.AppSettings["LogsTable"]!;
			string UsersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
			string LogsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;

			LoggerService loggerService = new LoggerService(new LoggerDataAccess(LogsConnectionString, LogsTable));
			RegistrationManager registrationmanager = new RegistrationManager(
				new RegistrationService(
					new UserAccountDataAccess(UsersConnectionString, UserAccountsTable),
					loggerService
				),
				loggerService
			);
			AuthorizationService authorizationService = new AuthorizationService();
			AuthenticationManager authenticationManager = new AuthenticationManager(
				new AuthenticationService(
					new UserAccountDataAccess(UsersConnectionString, UserAccountsTable),
					loggerService
				),
				new OTPService(
					new OTPDataAccess(UsersConnectionString, UserOTPsTable)
				),
				authorizationService,
				loggerService
			);
			// TEMP: To delete data
			UserAccountDataAccess userAccountDataAccess = new UserAccountDataAccess(UsersConnectionString, UserAccountsTable);
			OTPDataAccess otpDataAccess = new OTPDataAccess(UsersConnectionString, UserOTPsTable);

			string dummyIp = "192.0.2.0";
			string? cachedEmail = null;

			async Task<bool> Register()
			{
				if (authorizationService.authorize(_principal, new string[] { "VerifiedUser", "Admin" }).IsSuccessful)
				{
					Console.WriteLine("Error, you are already logged in.");
					return false;
				}
				Surround("Registration View");
				Surround("Note: User must enter email in all lowercase");
				Console.WriteLine();
				Console.Write("Email: ");
				string email = Console.ReadLine() ?? "";
				Console.Write("Password: ");
				string password = Console.ReadLine() ?? "";

				Console.WriteLine(Thread.CurrentPrincipal);
				var registerResult = await registrationmanager.Register(email, password, _principal).ConfigureAwait(false);
				if (registerResult.IsSuccessful)
				{
					Console.WriteLine("Registration Success!");
					return true;
				}
				else
				{
					Console.WriteLine(registerResult.ErrorMessage);
					return false;
				}
			}
			async Task<bool> Login()
			{
				if (authorizationService.authorize(_principal, new string[] { "VerifiedUser", "Admin" }).IsSuccessful)
				{
					Console.WriteLine("Error, you are already logged in.");
					return false;
				}
				Surround("Login View");

				Console.WriteLine();
				Console.Write("Email: ");
				string email = Console.ReadLine() ?? "";
				Console.Write("Password: ");
				string password = Console.ReadLine() ?? "";
				Result<bool> loginResult = await authenticationManager.Login(email, password, dummyIp, _principal).ConfigureAwait(false);
				if (loginResult.IsSuccessful)
				{
					cachedEmail = email;
					Console.WriteLine("Sending OTP to your email, please check spam folder just in case!");
					return true;
				}
				else
				{
					Console.WriteLine($"[ERROR]: {loginResult.ErrorMessage}");
				}
				return false;
			}
			async Task<bool> OtpEntry()
			{
				if (authorizationService.authorize(_principal, new string[] { "VerifiedUser", "Admin" }).IsSuccessful)
				{
					Console.WriteLine("Error, you are already logged in.");
					return false;
				}

				while (true)
				{
					Surround("OTP View");
					Console.WriteLine();
					Console.Write("OTP: ");
					string otp = Console.ReadLine() ?? "";
					Result<GenericPrincipal> otpResult = await authenticationManager.AuthenticateOTP((await userAccountDataAccess.GetId(cachedEmail!).ConfigureAwait(false)).Payload, otp, dummyIp).ConfigureAwait(false);
					if (otpResult.IsSuccessful)
					{
						Console.WriteLine("Login Success!");
						_principal = otpResult.Payload!;
						Home();
						return true;
					}
					else
					{
						Console.WriteLine($"[ERROR]: {otpResult.ErrorMessage}");
						return false;
					}
				}
			}
			void Home()
			{
				Surround("User Home View");
			}

			void Surround(string text, char character = '-', int spacing = 1)
			{
				int textLength = text.Length;
				int barLength = textLength + spacing * 2 + 2;
				Console.WriteLine(new String(character, barLength));
				Console.WriteLine($"{character}{new String(' ', spacing)}{text}{new String(' ', spacing)}{character}");
				Console.WriteLine(new String(character, barLength));
			}

			while (true)
			{
				Console.WriteLine("\n1:Attempt Registration");
				Console.WriteLine("2:Attempt Login");
				Console.WriteLine("3:Exit");
				Console.Write("Choose view to access:");

				string choice = Console.ReadLine() ?? "";
				int intChoice;
				try
				{
					intChoice = Convert.ToInt32(choice);
					if (choice.Length == 0)
					{
						Console.WriteLine("No input found, try again\n");
						continue;
					}
				}
				catch
				{
					Console.WriteLine("Unable to parse input, try again\n");
					continue;
				}

				switch (intChoice)
				{
					case 1:
						await Register();
						break;
					case 2:
						if (await Login()) await OtpEntry();
						break;
					case 3:
						Console.WriteLine("\nGoodbye");
						return;
					default:
						Console.WriteLine("Invalid input, try again\n");
						break;
				}
			}
		}

	}
}
