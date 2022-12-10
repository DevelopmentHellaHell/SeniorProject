// See https://aka.ms/new-console-template for more information

using DevelopmentHell.Hubba.Authentication.Manager;
using DevelopmentHell.Hubba.Cryptography;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Manager;
using DevelopmentHell.Hubba.SqlDataAccess;

Console.WriteLine("Hello, World!");
var registrationManager = new RegistrationManager();
var authenticationManager = new AuthenticationManager();

string email = "";
string password = "";
while (true)
{
	Console.WriteLine("\nRegistration");
	Console.Write("Email: ");
	email = Console.ReadLine() ?? "";
	Console.Write("Password: ");
	password = Console.ReadLine() ?? "";

	var registerResult = await registrationManager.Register(email, password).ConfigureAwait(false);
	if (registerResult.IsSuccessful)
	{
		Console.WriteLine("Register Success!");
		break;
	}
	else
	{
		Console.WriteLine(registerResult.ErrorMessage);
	}
}

while (true)
{
	Console.WriteLine("\nLogin");
	Console.Write("Email: ");
	email = Console.ReadLine() ?? "";
	Console.Write("Password: ");
	password = Console.ReadLine() ?? "";
	var loginResult = await authenticationManager.Login(email, password).ConfigureAwait(false);
	if (loginResult.IsSuccessful)
	{
		Console.WriteLine("Login Success!");
		break;
	}
	else
	{
		Console.WriteLine(loginResult.ErrorMessage);
	}
}

string connectionString = "Server=.;Database=DevelopmentHell.Hubba.Users;Encrypt=false;User Id=DevelopmentHell.Hubba.SqlUser.User;Password=password";
var userAccountDataAccess = new UserAccountDataAccess(connectionString);
var otpDataAccess = new OTPDataAccess(connectionString);

Result<int> getResult = await userAccountDataAccess.GetUserAccountIdByEmail(email).ConfigureAwait(false);
if (getResult.IsSuccessful)
{
	Console.WriteLine("Found Account!");
} else
{
	Console.WriteLine(getResult.ErrorMessage);
}

int accountId = getResult.Payload;
Result deleteOTPResult = await otpDataAccess.Delete(accountId).ConfigureAwait(false);
Result deleteAccountResult = await userAccountDataAccess.DeleteUserAccount(accountId).ConfigureAwait(false);
if (deleteOTPResult.IsSuccessful && deleteAccountResult.IsSuccessful)
{
	Console.WriteLine("Delete Success!");
}
else
{
	Console.WriteLine(deleteOTPResult.ErrorMessage);
	Console.WriteLine(deleteAccountResult.ErrorMessage);
}