using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Authentication.Abstractions
{
	public interface IAuthenticatonService
	{
		public Task<Result> AuthenticateCredentials(string email, string password);
	}
}
