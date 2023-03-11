using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Registration.Service.Abstractions
{
	public interface IRegistrationService
	{
		Task<Result> RegisterAccount(string email, string password, string accountType = "VerifiedUser");
	}
}
