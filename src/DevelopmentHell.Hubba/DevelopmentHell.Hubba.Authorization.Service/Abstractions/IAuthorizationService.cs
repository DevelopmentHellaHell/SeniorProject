using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Authorization.Service.Abstractions
{
	public interface IAuthorizationService
	{
		Result authorize(string[] roles);
		Task<Result<string>> GenerateToken(int accountId, bool defaultUser = false);

    }
}
