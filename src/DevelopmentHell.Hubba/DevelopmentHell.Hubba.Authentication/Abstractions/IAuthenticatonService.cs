using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Authentication.Abstractions
{
	public interface IAuthenticatonService
	{
		public Task<Result<UserAccount>> AuthenticateCredentials(string email, string password);
	}
}
