using DevelopmentHell.Hubba.Cryptography.Service;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Validation.Service;

namespace DevelopmentHell.Hubba.Registration.Service.Implementation
{
	public class RegistrationService : IRegistrationService
	{
		private IUserAccountDataAccess _dao;

		public RegistrationService(string connectionString)
		{
			_dao = new UserAccountDataAccess(connectionString);
		}

		public async Task<Result> RegisterAccount(string email, string password)
		{
			Result result = new Result();
			if (!ValidationService.ValidateEmail(email).IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Invalid email provided. Retry again or contact system administrator.";
				return result;
			}

			if (!ValidationService.ValidatePassword(password).IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Invalid password provided. Retry again or contact system administrator.";
				return result;
			}

			Result<int> getResult = await _dao.GetUserAccountIdByEmail(email).ConfigureAwait(false);
			if (getResult.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "An account with that email already exists.";
				return result;
			}

			HashData hashData = HashService.HashString(password).Payload;
			Result createResult = await _dao.CreateUserAccount(email, hashData);
			return createResult;
		}
	}
}
