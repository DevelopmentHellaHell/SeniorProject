using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Scheduling.Service.Abstractions
{
    public interface IBookingService
    {
        Task<Result<int>> AddNewBooking(Booking booking);
        Task<Result<Booking>> GetBookingByBookingId(int bookingId);
        Task<Result<BookingStatus>> GetBookingStatusByBookingId(int bookingId);
        Task<Result<List<BookedTimeFrame>>> GetBookedTimeFramesByBookingId(int bookingId);
        Task<Result<bool>> CancelBooking(int bookingId);
        Task<Result<bool>> DeleteIncompleteBooking(int bookingId);
    }
}