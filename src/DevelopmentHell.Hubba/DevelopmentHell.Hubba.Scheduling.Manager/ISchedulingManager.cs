using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Scheduling.Manager
{
    public interface ISchedulingManager
    {
        Task<Result<List<Tuple<DateTime,DateTime>>>> FindListingAvailabiityByMonth(int listingId, int month, int year);
        Task<Result<int>> ReserveBooking(int userId, int listingId, float fullPrice, List<BookedTimeFrame> chosenTimeframes, BookingStatus? bookingStatus);
        Task<Result<bool>> CancelBooking(int userId, int bookingId);
    }
}
