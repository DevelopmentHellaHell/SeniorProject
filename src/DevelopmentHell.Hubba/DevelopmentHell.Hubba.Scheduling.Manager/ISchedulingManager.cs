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
        Task<Result> FindListingAvailabiityByMonth(int listingId, int month, int year);
        Task<Result> ReserveBooking(int userId, int listingId, float fullPrice, BookingStatus bookingStatus, int availabilityId);
        Task<Result> CancelBooking(int userId, int bookingId);
    }
}
