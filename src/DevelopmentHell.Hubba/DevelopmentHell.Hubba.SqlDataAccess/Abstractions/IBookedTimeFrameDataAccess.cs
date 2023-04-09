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
        Task<Result> GetBookedStartDateTime(int bookingId);
        Task<Result> GetBookedStartDateTime(int bookingId, int listingId, int availabilityId);
        Task<Result> GetBookedEndDateTime(int bookingId);
        Task<Result> GetBookedEndDateTime(int bookingId, int listingId, int availabilityId);
        Task<Result> GetBookedTimeFrames(int bookingId);
        Task<Result> GetListingBookedTimeFrames(int listingId);
        Task<Result> GetListingBookedTimeFrames(int listingId, int availabilityId);
        Task<Result> CreateBookedTimeFrame(int bookingId, int listingId, int availabilityId, DateTime startDateTime, DateTime endDateTime);
        Task<Result> UpdateBookedTimeFrame(BookedTimeFrame bookedTimeFrame);
        Task<Result> DeleteBookedTimeFrame(int bookingId, int listingId, int availabilityId, DateTime startDateTime, DateTime endDateTime);

    }
}
