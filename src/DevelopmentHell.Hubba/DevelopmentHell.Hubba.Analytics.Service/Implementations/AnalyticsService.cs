using DevelopmentHell.Hubba.Analytics.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;

namespace DevelopmentHell.Hubba.Analytics.Service.Implementations
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

        private IAnalyticsDataAccess _analyticsDataAccess;
        private IAuthorizationService _authorizationService;
        private ILoggerService _loggerService;

        private Dictionary<string, Dictionary<string, int>>? _cachedData;
        private static int _queryCooldownSeconds;
        private DateTime _queryCooldownAccess;

        public AnalyticsService(IAnalyticsDataAccess analyticsDataAccess, IAuthorizationService authoirzationService, ILoggerService loggerService, int queryCooldownSeconds = 60)
        {
            _analyticsDataAccess = analyticsDataAccess;
            _authorizationService = authoirzationService;
            _loggerService = loggerService;
            _cachedData = null;
            _queryCooldownAccess = DateTime.Now;
            _queryCooldownSeconds = queryCooldownSeconds;
        }

        public async Task<Result<Dictionary<string, Dictionary<string, int>>>> GetData(DateTime fromTimeMonths)
        {
            var result = new Result<Dictionary<string, Dictionary<string, int>>>();
            if (!_authorizationService.Authorize(new string[] { "AdminUser" }).IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unauthorized.";
                return result;
            }


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
                getResults.Add(item.Key, await _analyticsDataAccess.GetDailyTotal(item.Value, fromTimeMonths).ConfigureAwait(false));
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