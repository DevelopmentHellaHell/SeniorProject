using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Implementation;
using Microsoft.AspNetCore.Authentication;

namespace DevelopmentHell.Hubba.Registration.Manager
{
	public class RegistrationManager
	{
		private IRegistrationService _registrationService;
		private readonly string _connectionString = "Server=.;Database=DevelopmentHell.Hubba.Users;Encrypt=false;User Id=DevelopmentHell.Hubba.SqlUser.User;Password=password";

		public RegistrationManager()
		{
			_registrationService = new RegistrationService(_connectionString);
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