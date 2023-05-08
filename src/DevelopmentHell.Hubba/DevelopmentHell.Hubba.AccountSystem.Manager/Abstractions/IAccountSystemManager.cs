using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.AccountSystem.Manager.Abstractions
{
    public interface IAccountSystemManager
    {
        Task<Result> VerifyAccount();
        Task<Result> VerifyNewEmail(string newEmail);
        Task<Result> OTPVerification(string otpEntry);
        Task<Result> UpdateEmailInformation(string newEmail, string newPassword);
        Task<Result> UpdatePassword(string oldPassword, string newPassword, string newPasswordDupe);
        Task<Result> UpdateUserName(string firstName, string lastName);
        Task<Result<AccountSystemSettings>> GetAccountSettings();
        Task<Result> CancelBooking(int bookingId);
        Task<Result<List<BookingHistory>>> GetBookingHistory(int bookingCount, int page);
        Task<Result<List<Reservations>>> GetReservations(string sort, int reservationCount, int page);
        Task<Result<List<Reservations>>> GetReservationsQuery(string query);
    }
}
