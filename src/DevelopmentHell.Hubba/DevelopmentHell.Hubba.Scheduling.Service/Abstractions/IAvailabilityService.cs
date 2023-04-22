using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Scheduling.Service.Abstractions
{
    public interface IAvailabilityService
    {
        Task<Result<bool>> ValidateChosenTimeFrames(int listingId, int availabilityId, BookedTimeFrame timeframes);
        Task<Result<List<Tuple<DateTime,DateTime>>>> GetOpenTimeSlotsByMonth(int listingId, int month, int year);
        Task<Result<int>> GetOwnerId(int listingId);
    }
}