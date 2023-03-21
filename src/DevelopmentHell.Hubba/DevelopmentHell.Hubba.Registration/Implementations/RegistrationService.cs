using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;

namespace DevelopmentHell.Hubba.Registration.Service.Implementations
{
	public class RegistrationService : IRegistrationService
	{
		private IUserAccountDataAccess _userAccountDataAccess;
		private ICryptographyService _cryptographyService;
		private IValidationService _validationService;
		private ILoggerService _loggerService;

		public RegistrationService(IUserAccountDataAccess userAccountDataAccess, ICryptographyService cryptographyService, IValidationService validationService, ILoggerService loggerService)
		{
			_userAccountDataAccess = userAccountDataAccess;
			_cryptographyService = cryptographyService;
			_validationService = validationService;
			_loggerService = loggerService;
		}

		public async Task<Result> RegisterAccount(string email, string password)
		{
			Result result = new Result();

			if (!_validationService.ValidateEmail(email).IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Invalid email provided. Retry again or contact system administrator.";
				return result;
			}

			if (!_validationService.ValidatePassword(password).IsSuccessful)
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
			string salt = new(Enumerable.Repeat(_cryptographyService.GetSaltValidChars(), 64).Select(s => s[random.Next(s.Length)]).ToArray());
			HashData hashData = _cryptographyService.HashString(password, salt).Payload!;
			Result createResult = await _userAccountDataAccess.CreateUserAccount(email, hashData);
			return createResult;
		}
	}
}
