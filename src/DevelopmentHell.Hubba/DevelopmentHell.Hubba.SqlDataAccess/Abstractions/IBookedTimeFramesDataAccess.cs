using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface IBookedTimeFramesDataAccess
    {
        Task<Result<List<BookedTimeFrame>>> GetBookedTimeFrames(List<Tuple<string, object>> filters);
        Task<Result<bool>> CreateBookedTimeFrames(int bookingId, List<BookedTimeFrame> timeframes);
        Task<Result<bool>> DeleteBookedTimeFrames(List<Tuple<string, object>> filters);
    }
}
