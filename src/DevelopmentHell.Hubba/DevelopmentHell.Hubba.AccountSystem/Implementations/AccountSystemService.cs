using DevelopmentHell.Hubba.AccountSystem.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;

namespace DevelopmentHell.Hubba.AccountSystem.Implementations
{
    public class AccountSystemService : IAccountSystemService
    {
        private IUserAccountDataAccess _userAccountDataAccess;
        private IBookingsDataAccess _bookingDataAccess;
        private IListingHistoryDataAccess _listHistoryDataAccess;
        private ILoggerService _loggerService;

        public AccountSystemService(IUserAccountDataAccess userAccountDataAccess, IBookingsDataAccess bookingsDataAccess, IListingHistoryDataAccess listingHistoryDataAccess, ILoggerService loggerService)
        {
            _userAccountDataAccess = userAccountDataAccess;
            _bookingDataAccess = bookingsDataAccess;
            _listHistoryDataAccess = listingHistoryDataAccess;
            _loggerService = loggerService;
        }
        //TODO: Remember to write what needs to be checked in this layer
        public async Task<Result> UpdateEmailInformation(int userId, string newEmail)
        {
            Result result = new Result();
            return await _userAccountDataAccess.SaveEmailAlterations(userId, newEmail).ConfigureAwait(false);
        }

        public async Task<Result<PasswordInformation>> GetPasswordData(int userId)
        {
            return await _userAccountDataAccess.GetPasswordData(userId).ConfigureAwait(false);
        }

        public async Task<Result> UpdatePassword(string newHashPassword, string email)
        {
            Result result = new Result();
            return await _userAccountDataAccess.SavePassword(newHashPassword, email).ConfigureAwait(false);
        }

        public async Task<Result> UpdateUserName(int userId, string? firstName, string? lastName)
        {
            Result result = new Result();
            if (firstName == null && lastName == null) //MOVE
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Please enter a valid name for First Name and/or Last Name. ";
                return result;
            }
            Result updateResult = await _userAccountDataAccess.UpdateUserName(userId, firstName!, lastName!).ConfigureAwait(false);
            if (!updateResult.IsSuccessful) 
            {
                result.IsSuccessful = false;
                result.ErrorMessage = updateResult.ErrorMessage;
                return result;
            }
            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result<AccountSystemSettings>> GetAccountSettings(int userId)
        {
            Result<AccountSystemSettings> result = new Result<AccountSystemSettings>();
            Result<AccountSystemSettings> getResult = await _userAccountDataAccess.GetAccountSettings(userId).ConfigureAwait(false);
            if (!getResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = getResult.ErrorMessage;
                return result;
            }
            result.IsSuccessful = true;
            result.Payload = getResult.Payload;
            return result;
        }

        public async Task<Result> CheckNewEmail(string newEmail)
        {
            Result<int> result = new Result<int>();
            Result<int> checkEmailResult = await _userAccountDataAccess.GetId(newEmail).ConfigureAwait(false);
            if (checkEmailResult.IsSuccessful && (checkEmailResult.Payload != 0))
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "An email is already registered with this account. ";
                return result;
            }
            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result<List<BookingHistory>>> GetBookingHistory(int userId)
        {
            return await _bookingDataAccess.GetBookingHistory(userId).ConfigureAwait(false);
        }

        public async Task<Result<List<Reservations>>> GetReservations(int ownerId)
        {
            return await _listHistoryDataAccess.GetReservations(ownerId).ConfigureAwait(false);
        }

    }
}
