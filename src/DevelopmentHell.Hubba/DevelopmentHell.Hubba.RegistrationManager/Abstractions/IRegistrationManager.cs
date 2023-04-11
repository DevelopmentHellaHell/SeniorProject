using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Registration.Manager.Abstractions
{
	public interface IRegistrationManager
	{
		Task<Result> Register(string email, string password);
	}
}
