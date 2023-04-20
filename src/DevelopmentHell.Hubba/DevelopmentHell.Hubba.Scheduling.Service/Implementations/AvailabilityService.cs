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
        private readonly IBookedTimeFrameDataAccess _bookedTimeFrameDAO;

        public AvailabilityService(IListingDataAccess listingDAO, IBookedTimeFrameDataAccess bookedTimeFrameDAO)
        {
            _listingDAO = listingDAO;
            _bookedTimeFrameDAO = bookedTimeFrameDAO;
        }
        private async Task<Result<T>> ExecuteAvailabilityService<T>(Func<Task<Result<T>>> operation)
        {
            var result = new Result<T> { IsSuccessful = false };
            try
            {
                result = await operation().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
        /// <summary>
        /// Check if TimeFrames already booked
        /// </summary>
        /// <param name="listingId"></param>
        /// <param name="availabilityId"></param>
        /// <param name="timeframes"></param>
        /// <returns>Bool in result.Payload</returns>
        public async Task<Result<bool>> CheckIfOverlapBookedTimeFrames(int listingId, int availabilityId, List<BookedTimeFrame> timeframes)
        {
            Result<bool> result = new() { IsSuccessful = false };
            List<Tuple<string,object>> filters = new()
            {
                new Tuple<string,object>(nameof(BookedTimeFrame.ListingId), listingId),
                new Tuple<string,object>(nameof(BookedTimeFrame.AvailabilityId),availabilityId)
            };
            // Get BookedTimeFrames by ListingId, AvailabilityId
            var getBookedTimeFrames = await ExecuteAvailabilityService(() => _bookedTimeFrameDAO.GetBookedTimeFrames(filters));

            // Check each input time frame against booked time frames
            foreach (var timeFrame in timeframes)
            {
                // Check if the time frame overlaps with any booked time frame
                foreach (var bookedTimeFrame in getBookedTimeFrames.Payload)
                {
                    if (timeFrame.StartDateTime < bookedTimeFrame.EndDateTime 
                        && timeFrame.EndDateTime > bookedTimeFrame.StartDateTime)
                    {
                        result.ErrorMessage = "Time frames already booked";
                        return result; 
                    }
                }
            }
            // All time frames are available
            result.IsSuccessful = true;
            result.Payload = true;
            return result; 
        }
        /// <summary>
        /// Get Listing Availability by Month, Year
        /// </summary>
        /// <param name="listingId"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns>List<Tuple<DateTime,DateTime> in result.Payload</returns>
        public Task<Result<List<Tuple<DateTime,DateTime>>>> GetListingAvailabilityByMonth(int listingId, int month, int year)
        {
            //TODO
            throw new NotImplementedException();
        }
        /// <summary>
        /// Get Listing by ListingId
        /// </summary>
        /// <param name="listingId"></param>
        /// <returns>OwnerId : int in result.Payload</returns>
        public async Task<Result<int>> GetOwnerId(int listingId)
        {
            Result<int> result = new() { IsSuccessful = false };
            // Get Listing by ListingId
            var getOwnerId = await ExecuteAvailabilityService(() => _listingDAO.GetListingByListingId(listingId));
                
            if (!getOwnerId.IsSuccessful) 
            {
                result.ErrorMessage = getOwnerId.ErrorMessage;
                return result;
            }
            // return OwnerId from the returned Listing
            
            result.IsSuccessful = true;
            result.Payload = getOwnerId.Payload.OwnerId;
            return result;
        }
    }
}
