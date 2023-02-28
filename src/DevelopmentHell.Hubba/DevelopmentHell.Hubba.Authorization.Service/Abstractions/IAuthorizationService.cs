using DevelopmentHell.Hubba.Models;
using System.Security.Principal;

namespace DevelopmentHell.Hubba.Authorization.Service.Abstractions
{
	public interface IAuthorizationService
	{
		Result authorize(IPrincipal? principal, string[]? roles = null);
		Task<Result<string>> GenerateToken(int accountId);

    }
}
