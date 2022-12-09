using DevelopmentHell.Hubba.Authentication.Abstractions;
using DevelopmentHell.Hubba.Cryptography;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.Validaition;

namespace DevelopmentHell.Hubba.Authentication.Implementation
{
	public class AuthenticationService : IAuthenticatonService
	{
		private IAuthenticationDataAccess _dao;
		public AuthenticationService(string connectionString)
		{
			_dao = new AuthenticationDataAccess(connectionString);
		}
		public async Task<Result<UserAccount>> AuthenticateCredentials(string email, string password)
		{
			if (!Validation.ValidateEmail(email).IsSuccessful ||
				!Validation.ValidatePassword(password).IsSuccessful)
			{
				return new Result<UserAccount>()
				{
					IsSuccessful = false,
					ErrorMessage = "The email or password provided is invalid.",
				};
			}

			var hashResult = Cryptography.Hash.HashString(password).Payload as HashResult;
			var getAccountResult = await _dao.SelectUserAccount(email, hashResult!.Hash, hashResult!.Salt).ConfigureAwait(false);
			return getAccountResult;
		}
	}
}