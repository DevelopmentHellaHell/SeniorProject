using System.Configuration;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Manager;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Authentication.Manager;
using DevelopmentHell.Hubba.Logging.Service.Implementation;
using DevelopmentHell.Hubba.OneTimePassword.Service.Implementation;
using DevelopmentHell.Hubba.Registration.Service.Implementation;
using DevelopmentHell.Hubba.Authentication.Service.Implementation;

namespace DevelopmentHell.Hubba.Client
{
    public class ViewDemoConsole
    {
        public static async Task Main()
        {
			string UserAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
			string UserOTPsTable = ConfigurationManager.AppSettings["UserOTPsTable"]!;
            string UserSessionsTable = ConfigurationManager.AppSettings["UserSessionsTable"]!;
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
			AuthenticationManager authenticationManager = new AuthenticationManager(
				new AuthenticationService(
					new UserAccountDataAccess(UsersConnectionString, UserAccountsTable), 
					loggerService,
                    new UserSessionDataAccess(UsersConnectionString, UserSessionsTable)
                ),
				new OTPService(
					new OTPDataAccess(UsersConnectionString, UserOTPsTable)
				),
				loggerService
            );
			// TEMP: To delete data
			UserAccountDataAccess userAccountDataAccess = new UserAccountDataAccess(UsersConnectionString, UserAccountsTable);
			OTPDataAccess otpDataAccess = new OTPDataAccess(UsersConnectionString, UserOTPsTable);

			string dummyIp = "192.0.2.0";
			string? cachedEmail = null;

            async Task<bool> Register()
            {
				Surround("Registration View");
				Console.WriteLine();
                Console.Write("Email: ");
                string email = Console.ReadLine() ?? "";
                Console.Write("Password: ");
                string password = Console.ReadLine() ?? "";

                var registerResult = await registrationmanager.Register(email, password).ConfigureAwait(false);
                if (registerResult.IsSuccessful)
                {
                    Console.WriteLine("Registeration Success!");
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
				Surround("Login View");
				Console.WriteLine();
                Console.Write("Email: ");
                string email = Console.ReadLine() ?? "";
                Console.Write("Password: ");
                string password = Console.ReadLine() ?? "";
                Result<bool> loginResult = await authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
                if (loginResult.IsSuccessful)
                {
                    cachedEmail = email;
                    Console.WriteLine("Login Success! Sending OTP...");
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
                if (cachedEmail == null)
                {
                    Console.WriteLine("User has not logged in recently, redirecting to Login view");
                    await Login();
                    return false;
                }

				Surround("OTP View");
				Console.WriteLine();
                Console.Write("OTP: ");
                string otp = Console.ReadLine() ?? "";
                Result<AuthTicket> otpResult = await authenticationManager.AuthenticateOTP((await userAccountDataAccess.GetId(cachedEmail).ConfigureAwait(false)).Payload, otp);
                if (otpResult.IsSuccessful)
                {
                    Console.WriteLine("OTP Login Success!");
					Home();
					return true;
                }
                else
                {
                    Console.WriteLine($"[ERROR]: {otpResult.ErrorMessage}");
                    return false;
                }
            }
            void Home()
            {
                Surround("Home View");
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
                Console.WriteLine("3:Attempt OTP Entry");
                Console.WriteLine("4:Exit");
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

                switch(intChoice)
                {
                    case 1:
                        while(!await Register());
                        break;
                    case 2:
                        while(!await Login());
						while(!await OtpEntry());
                        break;
                    case 4:
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
