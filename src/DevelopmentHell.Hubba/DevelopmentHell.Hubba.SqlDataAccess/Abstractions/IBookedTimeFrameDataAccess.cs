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
        Task<Result<List<BookedTimeFrame>>> GetBookedTimeFrames(List<Tuple<string,object>> filters);
        Task<Result<bool>> CreateBookedTimeFrames(int bookingId, List<BookedTimeFrame> timeframes);
        Task<Result<bool>> DeleteBookedTimeFrames(List<Tuple<string, object>> filters);
    }
}
