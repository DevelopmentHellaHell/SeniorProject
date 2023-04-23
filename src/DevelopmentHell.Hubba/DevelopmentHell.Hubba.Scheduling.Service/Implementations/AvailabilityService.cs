using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.Scheduling.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Scheduling.Service.Implementations
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IListingsDataAccess _listingDAO;
        private readonly IListingAvailabilitiesDataAccess _availabilityDAO;
        private readonly IBookedTimeFramesDataAccess _bookedTimeFrameDAO;

        public AvailabilityService(IListingsDataAccess listingDAO, IListingAvailabilitiesDataAccess availabilityDAO, IBookedTimeFramesDataAccess bookedTimeFrameDAO)
        {
            _listingDAO = listingDAO;
            _availabilityDAO = availabilityDAO;
            _bookedTimeFrameDAO = bookedTimeFrameDAO;
        }
        private async Task<Result<T>> ExecuteAvailabilityService<T>(Func<Task<Result<T>>> operation)
        {
            try
            {
                var result = await operation().ConfigureAwait(false);
                if (!result.IsSuccessful || result.Payload == null)
                {
                    return new(Result.Failure(result.ErrorMessage));
                }
                return Result<T>.Success(result.Payload);
            }
            catch (Exception ex)
            {
                return new(Result.Failure(ex.Message));
            }
        }
        /// <summary>
        /// Check if TimeFrames already booked
        /// </summary>
        /// <param name="listingId"></param>
        /// <param name="availabilityId"></param>
        /// <param name="chosenTimeFrames"></param>
        /// <returns>Bool in result.Payload</returns>
        public async Task<Result<bool>> ValidateChosenTimeFrames(int listingId, int availabilityId, BookedTimeFrame chosenTimeFrame)
        {

            List<Tuple<string, object>> filters = new()
            {
                new Tuple<string,object>(nameof(BookedTimeFrame.ListingId), listingId),
                new Tuple<string,object>(nameof(BookedTimeFrame.AvailabilityId),availabilityId)
            };
            // Get BookedTimeFrames by ListingId, AvailabilityId
            var getBookedTimeFrames = await ExecuteAvailabilityService(() => _bookedTimeFrameDAO.GetBookedTimeFrames(filters));

            // Check if the time frame overlaps with any booked time frame
            foreach (var bookedTimeFrame in getBookedTimeFrames.Payload)
            {
                if (chosenTimeFrame.StartDateTime < bookedTimeFrame.EndDateTime
                    && chosenTimeFrame.EndDateTime > bookedTimeFrame.StartDateTime)
                {
                    return new(Result.Failure("Invalid Time Frame", StatusCodes.Status400BadRequest));
                }
            }

            // All time frames are available
            return Result<bool>.Success(true);
        }
        /// <summary>
        /// Get Listing Availability by Month, Year
        /// </summary>
        /// <param name="listingId"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns>List<Tuple<DateTime,DateTime> in result.Payload</returns>
        public async Task<Result<List<Tuple<DateTime, DateTime>>>> GetOpenTimeSlotsByMonth(int listingId, int month, int year)
        {
            var getListingAvailabilityByMonth = await ExecuteAvailabilityService(() =>
                _availabilityDAO.GetListingAvailabilitiesByMonth(listingId, month, year)).ConfigureAwait(false);
            
            // sorted list of BookedTimeFrame by ListingId
            var getBookedTimeFramesByListing = await ExecuteAvailabilityService(() => _bookedTimeFrameDAO.GetBookedTimeFrames
                (new List<Tuple<string, object>>()
                        {
                            new Tuple<string,object> (nameof(BookedTimeFrame.ListingId), listingId)
                        })
            ).ConfigureAwait(false);
            
            List<Tuple<DateTime, DateTime>> openTimeSlots = new();

            foreach (var availability in getListingAvailabilityByMonth.Payload)
            {
                var lastEnd = availability.StartTime; //latest open slots
                if (!getBookedTimeFramesByListing.IsSuccessful || getBookedTimeFramesByListing.Payload == null)
                {
                    return new(Result.Failure("Scheduling Error. Can't access Booked Time Frames."));
                }
                foreach (var bookedTimeFrame in getBookedTimeFramesByListing.Payload)
                {
                    if (availability.AvailabilityId == bookedTimeFrame.AvailabilityId)
                    {
                        if (bookedTimeFrame.StartDateTime == lastEnd) //booked time starts from the lastEnd
                        {
                            lastEnd = bookedTimeFrame.EndDateTime; //update lastEnd
                        }
                        if (bookedTimeFrame.StartDateTime > lastEnd && bookedTimeFrame.EndDateTime < availability.EndTime) //booked time in between lastEnd and availability.EndTime
                        {
                            openTimeSlots.Add(new Tuple<DateTime, DateTime>(lastEnd, bookedTimeFrame.StartDateTime));
                            lastEnd = bookedTimeFrame.EndDateTime;
                        }
                    }
                }
                if (lastEnd < availability.EndTime)
                {
                    openTimeSlots.Add(new Tuple<DateTime, DateTime>(lastEnd, availability.EndTime)); //add the remaining of the date
                }
            }
            return Result<List<Tuple<DateTime, DateTime>>>.Success(openTimeSlots);
        }
        /// <summary>
        /// Get Listing by ListingId
        /// </summary>
        /// <param name="listingId"></param>
        /// <returns>OwnerId : int in result.Payload</returns>
        public async Task<Result<BookingViewDTO>> GetListingDetails(int listingId)
        {
            // Get Listing by ListingId
            var getListings = await ExecuteAvailabilityService(() => _listingDAO.GetListing(listingId));

            if (!getListings.IsSuccessful)
            {
                return new(Result.Failure(getListings.ErrorMessage, getListings.StatusCode));
            }
            // return needed Listing's details for the Booking View
            BookingViewDTO bookingViewDTO = new()
            {
                OwnerId = getListings.Payload.OwnerId,
                ListingId = listingId,
                ListingTitle = getListings.Payload.Title,
                ListingLocation = getListings.Payload.Location
            };
            return Result<BookingViewDTO>.Success(bookingViewDTO);
        }
    }
}
