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
        Task<Result> GetBookedTimeFrames(string filter, int value);
        Task<Result> CreateBookedTimeFrames(List<BookedTimeFrame> timeframes);
        Task<Result> DeleteBookedTimeFrames(int bookingId, int listingId);
    }
}
