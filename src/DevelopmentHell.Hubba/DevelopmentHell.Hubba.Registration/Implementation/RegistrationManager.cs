using System.Globalization;
using System.Text.RegularExpressions;
using DevelopmentHell.Hubba.Registration.Abstractions;
using DevelopmentHell.Hubba.Models;
using System.Text.Json;


namespace DevelopmentHell.Hubba.Registration
{
    public class RegistrationManager
    {
        private string _jsonString { get; set; }

        public RegistrationManager(string jsonString)
        {
            _jsonString = jsonString;
        }

        public Result createAccount()
        {
            Account? newAccount = JsonSerializer.Deserialize<Account>(_jsonString);
            if(newAccount is null)
            {
                return new Result(false, "Unable to initialize Account from JSON data");
            }
            newAccount.adminAccount = false;
            String connectionString = @"Server=localhost\SQLEXPRESS;Database=DevelopmentHell.Hubba.Accounts;Integrated Security=True;Encrypt=False";
            RegistrationService userService = new RegistrationService(newAccount, connectionString);


            return userService.RegisterAccount();

        }
        
    }
}
// References:
// Email validation: https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format