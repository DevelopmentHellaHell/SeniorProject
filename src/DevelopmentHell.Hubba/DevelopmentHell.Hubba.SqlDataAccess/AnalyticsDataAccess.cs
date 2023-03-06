using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using System.Data.SqlTypes;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
	public class AnalyticsDataAccess : IAnalyticsDataAccess
	{
		private SelectDataAccess _selectDataAccess;
		private string _tableName;

		public AnalyticsDataAccess(string connectionString, string tableName)
		{
			_selectDataAccess = new SelectDataAccess(connectionString);
			_tableName = tableName;
		}

		public async Task<Result<List<Dictionary<string, object>>>> GetDailyTotal(string item, DateTime fromTime)
		{
			Result<List<Dictionary<string, object>>> result = new Result<List<Dictionary<string, object>>>();

			Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
				_tableName,
				new List<string>() { "MIN(convert(varchar, Timestamp, 1)) AS 'Date'", "COUNT(id) AS 'Count'" },
				new List<Comparator> {
					new Comparator("Timestamp", ">=", fromTime),
					new Comparator("Message", "LIKE", $"%{item}%"),
				},
				"DAY(Timestamp)",
				"Date ASC"
			).ConfigureAwait(false);

			if (!selectResult.IsSuccessful || selectResult.Payload is null)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = selectResult.ErrorMessage;
				return result;
			}

			List<Dictionary<string, object>> payload = selectResult.Payload;
			
			result.IsSuccessful = true;
			if (payload.Count > 0) result.Payload = payload;
			return result;
		}
	}
}
