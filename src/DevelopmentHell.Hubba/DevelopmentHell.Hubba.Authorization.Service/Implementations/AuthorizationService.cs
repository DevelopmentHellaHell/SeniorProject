﻿using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Collections.Specialized;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;

namespace DevelopmentHell.Hubba.Authorization.Service.Implementations
{
	public class AuthorizationService : IAuthorizationService
	{
		private readonly string _jwtKey;
		private IUserAccountDataAccess _userAccountDataAccess;
		private ILoggerService _loggerService;

        public AuthorizationService(string jwtKey, IUserAccountDataAccess userAccountDataAccess, ILoggerService loggerService)
		{
			_jwtKey = jwtKey;
			_userAccountDataAccess = userAccountDataAccess;
			_loggerService = loggerService;
		}

		public async Task<Result<string>> GenerateToken(int accountId, bool defaultUser = false)
		{
			var result = new Result<string>();

			var getResult = await _userAccountDataAccess.GetUser(accountId).ConfigureAwait(false);
			if (!getResult.IsSuccessful || getResult.Payload is null)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Invalid email.";
				return result;
			}

			var role = getResult.Payload.Role;
			var email = getResult.Payload.Email;
			if (role is null || email is null)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Empty user.";
				return result;
			}

            try
			{
				var handler = new JwtSecurityTokenHandler();
				var date = DateTime.Now;
				var descriptor = new SecurityTokenDescriptor
				{
					Subject = new ClaimsIdentity(new[] { new Claim("accountId", accountId.ToString()), new Claim(ClaimTypes.Role, defaultUser ? "DefaultUser" : role), new Claim(ClaimTypes.Email, email) }),
					//TODO: Magic Number
					Expires = date.AddMinutes(!defaultUser ? 60 : 2),
					NotBefore = date,
					SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtKey)), SecurityAlgorithms.HmacSha256Signature)
				};

				var token = handler.CreateToken(descriptor);
				return new() { IsSuccessful = true, Payload = handler.WriteToken(token) };
			}
			catch
			{
				return new() { IsSuccessful = false, ErrorMessage = "Unable to Generate access token." };
			}
		}

        public Result Authorize(string[] roles)
		{
			Result result = new Result()
			{
				IsSuccessful = false,
			};

			var principal = Thread.CurrentPrincipal as ClaimsPrincipal;

			if (principal is null)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Unauthorized.";
				return result;
			}
			
			if (principal is not null)
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

			result.IsSuccessful = false;
			result.ErrorMessage = "Unauthorized.";
			return result;
		}
	}
}