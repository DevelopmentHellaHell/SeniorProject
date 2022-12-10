// See https://aka.ms/new-Console-template for more information

using DevelopmentHell.Hubba.Authentication.Manager;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Manager;
using DevelopmentHell.Hubba.SqlDataAccess;

Console.WriteLine("Hello, World!");
var registrationmanager = new RegistrationManager();
var authenticationmanager = new AuthenticationManager();

string connectionstring = "Server=.;Database=DevelopmentHell.Hubba.Users;Encrypt=false;User Id=DevelopmentHell.Hubba.SqlUser.User;Password=password";
var useraccountdataaccess = new UserAccountDataAccess(connectionstring);
var otpdataaccess = new OTPDataAccess(connectionstring);

string email;
string password;
while (true)
{
	Console.WriteLine("\nregistration");
	Console.Write("email: ");
	email = Console.ReadLine() ?? "";
	Console.Write("password: ");
	password = Console.ReadLine() ?? "";

	var registerresult = await registrationmanager.Register(email, password).ConfigureAwait(false);
	if (registerresult.IsSuccessful)
	{
		Console.WriteLine("register success!");
		break;
	}
	else
	{
		Console.WriteLine(registerresult.ErrorMessage);
		Result<int> getresult1 = await useraccountdataaccess.GetUserAccountIdByEmail(email).ConfigureAwait(false);
		if (getresult1.IsSuccessful)
		{
			await delete(getresult1.Payload).ConfigureAwait(false);
		}
	}
}

int accountid;
while (true)
{
	Console.WriteLine("\nlogin");
	Console.Write("email: ");
	email = Console.ReadLine() ?? "";
	Console.Write("password: ");
	password = Console.ReadLine() ?? "";
	Result<int> loginresult = await authenticationmanager.Login(email, password).ConfigureAwait(false);
	if (loginresult.IsSuccessful)
	{
		accountid = loginresult.Payload;
		Console.WriteLine("login success! sending otp.");
		break;
	}
	else
	{
		Console.WriteLine(loginresult.ErrorMessage);
	}
}

string otp;
while (true)
{
	Console.WriteLine("\none time password entry");
	otp = Console.ReadLine() ?? "";
	Result otpresult = await authenticationmanager.AuthenticateOTP(accountid, otp);
	if (otpresult.IsSuccessful)
	{
		Console.WriteLine("otp login success!");
		break;
	}
	else
	{
		Console.WriteLine(otpresult.ErrorMessage);
	}
}

await delete(accountid).ConfigureAwait(false);

async Task<bool> delete(int accountid)
{
	Result deleteotpresult = await otpdataaccess.Delete(accountid).ConfigureAwait(false);
	Result deleteaccountresult = await useraccountdataaccess.DeleteUserAccount(accountid).ConfigureAwait(false);
	if (deleteotpresult.IsSuccessful && deleteaccountresult.IsSuccessful)
	{
		Console.WriteLine("delete success!");
	}
	else
	{
		Console.WriteLine(deleteotpresult.ErrorMessage);
		Console.WriteLine(deleteaccountresult.ErrorMessage);
	}
	return true;
}