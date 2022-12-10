using DevelopmentHell.Hubba.Logging.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;

namespace DevelopmentHell.Hubba.Logging.Implementation
{
	public class Logger : ILogger
    {
		private readonly ILoggerDataAccess _insertDataAccess;

        public Logger(ILoggerDataAccess insertDataAccess)
        {
            _insertDataAccess = insertDataAccess;
        }

        public async Task<Result> Log(LogLevel logLevel, Category category, string userName, string message)
        {
            if (message == null)
            {
				return new Result(true);
            }

            if (message.Length > 200)
            {
				return new Result(false, "Logging message was over 200 characters.");
            }

            if (userName.Length > 50)
            {
                return new Result(false, "Logging user was over 50 characters.");
            }

            var dataAccessResult = await _insertDataAccess.LogData(logLevel, category, userName, message).ConfigureAwait(false);
            if (!dataAccessResult.IsSuccessful)
            {
				return dataAccessResult;
            }

            return new Result(true);
        }
    }
}