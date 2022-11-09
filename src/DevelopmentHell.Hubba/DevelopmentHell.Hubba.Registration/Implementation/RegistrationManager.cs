using System.Text.Json;
using DevelopmentHell.Hubba.Models;


namespace DevelopmentHell.Hubba.Registration
{
    public class RegistrationManager
    {
        private string _jsonString { get; set; }

        public RegistrationManager(string jsonString)
        {
            _jsonString = jsonString;
        }

        public async Task<Result> createAccount()
        {
            Account? newAccount = JsonSerializer.Deserialize<Account>(_jsonString);
            if(newAccount is null)
            {
                return new Result(false, "Unable to initialize Account from JSON data");
            }
            newAccount.AdminAccount = false;
            RegistrationService userService = new RegistrationService(newAccount);

            return await userService.RegisterAccount().ConfigureAwait(false);
        }
    }
}
// References:
// Email validation: https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format