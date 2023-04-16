using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface IBookingStatusDataAccess
    {
        Task<Result> GetBookingStatus(int bookingStatusId);
        Task<Result> CreateBookingStatus(int bookingStatusId, string status);
        Task<Result> UpdateBookingStatus(BookingStatus bookingStatus);
        Task<Result> DeleteBookingStatus(int bookingStatusId);

    }
}
