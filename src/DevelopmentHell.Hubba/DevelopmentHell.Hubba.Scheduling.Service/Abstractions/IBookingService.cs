using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Scheduling.Service.Abstractions
{
    public interface IBookingService
    {
        Task<Result> AddNewBooking(Booking booking);
        Task<Result> CancelBooking(Booking booking);
        Task<Result> GetBookings(List<Tuple<string,object>> filters);
        Task<Result> GetBookingByBookingId(int bookingId);
        Task<Result> GetBookingStatusByBookingId(int bookingId);
        Task<Result> GetBookedTimeFrames(List<Tuple<string, object>> filters);
        Task<Result> DeleteBookedTimeFrames(List<Tuple<string, object>> filters);
    }
}