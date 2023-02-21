using DevelopmentHell.Hubba.Analytics.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;

namespace DevelopmentHell.Hubba.Analytics.Service.Implementation
{
	public class AnalyticsService : IAnalyticsService
	{
		private static Dictionary<string, string> Items = new Dictionary<string, string>()
		{
			{ "registrations", "register" },
			{ "logins", "login" },
			{ "bookings", "booking" },
			{ "listings", "listing" }
		};
		
		private IAnalyticsDataAccess _dao;
		private ILoggerService _loggerService;

		private Dictionary<string, Dictionary<string, int>>? _cachedData;
		private static int _queryCooldownSeconds;
		private DateTime _queryCooldownAccess;

		public AnalyticsService(IAnalyticsDataAccess dao, ILoggerService loggerService, int queryCooldownSeconds = 60)
		{
			_dao = dao;
			_loggerService = loggerService;
			_cachedData = null;
			_queryCooldownAccess = DateTime.Now;
			_queryCooldownSeconds = queryCooldownSeconds;
		}

		public async Task<Result<Dictionary<string, Dictionary<string, int>>>> GetData(DateTime fromTimeMonths)
		{
			var result = new Result<Dictionary<string, Dictionary<string, int>>>();
			if (_queryCooldownAccess.CompareTo(DateTime.Now) > 0 && _cachedData is not null)
			{
				result.IsSuccessful = true;
				result.Payload = _cachedData;
				return result;
			}

			_queryCooldownAccess = DateTime.Now.AddSeconds(_queryCooldownSeconds);
			result.Payload = new Dictionary<string, Dictionary<string, int>>();

			var getResults = new Dictionary<string, Result<List<Dictionary<string, object>>>>();
			foreach (var item in Items)
			{
				getResults.Add(item.Key, await _dao.GetDailyTotal(item.Value, fromTimeMonths).ConfigureAwait(false));
			}

			foreach (var getResult in getResults)
			{
				var resultData = getResult.Value.Payload;
				var processedData = new Dictionary<string, int>();
				if (getResult.Value.IsSuccessful && resultData is not null)
				{
					foreach (var i in resultData)
					{
						processedData.Add((string)i["Date"], (int)i["Count"]);
					}
				}
				result.Payload.Add(getResult.Key, processedData);
			}

			_cachedData = result.Payload;

			result.IsSuccessful = true;
			return result;
		}
	}
}