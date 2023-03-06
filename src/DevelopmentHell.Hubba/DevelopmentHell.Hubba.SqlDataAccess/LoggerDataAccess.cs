using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
	public class LoggerDataAccess : ILoggerDataAccess
	{
		private InsertDataAccess _insertDataAccess;
		private SelectDataAccess _selectDataAccess;
		private readonly string _tableName;

		public LoggerDataAccess(string connectionString, string tableName)
		{
			_insertDataAccess = new InsertDataAccess(connectionString);
			_selectDataAccess = new SelectDataAccess(connectionString);
			_tableName = tableName;
		}

		public async Task<Result> LogData(LogLevel logLevel, Category category, string message, string? userName = null)
		{
			if (!Enum.IsDefined(typeof(Category), category))
			{
				return new Result()
				{
					IsSuccessful = false,
					ErrorMessage = String.Format(@"{0} is not in Category Enum.", category),
				};
			}

			var logDictionary = new Dictionary<string, object>()
			{
				{ "Timestamp", DateTime.Now },
				{ "LogLevel", logLevel },
				{ "Category", category },
				{ "UserName", userName ?? Convert.DBNull },
				{ "Message", message },
			};

			var insertResult = await _insertDataAccess.Insert(_tableName, logDictionary).ConfigureAwait(false);
			if (!insertResult.IsSuccessful)
			{
				return insertResult;
			}

			return new Result()
			{
				IsSuccessful = true,
			};
		}

		public async Task<Result<List<Dictionary<string, object>>>> SelectLogs(List<string> columns, List<Comparator> filters)
		{
			var selectResult = await _selectDataAccess.Select(_tableName, columns, filters).ConfigureAwait(false);
			return selectResult;
		}
	}
}