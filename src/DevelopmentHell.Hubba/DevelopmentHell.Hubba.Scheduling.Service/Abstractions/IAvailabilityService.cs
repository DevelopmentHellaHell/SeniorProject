using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Scheduling.Service.Abstractions
{
    public interface IAvailabilityService
    {
        Task<Result> GetBookedTimeFrames(int id);
        Task<Result> GetListingAvailabilityByMonth(int listingId, int month, int year);
        Task<Result> GetOwnerId(int listingId);
    }
}