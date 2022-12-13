using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Authentication.Service.Abstractions
{
	public interface IAuthenticationService
	{
		Task<Result<int>> AuthenticateCredentials(string email, string password, string ipAddress);
		Task<Result<AuthCookieTicket>> CreateSession(int AccountId, DateTime expiration);
		Task<Result<AuthCookieTicket>> CreateSession(int AccountId);
		Task<Result<AuthCookieTicket>> RenewSession(AuthCookieTicket ticket);
		Task<Result<bool>> ValidateSession(AuthCookieTicket ticket);
	}
}
