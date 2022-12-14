using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using System.Security.Principal;

namespace DevelopmentHell.Hubba.Authorization.Service.Implementation
{
	public class AuthorizationService : IAuthorizationService
	{
		public Result authorize(IPrincipal? principal, string[]? roles = null)
		{
			Result result = new Result()
			{
				IsSuccessful = false,
			};

			// No account and no roles = authorized
			if (principal is null && roles is null)
			{
				result.IsSuccessful = true;
				return result;
			}
			// Account and no roles = authorized
			else if (principal is not null && roles is null)
			{
				result.IsSuccessful = true;
				return result;
			}
			// No account and roles = not authorized
			else if (principal is null && roles is not null)
			{
				result.IsSuccessful = false;
				return result;
			}
			// Account and roles = check if principal is in roles
			else if (principal is not null && roles is not null)
			{
				foreach (string role in roles)
				{
					if (principal.IsInRole(role))
					{
						result.IsSuccessful = principal.IsInRole(role);
						return result;
					}
				}
			}

			result.ErrorMessage = "Fatal error.";
			return result;
		}
	}
}