using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
	public class LoggerDataAccess : ILoggerDataAccess
	{
		private InsertDataAccess _insertDataAccess;
		private SelectDataAccess _selectDataAccess;
		private static string _tableName = "logs";

		public LoggerDataAccess(string connectionString)
		{
			_insertDataAccess = new InsertDataAccess(connectionString);
			_selectDataAccess = new SelectDataAccess(connectionString);
		}

		public async Task<Result> LogData(LogLevel logLevel, Category category, string userName, string message)
		{
			var logDictionary = new Dictionary<string, object>()
			{
				{ "Timestamp", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",CultureInfo.InvariantCulture)},
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

		public async Task<Result> SelectLogs(List<string> columns, Dictionary<string, object> filters)
		{
			var selectResult = await _selectDataAccess.Select(_tableName, columns, filters).ConfigureAwait(false);
			return selectResult;
		}
	}
}