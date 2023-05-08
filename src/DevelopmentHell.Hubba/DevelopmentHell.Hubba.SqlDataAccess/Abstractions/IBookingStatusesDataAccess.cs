using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface IBookingStatusesDataAccess
    {
        Task<Result> GetBookingStatus(int bookingStatusId);
        Task<Result> CreateBookingStatus(int bookingStatusId, string status);
        Task<Result> UpdateBookingStatus(BookingStatus bookingStatus);
        Task<Result> DeleteBookingStatus(int bookingStatusId);

    }
}
