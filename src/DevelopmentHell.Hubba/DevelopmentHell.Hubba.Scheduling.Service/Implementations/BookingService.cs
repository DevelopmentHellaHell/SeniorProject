using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Scheduling.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
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
        private readonly IBookingsDataAccess _bookingDAO;
        private readonly IBookedTimeFramesDataAccess _bookedTimeFrameDAO;

        public BookingService(IBookingsDataAccess bookingDAO, IBookedTimeFramesDataAccess bookedTimeFrameDAO)
        {
            _bookingDAO = bookingDAO;
            _bookedTimeFrameDAO = bookedTimeFrameDAO;
        }

        private async Task<Result<T>> ExecuteBookingService<T>(Func<Task<Result<T>>> operation)
        {
            try
            {
                var result = await operation().ConfigureAwait(false);
                return Result<T>.Success(result.Payload!);
            }
            catch (Exception ex)
            {
                return new(Result.Failure(ex.Message));
            }
        }

        private async Task<Result<List<Booking>>> GetBookingsByFilters(List<Tuple<string, object>> filters)
        {
            var getBooking = await ExecuteBookingService(() => _bookingDAO.GetBooking(filters));

            if (!getBooking.IsSuccessful)
            {
                return getBooking;
            }
            if (getBooking.Payload!.Count < 1)
            {
                return new(Result.Failure( "No booking found"));
            }
            return Result<List<Booking>>.Success(getBooking.Payload);
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
                return new (Result.Failure("Empty booking"));
            }
            var createBooking = await ExecuteBookingService(() => _bookingDAO.CreateBooking(booking));
            if (!createBooking.IsSuccessful)
            {
                return createBooking;
            }
            else
            {
                // update booking with the inserted BookingId
                booking.BookingId = createBooking.Payload;

                // insert to BookedTimeFrames table
                var createBookedTimeFrames = await ExecuteBookingService(() =>
                    _bookedTimeFrameDAO.CreateBookedTimeFrames(booking.BookingId, booking.TimeFrames!.ToList())) as Result<bool>; 
                if (!createBookedTimeFrames.IsSuccessful)
                {
                    await ExecuteBookingService (() =>
                        _bookingDAO.DeleteBooking(new List<Tuple<string, object>> 
                        { 
                            new Tuple<string, object>( nameof(Booking.BookingId), booking.BookingId) 
                        }));
                    return new (Result.Failure(createBookedTimeFrames.ErrorMessage!));
                }
                else
                {
                    return Result<int>.Success(booking.BookingId);
                }
            }
        }

        public async Task<Result<List<BookedTimeFrame>>> GetBookedTimeFramesByBookingId(int bookingId)
        {
            var getBookedTimeFrames = await GetBookedTimeFramesByFilters( new List<Tuple<string, object>> 
            { 
                new Tuple<string, object>(nameof(BookedTimeFrame.BookingId), bookingId) 
            });
            if (!getBookedTimeFrames.IsSuccessful)
            {
                return getBookedTimeFrames;
            }
            return Result<List<BookedTimeFrame>>.Success(getBookedTimeFrames.Payload);
        }

        public async Task<Result<Booking>> GetBookingByBookingId(int bookingId)
        {
            var getBooking = await GetBookingsByFilters(new List<Tuple<string, object>>
            {
                new Tuple<string, object>( nameof(Booking.BookingId), bookingId)
            });

            if (!getBooking.IsSuccessful || getBooking.Payload == null)
            {
                return new (Result.Failure(getBooking.ErrorMessage!));
            }
            if (getBooking.Payload.Count > 1)
            {
                return new (Result.Failure("More than 1 Bookings returned" ));
            }
            return Result<Booking>.Success(getBooking.Payload[0]);
        }
        
        public async Task<Result<BookingStatus>> GetBookingStatusByBookingId(int bookingId)
        {
            var getBookingByBookingId = await GetBookingByBookingId(bookingId);
            if(!getBookingByBookingId.IsSuccessful)
            {
                return new (Result.Failure(getBookingByBookingId.ErrorMessage));
            }

            return Result<BookingStatus>.Success((BookingStatus)getBookingByBookingId.Payload.BookingStatusId);
        }

        public async Task<Result<bool>> CancelBooking(int bookingId)
        {
            Dictionary<string, object> values = new()
            {
                { nameof(Booking.BookingStatusId), BookingStatus.CANCELLED}
            };
            List<Comparator> comparators = new ()
            {
                new Comparator(nameof(Booking.BookingId),"=", bookingId) 
            };
            var updateBookingStatus = await ExecuteBookingService(() => _bookingDAO.UpdateBooking(values, comparators));
            
            return updateBookingStatus;
        }
    }
}