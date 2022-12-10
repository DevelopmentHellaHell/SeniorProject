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
			string userFriendlyErrorMessage = "The email or password provided is invalid.";
			Result<int> result = new Result<int>();

			if (!ValidationService.ValidateEmail(email).IsSuccessful ||
				!ValidationService.ValidatePassword(password).IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = userFriendlyErrorMessage;
				return result;
			}

			HashData hashData = HashService.HashString(password).Payload;
			Result<int> getResult = await _dao.GetUserAccountIdByCredentials(email, hashData).ConfigureAwait(false);
			if (!getResult.IsSuccessful) {
				result.IsSuccessful = false;
				result.ErrorMessage = userFriendlyErrorMessage;
				return result;
			}

			result.IsSuccessful = true;
			result.Payload = getResult.Payload;
			return result;
		}
	}
}