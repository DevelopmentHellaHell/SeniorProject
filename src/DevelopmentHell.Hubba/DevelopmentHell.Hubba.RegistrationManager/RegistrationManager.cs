using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Implementation;

namespace DevelopmentHell.Hubba.Registration.Manager
{
	public class RegistrationManager
	{
		private IRegistrationService _registrationService;

		public RegistrationManager(string connectionString, string accountsTableName)
		{
			_registrationService = new RegistrationService(connectionString,accountsTableName);
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

			result.IsSuccessful = true;
			return result;
		}
	}
}