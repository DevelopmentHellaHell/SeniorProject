using DevelopmentHell.Hubba.Models;
using System.Security.Principal;

namespace DevelopmentHell.Hubba.Registration.Manager.Abstractions
{
	public interface IRegistrationManager
	{
		Task<Result> Register(string email, string password, string accountType, bool bypass);
	}
}
