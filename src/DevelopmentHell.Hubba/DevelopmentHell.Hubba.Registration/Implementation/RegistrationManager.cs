using System.Globalization;
using System.Text.RegularExpressions;
using DevelopmentHell.Hubba.Registration.Abstractions;
using DevelopmentHell.Hubba.Models;


namespace DevelopmentHell.Hubba.Registration
{
    public class RegistrationManager
    {
        private RegistrationService _userRegister;

        public RegistrationManager() { }

        public RegistrationManager(RegistrationService userRegister)
        {
            _userRegister = userRegister;
        }
        
        public Result Register(string email, string passphrase)
        {
            var result = new Result();

            var inputValidation = new InputValidation();

            if (inputValidation.ValidateEmail(email).IsSuccessful == false)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Email provided is invalid. Retry or contact admin";
            }
            if (inputValidation.ValidatePassphrase(passphrase).IsSuccessful == false)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Passphrase provided is invalid. Retry or contact admin";
            }
            result = _userRegister.CreateAccount(email, passphrase);
            return result;
        }
    }
}
// References:
// Email validation: https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format