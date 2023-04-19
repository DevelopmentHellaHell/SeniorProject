using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Scheduling.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Scheduling.Service.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IBookingDataAccess _bookingDAO;
        private readonly IBookedTimeFrameDataAccess _bookedTimeFrameDAO;
        private readonly ILoggerService? _loggerService;

        public BookingService (IBookingDataAccess bookingDAO, IBookedTimeFrameDataAccess bookedTimeFrameDAO)
        {
            _bookingDAO = bookingDAO;
            _bookedTimeFrameDAO = bookedTimeFrameDAO;
        }
        /// <summary>
        /// Method receive Booking object, 
        /// call DAL to insert 1 entry to Bookings table and
        /// number of entries to BookedTimeFrames table
        /// </summary>
        /// <param name="booking"></param>
        /// <returns>bookingId:int</returns>
        public async Task<Result> AddNewBooking(Booking booking)
        {
            Result result = new() { IsSuccessful = false};
            if (booking == null)
            {
                result.ErrorMessage = "Empty booking object";
                return result;
            }
            //TODO: Check listingId, supposedly there's FK constraint
            try
            {
                // Insert 1 row to Bookings table, get BookingId of the inserted row
                var createBooking = await _bookingDAO.CreateBooking(booking);
                if (!createBooking.IsSuccessful)
                {
                    result.ErrorMessage = createBooking.ErrorMessage;
                }
                int bookingId = ((Result<int>)createBooking).Payload;
                booking.BookingId = bookingId;
                
                // Insert to BookedTimeFrames table
                var createBookedTimeFrames = await _bookedTimeFrameDAO.CreateBookedTimeFrames(booking.BookingId,booking.TimeFrames.ToList());
                if (!createBookedTimeFrames.IsSuccessful) 
                {
                    result.ErrorMessage = createBookedTimeFrames.ErrorMessage;
                    // Rollback and delete the inserted booking
                    List<Tuple<string, object>> filter = new() { new Tuple<string, object>(nameof(Booking.BookingId), bookingId) };
                    await _bookingDAO.DeleteBooking(filter);
                    return result;
                }
                return new Result<int>()
                {
                    IsSuccessful = true,
                    Payload = bookingId
                };
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                return result;
            }
        }

        public Task<Result> DeleteBookedTimeFrames(List<Tuple<string, object>> filters)
        {
            //TODO: implement
            throw new NotImplementedException();
        }

        public Task<Result> GetBookedTimeFrames(List<Tuple<string, object>> filters)
        {
            //TODO: implement
            throw new NotImplementedException();
        }
        /// <summary>
        /// Get Bookings by parameter
        /// </summary>
        /// <param name="filters"></param>
        /// <returns>List<Booking> in result.Payload</returns>
        public async Task<Result> GetBookings(List<Tuple<string,object>> filters)
        {
            Result result = new() { IsSuccessful = false };
            // Get Bookings from DAL
            var getBooking = await _bookingDAO.GetBooking(filters).ConfigureAwait(false);
            if (!getBooking.IsSuccessful) 
            {
                result.ErrorMessage = getBooking.ErrorMessage;
                return result;
            }
            // Process DAO
            var getBookingList = (Result<List<Booking>>)getBooking;
            if(getBookingList.Payload.Count < 1)
            {
                result.ErrorMessage = "No booking found";
                return result;
            }
            return getBookingList;
        }
        /// <summary>
        /// Get Bookings by BookingId
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns>Booking object in result.Payload</returns>
        public async Task<Result> GetBookingByBookingId(int bookingId)
        {
            Result result = new() { IsSuccessful = false };
            // Get Booking by parameter 
            var getBooking = await GetBookings
                (
                    new List<Tuple<string, object>> ()
                    {
                        new Tuple<string,object> ((nameof(Booking.BookingId)), bookingId)
                    }
                ).ConfigureAwait(false);
            if (!getBooking.IsSuccessful)
            {
                result.ErrorMessage = getBooking.ErrorMessage;
                return result;
            }
            // Process DAO return
            var getBookingList = (Result<List<Booking>>)getBooking;
            if (getBookingList.Payload.Count > 1)
            {
                result.ErrorMessage = "More than 1 Bookings returned";
                return result;
            }
            else if (getBookingList.Payload.Count == 0)
            {
                result.ErrorMessage = "No booking found";
                return result;
            }
            return new Result<Booking>
            {
                IsSuccessful = true,
                Payload = getBookingList.Payload[0]
            };
        }
        /// <summary>
        /// Get BookingStatus by BookingId
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns>BookingStatus in result.Payload</returns>
        public async Task<Result> GetBookingStatusByBookingId(int bookingId)
        {
            Result result = new() { IsSuccessful = false };
            // Get Booking by BookingId
            var getBooking = await GetBookingByBookingId(bookingId).ConfigureAwait(false);
            if ( !getBooking.IsSuccessful) 
            { 
                result.ErrorMessage = getBooking.ErrorMessage;
                return result;
            }
            // Process DAO return
            var bookingResult = ((Result<Booking>) getBooking).Payload;
            
            return new Result<BookingStatus>()
            {
                IsSuccessful = true,
                Payload = (BookingStatus)bookingResult.BookingStatusId
            };
        }
        /// <summary>
        /// Cancel Booking by Updating BookingStatus from CONFIRMED to CANCELLED
        /// </summary>
        /// <param name="booking"></param>
        /// <returns>Bool in result.Payload</returns>
        public async Task<Result> CancelBooking(Booking booking)
        {
            Result result = new() { IsSuccessful = false };
            if(booking == null)
            {
                result.ErrorMessage = "Null object";
                return result;
            };
            return await _bookingDAO.UpdateBooking(booking);
        }
    }
}
