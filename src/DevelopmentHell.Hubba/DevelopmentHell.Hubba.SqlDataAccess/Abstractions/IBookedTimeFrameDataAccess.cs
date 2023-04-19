using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface IBookedTimeFrameDataAccess
    {
        Task<Result> GetBookedTimeFrames(List<Tuple<string,object>> filters);
        Task<Result> CreateBookedTimeFrames(int bookingId, List<BookedTimeFrame> timeframes);
        Task<Result> DeleteBookedTimeFrames(int bookingId, int listingId);
    }
}
