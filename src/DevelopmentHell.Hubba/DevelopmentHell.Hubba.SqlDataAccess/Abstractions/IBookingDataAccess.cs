using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface IBookingDataAccess
    {
        Task<Result> GetBookingId(int userId, int listingId);
        Task<Result> GetBookingId(int userId, int listingId, int availabilityId);
        Task<Result> GetFullPrice(int bookingId);
        Task<Result> GetBookingStatus(int bookingId);
        Task<Result> CreateBooking(Booking booking);
        Task<Result> UpdateBooking(Booking booking);
        Task<Result> DeleteBooking(int bookingId);
    }
}
