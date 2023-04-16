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
        //Task<Result> GetBookedStartDateTime(int bookingId);
        //Task<Result> GetBookedStartDateTime(int bookingId, int listingId, int availabilityId);
        //Task<Result> GetBookedEndDateTime(int bookingId);
        //Task<Result> GetBookedEndDateTime(int bookingId, int listingId, int availabilityId);
        Task<Result> GetBookedTimeFramesByBookingId(int bookingId);
        Task<Result> GetBookedTimeFramesByListingId(int listingId);
        Task<Result> CreateSingleBookedTimeFrame(BookedTimeFrame timeframe);
        Task<Result> CreateBookedTimeFrames(List<BookedTimeFrame> timeframes);
        Task<Result> UpdateBookedTimeFrame(BookedTimeFrame bookedTimeFrame);
        Task<Result> DeleteBookedTimeFrames(int bookingId, int listingId);

    }
}
