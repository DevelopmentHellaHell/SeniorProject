using DevelopmentHell.Hubba.Models;
using System.Security.Principal;

namespace DevelopmentHell.Hubba.Authentication.Manager.Abstractions
{
	public interface IAuthenticationManager
	{
		Task<Result<bool>> Login(string email, string password, string ipAddress, IPrincipal? principal = null, bool enabledSend = true);
		Task<Result<string>> AuthenticateOTP(int accountId, string otp, string ipAddress, IPrincipal? principal = null);
	}
}
