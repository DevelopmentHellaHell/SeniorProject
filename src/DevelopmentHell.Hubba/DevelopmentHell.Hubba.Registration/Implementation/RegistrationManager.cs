using System.Configuration;
using System.Diagnostics;
using System.Text.Json;
using DevelopmentHell.Hubba.Logging.Implementation;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;

namespace DevelopmentHell.Hubba.Registration
{
    public class RegistrationManager
    {
		private readonly string _loggerConnectionString = String.Format(@"Server={0};Database=DevelopmentHell.Hubba.Logging;Encrypt=false;User Id=DevelopmentHell.Hubba.SqlUser.Registration;Password=password", ConfigurationManager.AppSettings["LoggingServer"]);
        private Logger _logger;

		public RegistrationManager()
        {
            _logger = new Logger(new LoggerDataAccess(_loggerConnectionString));
        }

        public async Task<Result> createAccount(string jsonString)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Account? newAccount = JsonSerializer.Deserialize<Account>(jsonString);
            if(newAccount is null)
            {
                return new Result(false, "Unable to initialize Account from JSON data");
            }
            newAccount.AdminAccount = false;
            RegistrationService userService = new RegistrationService();

            var registerAccountResult = await userService.RegisterAccount(newAccount).ConfigureAwait(false);
            stopwatch.Stop();
            
            if (stopwatch.ElapsedMilliseconds > 5000)
            {
                await _logger.Log(LogLevel.WARNING, Category.BUSINESS, "System", String.Format(@"Elapsed time of account creation was greater than 5 seconds. Took {0} ms", stopwatch.ElapsedMilliseconds)).ConfigureAwait(false);
            }
			return registerAccountResult;
        }
    }
}
// References:
// Email validation: https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format