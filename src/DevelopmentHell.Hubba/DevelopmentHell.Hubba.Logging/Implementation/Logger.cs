using DevelopmentHell.Hubba.Logging.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;

namespace DevelopmentHell.Hubba.Logging.Implementation
{
	public class Logger : ILogger
    {
		private readonly ILoggerDataAccess _dataAccess;
        private readonly Category _category;

        public Logger(ILoggerDataAccess dataAccess, Category category)
        {
            _dataAccess = dataAccess;
            _category = category;
        }

        public async Task<Result> Log(LogLevel level, string user, string message)
        {
            if (message == null)
            {
				return new Result(true);
            }

            if (message.Length > 200)
            {
				return new Result(false, "Logging message was over 200 characters.");
            }

            if (user.Length > 50)
            {
                return new Result(false, "Logging user was over 50 characters.");
            }

            var dataAccessResult = await _dataAccess.LogData(level, _category, user, message).ConfigureAwait(false);
            if (!dataAccessResult.IsSuccessful)
            {
				return dataAccessResult; //TODO error msg
            }

            return new Result(true);
        }
    }
}