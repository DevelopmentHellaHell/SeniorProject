using DevelopmentHell.Hubba.Models;
using System.Security.Principal;

namespace DevelopmentHell.Hubba.Registration.Service.Abstractions
{
	public interface IRegistrationService
	{
		Task<Result> RegisterAccount(string email, string password);
	}
}
