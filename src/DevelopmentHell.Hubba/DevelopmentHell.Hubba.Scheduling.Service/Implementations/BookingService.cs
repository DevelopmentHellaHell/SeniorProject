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

        public BookingService(IBookingDataAccess bookingDAO, IBookedTimeFrameDataAccess bookedTimeFrameDAO)
        {
            _bookingDAO = bookingDAO;
            _bookedTimeFrameDAO = bookedTimeFrameDAO;
        }

        private async Task<Result<T>> ExecuteBookingService<T>(Func<Task<Result<T>>> operation)
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

        private async Task<Result<List<Booking>>> GetBookingsByFilters(List<Tuple<string, object>> filters)
        {
            var result = new Result<List<Booking>> { IsSuccessful = false };
            var getBooking = await ExecuteBookingService(() => 
                _bookingDAO.GetBooking(filters));

            if (!getBooking.IsSuccessful)
            {
                result.ErrorMessage = getBooking.ErrorMessage;
                return result;
            }
            if (getBooking.Payload.Count < 1)
            {
                result.ErrorMessage = "No booking found";
                return result;
            }
            result.IsSuccessful = true;
            result.Payload = getBooking.Payload;
            return result;
        }
        private async Task<Result<List<BookedTimeFrame>>> GetBookedTimeFramesByFilters(List<Tuple<string, object>> filters)
        {
            return await ExecuteBookingService(() => _bookedTimeFrameDAO.GetBookedTimeFrames(filters));
        }

        public async Task<Result<int>> AddNewBooking(Booking booking)
        {
            var result = new Result<int> { IsSuccessful = false };
            if (booking == null)
            {
                result.ErrorMessage = "Empty booking object";
                return result;
            }
            var createBooking = await ExecuteBookingService(() => _bookingDAO.CreateBooking(booking));
            if (!createBooking.IsSuccessful)
            {
                result.ErrorMessage = createBooking.ErrorMessage;
            }
            else
            {
                // update booking with the inserted BookingId
                booking.BookingId = createBooking.Payload;

                // insert to BookedTimeFrames table
                var createBookedTimeFrames = await ExecuteBookingService(() =>
                    _bookedTimeFrameDAO.CreateBookedTimeFrames(booking.BookingId, booking.TimeFrames.ToList())); 
                if (!createBookedTimeFrames.IsSuccessful)
                {
                    result.ErrorMessage = createBookedTimeFrames.ErrorMessage;
                    await ExecuteBookingService (() =>
                        _bookingDAO.DeleteBooking(new List<Tuple<string, object>> 
                        { 
                            new Tuple<string, object>( nameof(Booking.BookingId), booking.BookingId) 
                        }));
                }
                else
                {
                    result.IsSuccessful = true;
                    result.Payload = booking.BookingId;
                }
            }
            return result;
        }

        public async Task<Result<List<BookedTimeFrame>>> GetBookedTimeFramesByBookingId(int bookingId)
        {
            var getBookedTimeFrames = await GetBookedTimeFramesByFilters( new List<Tuple<string, object>> 
            { 
                new Tuple<string, object>(nameof(BookedTimeFrame.BookingId), bookingId) 
            });
            return new Result<List<BookedTimeFrame>>()
            {
                IsSuccessful = true,
                Payload = getBookedTimeFrames.Payload
            };
        }

        public async Task<Result<Booking>> GetBookingByBookingId(int bookingId)
        {
            var getBooking = await GetBookingsByFilters(new List<Tuple<string, object>>
            {
                new Tuple<string, object>( nameof(Booking.BookingId), bookingId)
            });

            if (!getBooking.IsSuccessful || getBooking.Payload == null)
            {
                return new Result<Booking> { IsSuccessful = false, ErrorMessage = getBooking.ErrorMessage };
            }
            if (getBooking.Payload.Count > 1)
            {
                return new Result<Booking> { IsSuccessful = false, ErrorMessage = "More than 1 Bookings returned" };
            }
            return new Result<Booking>
            {
                IsSuccessful = true,
                Payload = getBooking.Payload[0]
            };
        }
        
        public async Task<Result<BookingStatus>> GetBookingStatusByBookingId(int bookingId)
        {
            var getBookingByBookingId = await GetBookingByBookingId(bookingId);

            return new Result<BookingStatus>
            {
                IsSuccessful = true,
                Payload = (BookingStatus)getBookingByBookingId.Payload.BookingStatusId
            };
        }

        public async Task<Result<bool>> CancelBooking(int bookingId)
        {
            var getBookingByBookingId = await GetBookingByBookingId(bookingId);
            var returnedBooking = getBookingByBookingId.Payload;
            returnedBooking.BookingStatusId = BookingStatus.CANCELLED;

            var updateBookingStatus = await ExecuteBookingService(() => _bookingDAO.UpdateBooking(returnedBooking));
            return new Result<bool> { IsSuccessful = true, Payload = updateBookingStatus.Payload};
        }
    }
}