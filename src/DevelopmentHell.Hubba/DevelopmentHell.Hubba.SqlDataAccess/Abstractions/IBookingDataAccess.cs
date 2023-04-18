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
        Task<Result> GetBooking(List<Tuple<string, object>> filter);
        Task<Result> CreateBooking(Booking booking);
        Task<Result> UpdateBooking(Booking booking);
        Task<Result> DeleteBooking(List<Tuple<string, object>> filter);
    }
}
