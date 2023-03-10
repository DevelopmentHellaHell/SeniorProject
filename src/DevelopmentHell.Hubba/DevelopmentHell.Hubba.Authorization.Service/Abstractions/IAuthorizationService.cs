using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Authorization.Service.Abstractions
{
	public interface IAuthorizationService
	{
		Result Authorize(string[] roles);
		Task<Result<string>> GenerateAccessToken(int accountId, bool defaultUser = false);
	}
}
