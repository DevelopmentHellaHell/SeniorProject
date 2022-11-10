using System.Text.Json;
using DevelopmentHell.Hubba.Models;


namespace DevelopmentHell.Hubba.Registration
{
    public class RegistrationManager
    {
        

        public RegistrationManager()
        {
            
        }

        public async Task<Result> createAccount(string jsonString)
        {
            Account? newAccount = JsonSerializer.Deserialize<Account>(jsonString);
            if(newAccount is null)
            {
                return new Result(false, "Unable to initialize Account from JSON data");
            }
            newAccount.AdminAccount = false;
            RegistrationService userService = new RegistrationService();

            return await userService.RegisterAccount(newAccount).ConfigureAwait(false);
        }
    }
}
// References:
// Email validation: https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format