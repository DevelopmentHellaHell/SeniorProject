using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Implementation;

namespace DevelopmentHell.Hubba.Registration.Manager
{
	public class RegistrationManager
	{
		private IRegistrationService _registrationService;
		private ILoggerService _loggerService;
		public RegistrationManager(IRegistrationService registrationService, ILoggerService loggerService)
		{
			_registrationService = registrationService;
			_loggerService = loggerService;
		}

		public async Task<Result> Register(string email, string password)
		{
			Result result = new Result();

			Result registerResult = await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
			if (!registerResult.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = registerResult.ErrorMessage;
				return result;
			}

			_loggerService.Log(LogLevel.INFO, Category.BUSINESS, "RegistrationManager.Register", $"New registered user: {email}.");

			result.IsSuccessful = true;
			return result;
		}
	}
}