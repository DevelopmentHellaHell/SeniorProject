using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Authentication.Service.Abstractions
{
	public interface IAuthenticationService
	{
		Task<Result<int>> AuthenticateCredentials(string email, string password, string ipAddress);
		Task<Result<AuthTicket>> CreateSession(int AccountId, DateTime expiration);
		Task<Result<AuthTicket>> CreateSession(int AccountId);
		Task<Result<AuthTicket>> RenewSession(AuthTicket ticket);
		Task<Result<bool>> ValidateSession(AuthTicket ticket);
	}
}
