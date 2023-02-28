using DevelopmentHell.Hubba.Models;
using System.Security.Principal;

namespace DevelopmentHell.Hubba.Authorization.Service.Abstractions
{
	public interface IAuthorizationService
	{
		Result authorize(IPrincipal? principal, string[]? roles = null);
		public Task<Result<string>> GenerateToken(int accountId);

    }
}
