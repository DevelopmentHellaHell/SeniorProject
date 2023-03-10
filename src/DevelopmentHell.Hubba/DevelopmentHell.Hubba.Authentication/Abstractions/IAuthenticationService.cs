using DevelopmentHell.Hubba.Models;
using System.Security.Principal;

namespace DevelopmentHell.Hubba.Authentication.Service.Abstractions
{
	public interface IAuthenticationService
	{
		Task<Result<int>> AuthenticateCredentials(string email, string password, string ipAddress);
		Task<Result> RegisterIpAddress(int accountId, string ipAddress);
		Result<string> Logout();
		Result<string> GenerateIdToken(int accountId, string accessToken);

	}
}
