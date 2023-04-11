using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Validation.Service.Abstractions
{
	public interface IValidationService
	{
		Result ValidateEmail(string email);
		Result ValidatePassword(string password);
		Result ValidatePhoneNumber(string phoneNumber);

	}
}
