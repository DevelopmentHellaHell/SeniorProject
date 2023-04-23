using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;

namespace DevelopmentHell.Hubba.Scheduling.Service.Abstractions
{
    public interface IAvailabilityService
    {
        Task<Result<bool>> ValidateChosenTimeFrames(int listingId, int availabilityId, BookedTimeFrame timeframes);
        Task<Result<List<Tuple<DateTime,DateTime>>>> GetOpenTimeSlotsByMonth(int listingId, int month, int year);
        Task<Result<BookingViewDTO>> GetListingDetails(int listingId);
    }
}