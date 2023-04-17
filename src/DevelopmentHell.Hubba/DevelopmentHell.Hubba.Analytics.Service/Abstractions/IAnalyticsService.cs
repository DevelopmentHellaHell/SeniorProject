using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Analytics.Service.Abstractions
{
    public interface IAnalyticsService
    {
        Task<Result<Dictionary<string, Dictionary<string, int>>>> GetData(DateTime fromTimeMonths);
    }
}
