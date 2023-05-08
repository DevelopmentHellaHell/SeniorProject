using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Scheduling.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using Microsoft.AspNetCore.Http;

namespace DevelopmentHell.Hubba.Scheduling.Service.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IListingHistoryDataAccess _listingHistoryDAO;
        private readonly IBookingsDataAccess _bookingDAO;
        private readonly IBookedTimeFramesDataAccess _bookedTimeFrameDAO;

        public BookingService(IBookingsDataAccess bookingDAO, IBookedTimeFramesDataAccess bookedTimeFrameDAO, IListingHistoryDataAccess listingHistoryDAO)
        {
            _listingHistoryDAO = listingHistoryDAO;
            _bookingDAO = bookingDAO;
            _bookedTimeFrameDAO = bookedTimeFrameDAO;
        }

        private async Task<Result<T>> ExecuteBookingService<T>(Func<Task<Result<T>>> operation)
        {
            try
            {
                var result = await operation().ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return new(Result.Failure(result.ErrorMessage!, result.StatusCode));
                }
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
                return new(Result.Failure("No booking found", getBooking.StatusCode));
            }
            return Result<List<Booking>>.Success(getBooking.Payload);
        }
        private async Task<Result<List<BookedTimeFrame>>> GetBookedTimeFramesByFilters(List<Tuple<string, object>> filters)
        {
            return await ExecuteBookingService(() => _bookedTimeFrameDAO.GetBookedTimeFrames(filters));
        }

        public async Task<Result<int>> AddNewBooking(Booking booking)
        {
            if (booking == null)
            {
                return new(Result.Failure("Empty booking"));
            }
            var createBooking = await ExecuteBookingService(() => _bookingDAO.CreateBooking(booking));
            if (!createBooking.IsSuccessful)
            {
                return new(Result.Failure(createBooking.ErrorMessage!, createBooking.StatusCode));
            }
            else
            {
                // update booking with the inserted BookingId
                booking.BookingId = createBooking.Payload;

                // insert to BookedTimeFrames table
                var createBookedTimeFrames = await ExecuteBookingService(() =>
                    _bookedTimeFrameDAO.CreateBookedTimeFrames(booking.BookingId, booking.TimeFrames!.ToList()));
                if (!createBookedTimeFrames.IsSuccessful)
                {
                    await ExecuteBookingService(() =>
                        _bookingDAO.DeleteBooking(new List<Tuple<string, object>>
                        {
                            new Tuple<string, object>( nameof(Booking.BookingId), booking.BookingId)
                        }));
                    return new(Result.Failure(createBookedTimeFrames.ErrorMessage!, createBookedTimeFrames.StatusCode));
                }
                else
                {
                    return Result<int>.Success(booking.BookingId);
                }
            }
        }
        public async Task<Result<bool>> AddUserToListingHistory(int listingId, int userId)
        {
            if (userId <= 0 || listingId <= 0)
            {
                return new(Result.Failure("Invalid parameter", StatusCodes.Status400BadRequest));
            }
            try
            {
                var updateListingHistory = await _listingHistoryDAO.AddUser(listingId, userId).ConfigureAwait(false);
                if (!updateListingHistory.IsSuccessful)
                {
                    // User already added
                    if (updateListingHistory.ErrorMessage == "Unable to add user.")
                    {
                        return Result<bool>.Success(true);
                    }
                    return new(Result.Failure(updateListingHistory.ErrorMessage!, updateListingHistory.StatusCode));
                }
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return new(Result.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> RemoveUserFromListingHistory(int listingId, int userId)
        {
            if (userId <= 0 || listingId <= 0)
            {
                return new(Result.Failure("Invalid parameter", StatusCodes.Status400BadRequest));
            }
            try
            {
                var updateListingHistory = await _listingHistoryDAO.DeleteUser(listingId, userId).ConfigureAwait(false);
                if (!updateListingHistory.IsSuccessful)
                {
                    return new(Result.Failure(updateListingHistory.ErrorMessage!, updateListingHistory.StatusCode));
                }
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return new(Result.Failure(ex.Message));
            }
        }
        public async Task<Result<List<BookedTimeFrame>>> GetBookedTimeFramesByBookingId(int bookingId)
        {
            var getBookedTimeFrames = await GetBookedTimeFramesByFilters(new List<Tuple<string, object>>
            {
                new Tuple<string, object>(nameof(BookedTimeFrame.BookingId), bookingId)
            });
            if (!getBookedTimeFrames.IsSuccessful)
            {
                return getBookedTimeFrames;
            }
            return Result<List<BookedTimeFrame>>.Success(getBookedTimeFrames.Payload!);
        }

        public async Task<Result<Booking>> GetBookingByBookingId(int bookingId)
        {
            var getBooking = await GetBookingsByFilters(new List<Tuple<string, object>>
            {
                new Tuple<string, object>( nameof(Booking.BookingId), bookingId)
            });

            if (!getBooking.IsSuccessful || getBooking.Payload == null)
            {
                return new(Result.Failure(getBooking.ErrorMessage!));
            }
            if (getBooking.Payload.Count > 1)
            {
                return new(Result.Failure("More than 1 Bookings returned"));
            }


            return Result<Booking>.Success(getBooking.Payload[0]);
        }

        public async Task<Result<BookingStatus>> GetBookingStatusByBookingId(int bookingId)
        {
            var getBookingByBookingId = await GetBookingByBookingId(bookingId);
            if (!getBookingByBookingId.IsSuccessful)
            {
                return new(Result.Failure(getBookingByBookingId.ErrorMessage!));
            }

            return Result<BookingStatus>.Success((BookingStatus)getBookingByBookingId.Payload!.BookingStatusId!);
        }

        public async Task<Result<bool>> CancelBooking(int bookingId)
        {
            Dictionary<string, object> values = new()
            {
                { nameof(Booking.BookingStatusId), (int) BookingStatus.CANCELLED}
            };
            List<Comparator> comparators = new()
            {
                new Comparator(nameof(Booking.BookingId),"=", bookingId)
            };
            // Change BookingStatus from CONFIRMED to CANCELLED
            var cancelBooking = await ExecuteBookingService(() => _bookingDAO.UpdateBooking(values, comparators));
            if (!cancelBooking.IsSuccessful)
            {
                return new(Result.Failure(cancelBooking.ErrorMessage!));
            }
            // Delete BookedTimeFrames associated with BookingId
            var deleteBookedTimeFrames = await ExecuteBookingService(() => _bookedTimeFrameDAO.DeleteBookedTimeFrames
            (
                new List<Tuple<string, object>>()
                {
                    new Tuple<string, object>(nameof(Booking.BookingId), bookingId)
                }
            )).ConfigureAwait(false);

            if (!deleteBookedTimeFrames.IsSuccessful)
            {
                // if failed to delete BookedTimeFrame, roll back and change BookingStatus back to CONFIRMED
                values[nameof(Booking.BookingStatusId)] = (int)BookingStatus.CONFIRMED;
                var rollbackConfirmBooking = await ExecuteBookingService(() => _bookingDAO.UpdateBooking(values, comparators));
                return new(Result.Failure(rollbackConfirmBooking.ErrorMessage!));
            }
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteIncompleteBooking(int bookingId)
        {
            List<Tuple<string, object>> filter = new()
            {
                new Tuple<string,object>(nameof(Booking.BookingId), bookingId)
            };
            var deleteBooking = await ExecuteBookingService(() => _bookingDAO.DeleteBooking(filter)).ConfigureAwait(false);

            return deleteBooking;
        }
    }
}