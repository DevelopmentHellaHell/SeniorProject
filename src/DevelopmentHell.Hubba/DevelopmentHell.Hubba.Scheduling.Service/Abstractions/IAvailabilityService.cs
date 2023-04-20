using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Scheduling.Service.Abstractions
{
    public interface IAvailabilityService
    {
        Task<Result<bool>> CheckIfOverlapBookedTimeFrames(int listingId, int availabilityId, List<BookedTimeFrame> timeframes);
        Task<Result<List<Tuple<DateTime,DateTime>>>> GetListingAvailabilityByMonth(int listingId, int month, int year);
        Task<Result<int>> GetOwnerId(int listingId);
        
    }
}