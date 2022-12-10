using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using System.Configuration;
using System.Globalization;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
	public class LoggerDataAccess : ILoggerDataAccess
	{
		private InsertDataAccess _insertDataAccess;
		private SelectDataAccess _selectDataAccess;
		private readonly string _tableName = ConfigurationManager.AppSettings["LoggingServer"];

		public LoggerDataAccess(string connectionString)
		{
			_insertDataAccess = new InsertDataAccess(connectionString);
			_selectDataAccess = new SelectDataAccess(connectionString);
		}

		public async Task<Result> LogData(LogLevel logLevel, Category category, string userName, string message)
		{
			if (!Enum.IsDefined(typeof(Category), category)) {
				return new Result(false, String.Format(@"{0} is not in Category Enum.", category));
			}

			var logDictionary = new Dictionary<string, object>()
			{
				{ "Timestamp", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)},
				{ "LogLevel", logLevel },
				{ "Category", category },
				{ "UserName", userName },
				{ "Message", message },
			};

			var insertResult = await _insertDataAccess.Insert(_tableName, logDictionary).ConfigureAwait(false);
			if (!insertResult.IsSuccessful)
			{
				return insertResult;
			}

			return new Result(true);
		}

		public async Task<Result> SelectLogs(List<string> columns, List<Comparator> filters)
		{
			var selectResult = await _selectDataAccess.Select(_tableName, columns, filters).ConfigureAwait(false);
			return selectResult;
		}
	}
}