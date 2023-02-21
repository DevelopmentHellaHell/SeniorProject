using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;

namespace DevelopmentHell.Hubba.Logging.Service.Implementation
{
	public class LoggerService : ILoggerService
	{
		private readonly ILoggerDataAccess _dao;

		public LoggerService(ILoggerDataAccess insertDataAccess)
		{
			_dao = insertDataAccess;
		}

		public Result Log(LogLevel logLevel, Category category, string message, string userName = "")
		{
			Result result = new Result();

			if (message == null)
			{
				result.IsSuccessful = true;
				return result;
			}

			if (userName.Length > 256)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Logging user was over 256 characters.";
				return result;
			}

			try
			{
				var dataAccessResult = _dao.LogData(logLevel, category, userName, message);
				result.IsSuccessful = true;
			}
			catch (Exception e)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = e.Message;
				return result;
			}

			result.IsSuccessful = true;
			return result;
		}
	}
}