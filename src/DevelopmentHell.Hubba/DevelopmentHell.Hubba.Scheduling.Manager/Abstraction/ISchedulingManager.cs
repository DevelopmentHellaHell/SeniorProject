using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Scheduling.Manager.Abstraction
{
    public interface ISchedulingManager
    {
        Task<Result<List<ListingAvailabilityDTO>>> FindListingAvailabiityByMonth(int listingId, int month, int year);
        Task<Result<BookingViewDTO>> ReserveBooking(int userId, int listingId, float fullPrice, List<BookedTimeFrame> chosenTimeframes, BookingStatus bookingStatus = BookingStatus.CONFIRMED);
        Task<Result<bool>> CancelBooking(int userId, int bookingId);
    }
}
