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
			string UsersConnectionString = ConfigurationManager.AppSettings["UserConnectionString"]!;
			string UserAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
			string OTPTable = ConfigurationManager.AppSettings["OTPTable"]!;

			string LogsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
			string LogsTable = ConfigurationManager.AppSettings["LogsTable"]!;

            Console.WriteLine(UsersConnectionString);
			LoggerService loggerService = new LoggerService(new LoggerDataAccess(LogsConnectionString, LogsTable));
			RegistrationManager registrationmanager = new RegistrationManager(
				new RegistrationService(
					new UserAccountDataAccess(UsersConnectionString, UserAccountsTable),
					loggerService),
				loggerService);
			AuthenticationManager authenticationManager = new AuthenticationManager(
				new AuthenticationService(
					new UserAccountDataAccess(UsersConnectionString, UserAccountsTable),
					loggerService),
				new OTPService(
					new OTPDataAccess(UsersConnectionString, OTPTable)
					),
				loggerService);
			// TEMP: To delete data
			UserAccountDataAccess userAccountDataAccess = new UserAccountDataAccess(UsersConnectionString, UserAccountsTable);
			OTPDataAccess otpDataAccess = new OTPDataAccess(UsersConnectionString, OTPTable);

			string dummyIp = "192.0.2.0";
			string? cached_email = null;

            async Task<bool> Register()
            {
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
                    Result<int> getUserAccountResult = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
                    if (getUserAccountResult.IsSuccessful)
                    {
                        Console.WriteLine("User is Already Registered");
                    }
                    else
                    {
                        Console.WriteLine($"[ERROR]: {getUserAccountResult.ErrorMessage}");
                    }
                    return false;
                }
            }
            async Task<bool> Login()
            {
                Console.WriteLine();
                Console.Write("Email: ");
                string email = Console.ReadLine() ?? "";
                Console.Write("Password: ");
                string password = Console.ReadLine() ?? "";
                Result<int> loginResult = await authenticationManager.Login(email, password, dummyIp).ConfigureAwait(false);
                if (loginResult.IsSuccessful)
                {
                    cached_email = email;
                    Console.WriteLine("Login Success! Sending OTP...");
                    return true;
                }
                else
                {
                    Console.WriteLine($"[ERROR]: {loginResult.ErrorMessage}");
                }
                return false;
            }
            async Task OtpEntry()
            {
                if (cached_email == null)
                {
                    Console.WriteLine("User has not logged in recently, redirecting to Login view");
                    await Login();
                    return;
                }
                Console.WriteLine();
                Console.Write("OTP: ");
                string otp = Console.ReadLine() ?? "";
                Result otpresult = await authenticationManager.AuthenticateOTP((await userAccountDataAccess.GetId(cached_email).ConfigureAwait(false)).Payload, otp);
                if (otpresult.IsSuccessful)
                {
                    Console.WriteLine("OTP Login Success!");
					await Home();
					return;
                }
                else
                {
                    Console.WriteLine($"[ERROR]: {otpresult.ErrorMessage}");
                }
            }
            async Task Home()
            {
                Surround("Home View");
            }
            async Task<bool> DeleteLogin()
            {
                if (! await Login())
                {
                    Console.WriteLine("Unable to log into account");
                    return false;
                }
                int accountId = (await userAccountDataAccess.GetId(cached_email).ConfigureAwait(false)).Payload;
                Console.WriteLine();
                Console.Write("Email: ");
                string email = Console.ReadLine() ?? "";
                Console.Write("Password: ");
                string password = Console.ReadLine() ?? "";

                Result deleteAccountResult = await userAccountDataAccess.Delete(accountId).ConfigureAwait(false);
                if (deleteAccountResult.IsSuccessful)
                {
                    Console.WriteLine("Delete Success!");
                }
                else
                {
                    Console.WriteLine($"[ERROR]: {deleteAccountResult.ErrorMessage}");
                    return false;
                }
                return true;
            }
            async Task<bool> DeleteOtp()
            {
                if (!await Login())
                {
                    Console.WriteLine("Unable to log into account");
                    return false;
                }
                int accountId = (await userAccountDataAccess.GetId(cached_email).ConfigureAwait(false)).Payload;
                Result deleteOTPResult = await otpDataAccess.Delete(accountId).ConfigureAwait(false);
                if (deleteOTPResult.IsSuccessful)
                {
                    Console.WriteLine("Delete Success!");
                }
                else
                {
                    Console.WriteLine($"[ERROR]: {deleteOTPResult.ErrorMessage}");
                    return false;
                }
                return true;
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
                Console.WriteLine("4:Delete Login");
                Console.WriteLine("5:Delete OTP Entry");
                Console.WriteLine("6:Delete Login and OTP Entry");
                Console.WriteLine("7:Exit");
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
                } catch (Exception e)
                {
                    Console.WriteLine("Unable to parse input, try again\n");
                    continue;
                }

                switch(intChoice)
                {
                    case 1:
                        Surround("Registration View");
                        await Register();
                        break;
                    case 2:
						Surround("Login View");
                        await Login();
                        break;
                    case 3:
                        Surround("OTP Entry View");
                        await OtpEntry();
                        break;
                    case 4:
                        Console.WriteLine("\nDelete Login");
                        await DeleteLogin();
                        break;
                    case 5:
						Console.WriteLine("\nDelete OTP Entry");
                        await DeleteOtp();
                        break;
                    case 6:
                        Console.WriteLine("\nDelete Login and OTP Entry");
                        await DeleteOtp();
                        await DeleteLogin();
                        break;
                    case 7:
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
