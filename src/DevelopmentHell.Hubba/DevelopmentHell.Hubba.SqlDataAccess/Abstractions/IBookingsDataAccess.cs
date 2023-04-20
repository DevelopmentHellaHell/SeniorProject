using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface IBookingsDataAccess
    {
        Task<Result<List<Booking>>> GetBooking(List<Tuple<string, object>> filter);
        Task<Result<int>> CreateBooking(Booking booking);
        Task<Result<bool>> UpdateBooking(Dictionary<string, object> values, List<Comparator> filters);
        Task<Result<bool>> DeleteBooking(List<Tuple<string, object>> filter);
    }
}
