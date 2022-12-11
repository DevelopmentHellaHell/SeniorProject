using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Manager;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Authentication.Manager;
using Microsoft.Identity.Client;
using static System.Net.WebRequestMethods;

namespace DevelopmentHell.Hubba.Client
{
    public class ViewDemoConsole
    {
        public static async Task Main()
        {
            string AccountServer = ConfigurationManager.AppSettings["AccountServer"]!;
            string AccountDatabase = ConfigurationManager.AppSettings["AccountDatabase"]!;
            string AccountDataAccessUser = ConfigurationManager.AppSettings["AccountDataAccessUser"]!;
            string AccountDataAccessPass = ConfigurationManager.AppSettings["AccountDataAccessPass"]!;
            string UserAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
            string OTPTable = ConfigurationManager.AppSettings["OTPTable"]!;
            string LoggingServer = ConfigurationManager.AppSettings["LoggingServer"]!;
            string LoggingTable = ConfigurationManager.AppSettings["LoggingTable"]!;
            string OTPExpirationOffsetSeconds = ConfigurationManager.AppSettings["OTPExpirationOffsetSeconds"]!;
            string HubbaEmailAddress = ConfigurationManager.AppSettings["HubbaEmailAddress"]!;
            string HubbaEmailPassword = ConfigurationManager.AppSettings["HubbaEmailPassword"]!;
            string AESKey = ConfigurationManager.AppSettings["AESKey"]!;
            string connectionString = $"Server={AccountServer};Database={AccountDatabase};Encrypt=false;User Id={AccountDataAccessUser};Password={AccountDataAccessPass}";
            RegistrationManager registrationmanager = new RegistrationManager(connectionString, UserAccountsTable);
            AuthenticationManager authenticationManager = new AuthenticationManager(connectionString, UserAccountsTable, OTPTable);
            UserAccountDataAccess userAccountDataAccess = new UserAccountDataAccess(connectionString, UserAccountsTable);
            OTPDataAccess otpDataAccess = new OTPDataAccess(connectionString, OTPTable);

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
                Result<int> loginResult = await authenticationManager.Login(email, password).ConfigureAwait(false);
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
                    return;
                }
                else
                {
                    Console.WriteLine($"[ERROR]: {otpresult.ErrorMessage}");
                }
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

            while (true)
            {
                Console.WriteLine("\n1:Attempt Registration");
                Console.WriteLine("2:Attempt Login");
                Console.WriteLine("3:Attempt OTP Entry");
                Console.WriteLine("4:Delete Login");
                Console.WriteLine("5:Delete OTP Entry");
                Console.WriteLine("6:Delete Login and OTP Entry");
                Console.WriteLine("7:Exit");
                Console.WriteLine("Choose view to access:");

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
                        Console.WriteLine("\nRegistration");
                        await Register();
                        break;
                    case 2:
                        Console.WriteLine("\nLogin");
                        await Login();
                        break;
                    case 3:
                        Console.WriteLine("\nOTP Entry");
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
