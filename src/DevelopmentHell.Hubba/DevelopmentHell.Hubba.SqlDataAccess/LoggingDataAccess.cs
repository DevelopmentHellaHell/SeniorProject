using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
	public class LoggingDataAccess : ILoggerDataAccess
	{
		private InsertDataAccess _insertDataAccess;
		private SelectDataAccess _selectDataAccess;
		private static string _tableName = "logs";

		public LoggingDataAccess(string connectionString)
		{
			_insertDataAccess = new InsertDataAccess(connectionString);
			_selectDataAccess = new SelectDataAccess(connectionString);
		}

		public async Task<Result> LogData(LogLevel logLevel, Category category, string userName, string message)
		{
			var logDictionary = new Dictionary<string, object>()
			{
				{ "timestamp", DateTime.Now },
				{ "logLevel", logLevel },
				{ "category", category },
				{ "userName", userName },
				{ "message", message },
			};

			var insertResult = await _insertDataAccess.Insert(_tableName, logDictionary).ConfigureAwait(false);
			if (!insertResult.IsSuccessful)
			{
				return insertResult;
			}

			return new Result(true);
		}

		public async Task<Result> SelectLog(List<string> columns, Dictionary<string, object> filters)
		{
			var selectResult = await _selectDataAccess.Select(_tableName, columns, filters).ConfigureAwait(false);
			return selectResult;
		}
	}
}