using DevelopmentHell.Hubba.Authentication.Manager;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Manager;
using DevelopmentHell.Hubba.SqlDataAccess;

namespace DevelopmentHell.Hubba.Client
{
	public class DemoClass
	{
		private static RegistrationManager registrationmanager = new RegistrationManager();
		private static AuthenticationManager authenticationManager = new AuthenticationManager();

		private static string connectionString = "Server=.;Database=DevelopmentHell.Hubba.Users;Encrypt=false;User Id=DevelopmentHell.Hubba.SqlUser.User;Password=password";

		private static UserAccountDataAccess userAccountDataAccess = new UserAccountDataAccess(connectionString);
		private static OTPDataAccess otpDataAccess = new OTPDataAccess(connectionString);

		public static async Task Main(string[] args)
		{
			string email;
			string password;
			while (true)
			{
				Console.WriteLine();
				Surround("Registration View");
				Console.Write("Email: ");
				email = Console.ReadLine() ?? "";
				Console.Write("Password: ");
				password = Console.ReadLine() ?? "";

				var registerResult = await registrationmanager.Register(email, password).ConfigureAwait(false);
				if (registerResult.IsSuccessful)
				{
					Console.WriteLine("Registeration Success!");
					break;
				}
				else
				{
					Console.WriteLine(registerResult.ErrorMessage);
					Result<int> getUserAccountResult = await userAccountDataAccess.GetId(email).ConfigureAwait(false);
					if (getUserAccountResult.IsSuccessful)
					{
						await Delete(getUserAccountResult.Payload).ConfigureAwait(false);
					}
					else
					{
						Console.WriteLine($"[ERROR]: {getUserAccountResult.ErrorMessage}");
					}
				}
			}

			int accountId;
			while (true)
			{
				Console.WriteLine();
				Surround("Login View");
				Console.Write("Email: ");
				email = Console.ReadLine() ?? "";
				Console.Write("Password: ");
				password = Console.ReadLine() ?? "";
				Result<int> loginResult = await authenticationManager.Login(email, password).ConfigureAwait(false);
				if (loginResult.IsSuccessful)
				{
					accountId = loginResult.Payload;
					Console.WriteLine("Login Success! Sending OTP...");
					break;
				}
				else
				{
					Console.WriteLine($"[ERROR]: {loginResult.ErrorMessage}");
				}
			}

			string otp;
			while (true)
			{
				Console.WriteLine();
				Surround("OTP Entry View");
				Console.Write("OTP: ");
				otp = Console.ReadLine() ?? "";
				Result otpresult = await authenticationManager.AuthenticateOTP(accountId, otp);
				if (otpresult.IsSuccessful)
				{
					Console.WriteLine("OTP Login Success!");
					break;
				}
				else
				{
					Console.WriteLine($"[ERROR]: {otpresult.ErrorMessage}");
				}
			}

			await Delete(accountId).ConfigureAwait(false);
		}

		public static async Task<bool> Delete(int accountId)
		{
			Result deleteOTPResult = await otpDataAccess.Delete(accountId).ConfigureAwait(false);
			Result deleteAccountResult = await userAccountDataAccess.Delete(accountId).ConfigureAwait(false);
			if (deleteOTPResult.IsSuccessful && deleteAccountResult.IsSuccessful)
			{
				Console.WriteLine("Delete Success!");
			}
			else
			{
				Console.WriteLine($"[ERROR]: {deleteOTPResult.ErrorMessage}");
				Console.WriteLine($"[ERROR]: {deleteAccountResult.ErrorMessage}");
			}
			return true;
		}
		public static void Surround(string text, char character = '-', int spacing = 1)
		{
			int textLength = text.Length;
			int barLength = textLength + spacing * 2 + 2;
			Console.WriteLine(new String(character, barLength));
			Console.WriteLine($"{character}{new String(' ', spacing)}{text}{new String(' ', spacing)}{character}");
			Console.WriteLine(new String(character, barLength));
		}
	}
}