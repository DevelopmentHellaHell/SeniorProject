using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Authentication.Service.Abstractions
{
	public interface IAuthenticationService
	{
		Task<Result<int>> AuthenticateCredentials(string email, string password, string ipAddress);
	}
}
