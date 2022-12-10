using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;

namespace DevelopmentHell.Hubba.Logging.Service.Implementation
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

		public async Task<Result> Log(LogLevel logLevel, string userName, string message)
		{
			Result result = new Result();

			if (message == null)
			{
				result.IsSuccessful = true;
				return result;
			}

			var dataAccessResult = await _dataAccess.LogData(logLevel, _category, userName, message).ConfigureAwait(false);
			if (!dataAccessResult.IsSuccessful)
			{
				return dataAccessResult;
			}

			result.IsSuccessful = true;
			return result;
		}
	}
}