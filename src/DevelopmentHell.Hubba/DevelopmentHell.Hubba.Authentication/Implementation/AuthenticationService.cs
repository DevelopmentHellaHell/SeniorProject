using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Validation.Service;

namespace DevelopmentHell.Hubba.Authentication.Service.Implementation
{
	public class AuthenticationService : IAuthenticatonService
	{
		private IUserAccountDataAccess _dao;
		public AuthenticationService(string connectionString)
		{
			_dao = new UserAccountDataAccess(connectionString);
		}

		public async Task<Result<int>> AuthenticateCredentials(string email, string password)
		{

			Result<int> result = new Result<int>();
			if (!ValidationService.ValidateEmail(email).IsSuccessful ||
				!ValidationService.ValidatePassword(password).IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "The email or password provided is invalid.";
				return result;
			}

			HashData hashData = HashService.HashString(password).Payload;
			Result<int> getResult = await _dao.GetUserAccountIdByCredentials(email, hashData).ConfigureAwait(false);
			return getResult;
		}
	}
}