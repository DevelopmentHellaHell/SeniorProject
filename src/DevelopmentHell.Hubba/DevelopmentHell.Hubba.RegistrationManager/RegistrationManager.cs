using DevelopmentHell.Hubba.Cryptography.Service;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using System.Configuration;
using System.Security.Principal;
using System.Text;

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

		public async Task<Result> Register(string email, string password, IPrincipal? principal = null)
		{
			Result result = new Result();
			if (Thread.CurrentPrincipal is not null)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Error, user already logged in.";
				return result;
			}

			if (principal is not null)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Error, user already logged in.";
				return result;
			}

			Result registerResult = await _registrationService.RegisterAccount(email, password).ConfigureAwait(false);
			if (!registerResult.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = registerResult.ErrorMessage;
				return result;
			}

			string userHashKey = ConfigurationManager.AppSettings["UserHashKey"]!;
			Result<HashData> userHashResult = HashService.HashString(email, userHashKey);
			if (!userHashResult.IsSuccessful || userHashResult.Payload is null) {
				result.IsSuccessful = false;
				result.ErrorMessage = "Error, unexpected error. Please contact system administrator.";
				return result;
			}

			string userHash = Convert.ToBase64String(userHashResult.Payload.Hash!);
			_loggerService.Log(LogLevel.INFO, Category.BUSINESS, $"New registered user: {email}.", userHash);

			result.IsSuccessful = true;
			return result;
		}
	}
}