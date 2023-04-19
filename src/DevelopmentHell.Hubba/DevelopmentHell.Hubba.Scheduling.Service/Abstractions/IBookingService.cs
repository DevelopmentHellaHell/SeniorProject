using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Scheduling.Service.Abstractions
{
    public interface IBookingService
    {
        Task<Result<int>> AddNewBooking(Booking booking);
        Task<Result<bool>> CancelBooking(Booking booking);
        Task<Result<List<Booking>>> GetBookings(List<Tuple<string,object>> filters);
        Task<Result<Booking>> GetBookingByBookingId(int bookingId);
        Task<Result<BookingStatus>> GetBookingStatusByBookingId(int bookingId);
        Task<Result<List<BookedTimeFrame>>> GetBookedTimeFrames(List<Tuple<string, object>> filters);
        Task<Result<bool>> DeleteBookedTimeFrames(List<Tuple<string, object>> filters);
    }
}