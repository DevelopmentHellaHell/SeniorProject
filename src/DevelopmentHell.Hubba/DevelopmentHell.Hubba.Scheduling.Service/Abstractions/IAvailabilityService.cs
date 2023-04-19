using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Scheduling.Service.Abstractions
{
    public interface IAvailabilityService
    {
        Task<Result> AreTimeFramesBooked(int listingId, int availabilityId, List<BookedTimeFrame> timeframes);
        Task<Result> GetListingAvailabilityByMonth(int listingId, int month, int year);
        Task<Result> GetOwnerId(int listingId);
        
    }
}