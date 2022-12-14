using DevelopmentHell.Hubba.Models;
using System.Security.Principal;

namespace DevelopmentHell.Hubba.Authentication.Service.Abstractions
{
	public interface IAuthenticationService
	{
		Task<Result<int>> AuthenticateCredentials(string email, string password, string ipAddress);
		Result<GenericPrincipal> CreateSession(int AccountId);
	}
}
