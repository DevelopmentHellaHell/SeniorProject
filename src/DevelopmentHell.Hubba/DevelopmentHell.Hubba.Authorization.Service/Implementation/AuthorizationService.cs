using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using System.Security.Principal;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Specialized;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess;

namespace DevelopmentHell.Hubba.Authorization.Service.Implementation
{
	public class AuthorizationService : IAuthorizationService
	{
		private readonly NameValueCollection _configuration;
		IUserAccountDataAccess _userAccountDataAccess;
        public AuthorizationService(NameValueCollection configuration,IUserAccountDataAccess dac)
		{
			_configuration = configuration;
			_userAccountDataAccess = dac;
		}

		async public Task<Result<string>> GenerateToken(int accountId)
		{
			var result = await _userAccountDataAccess.GetUser(accountId).ConfigureAwait(false);
			string accountType = result.Payload!.Role!;

            try
			{
				var handler = new JwtSecurityTokenHandler();
				var descriptor = new SecurityTokenDescriptor
				{
					Subject = new ClaimsIdentity(new[] { new Claim("AccountId", accountId.ToString()), new Claim(ClaimTypes.Role, accountType) }),
					//TODO: Magic Number
					Expires = DateTime.UtcNow.AddMinutes(60),
					SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JwtKey"]!)), SecurityAlgorithms.HmacSha256Signature)
				};

				var token = handler.CreateToken(descriptor);
				return new() { IsSuccessful = true, Payload = handler.WriteToken(token) };
			} catch
			{
				return new() { IsSuccessful = false, ErrorMessage = "Unable to Generate JWT token" };
			}
		}

        public Result authorize(IPrincipal? principal, string[]? roles = null)
		{
			Result result = new Result()
			{
				IsSuccessful = false,
			};
			Console.WriteLine(principal.ToString());

			// no roles = authorized
			if (roles is null)
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