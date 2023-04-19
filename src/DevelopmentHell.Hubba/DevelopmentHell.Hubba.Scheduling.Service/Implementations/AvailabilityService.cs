using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Scheduling.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Scheduling.Service.Implementations
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IListingDataAccess _listingDAO;
        private readonly IBookingDataAccess _bookingDAO;
        private readonly IBookedTimeFrameDataAccess _bookedTimeFrameDAO;

        public AvailabilityService(IListingDataAccess listingDAO, IBookingDataAccess bookingDAO, IBookedTimeFrameDataAccess bookedTimeFrameDAO)
        {
            _listingDAO = listingDAO;
            _bookingDAO = bookingDAO;
            _bookedTimeFrameDAO = bookedTimeFrameDAO;
        }
        /// <summary>
        /// Check if TimeFrames already booked
        /// </summary>
        /// <param name="listingId"></param>
        /// <param name="availabilityId"></param>
        /// <param name="timeframes"></param>
        /// <returns>Bool in result.Payload</returns>
        public async Task<Result> AreTimeFramesBooked(int listingId, int availabilityId, List<BookedTimeFrame> timeframes)
        {
            Result result = new() { IsSuccessful = false };
            List<Tuple<string,object>> filters = new()
            {
                new Tuple<string,object>("ListingId", listingId),
                new Tuple<string,object>("AvailabilityId",availabilityId)
            };
            // Get BookedTimeFrames by ListingId, AvailabilityId
            var getBookedTimeFrames = await _bookedTimeFrameDAO.GetBookedTimeFrames(filters).ConfigureAwait(false);

            // Check each input time frame against booked time frames
            foreach (var timeFrame in timeframes)
            {
                // Check if the time frame overlaps with any booked time frame
                foreach (var bookedTimeFrame in ((Result<List<BookedTimeFrame>>)getBookedTimeFrames).Payload)
                {
                    if (timeFrame.StartDateTime < bookedTimeFrame.EndDateTime && timeFrame.EndDateTime > bookedTimeFrame.StartDateTime)
                    {
                        result.ErrorMessage = "Time frames already booked";
                        return result; 
                    }
                }
            }
            // All time frames are available
            result.IsSuccessful = true;
            return result; 
        }
        /// <summary>
        /// Get Listing Availability by Month, Year
        /// </summary>
        /// <param name="listingId"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns>List<Tuple<DateTime,DateTime> in result.Payload</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<Result> GetListingAvailabilityByMonth(int listingId, int month, int year)
        {
            //TODO
            throw new NotImplementedException();
        }
        /// <summary>
        /// Get Listing by ListingId
        /// </summary>
        /// <param name="listingId"></param>
        /// <returns>OwnerId : int in result.Payload</returns>
        public async Task<Result> GetOwnerId(int listingId)
        {
            Result<int> result = new() { IsSuccessful = false };
            // Get Listing by ListingId
            var getOwnerId = await _listingDAO.GetListing(listingId).ConfigureAwait(false);
            if (!getOwnerId.IsSuccessful) 
            {
                return getOwnerId;
            }
            // return OwnerId from the returned Listing
            var listingResult = ((Result<List<ListingModel>>)getOwnerId).Payload[0];
            result.IsSuccessful = true;
            result.Payload = listingResult.OwnerId;
            return result;
        }
    }
}
