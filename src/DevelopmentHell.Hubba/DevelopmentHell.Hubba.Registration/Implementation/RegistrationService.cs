using DevelopmentHell.Hubba.Cryptography.Service;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Validation.Service;

namespace DevelopmentHell.Hubba.Registration.Service.Implementation
{
	public class RegistrationService : IRegistrationService
	{
		private IUserAccountDataAccess _userAccountDataAccess;
		private ILoggerService _loggerService;

		public RegistrationService(IUserAccountDataAccess userAccountDataAccess, ILoggerService loggerService)
		{
			_userAccountDataAccess = userAccountDataAccess;
			_loggerService = loggerService;
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

			Result<int> getResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
			if (!getResult.IsSuccessful)
			{

				Console.WriteLine(getResult.ErrorMessage);
				result.IsSuccessful = false;
				result.ErrorMessage = "Unable to assign username. Retry again or contact system administrator";
				return result;
			}

			if (getResult.Payload != 0)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "An account with that email already exists.";
				return result;
			}

			Random random = new((int)(DateTime.Now.Ticks << 4 >> 4));
			string salt = new(Enumerable.Repeat(HashService.saltValidChars, 64).Select(s => s[random.Next(s.Length)]).ToArray());
			HashData hashData = HashService.HashString(password, salt).Payload!;
			Result createResult = await _userAccountDataAccess.CreateUserAccount(email, hashData);
			return createResult;
		}
	}
}
