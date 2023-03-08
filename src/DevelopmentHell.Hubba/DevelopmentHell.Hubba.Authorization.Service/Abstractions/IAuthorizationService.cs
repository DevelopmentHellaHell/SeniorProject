using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Authorization.Service.Abstractions
{
	public interface IAuthorizationService
	{
		Result Authorize(string[] roles);
		Task<Result<string>> GenerateToken(int accountId, bool defaultUser = false);
    }
}
