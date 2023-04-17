using DevelopmentHell.Hubba.AccountDeletion.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Notification.Manager.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess;

namespace DevelopmentHell.Hubba.AccountDeletion.Service.Implementations
{
    public class AccountDeletionService : IAccountDeletionService
    {
        private IUserAccountDataAccess _dao;
        private INotificationManager _notificationManager;
        private ILoggerService _loggerService;

        public AccountDeletionService(IUserAccountDataAccess dao, INotificationManager notificationManager, ILoggerService loggerService)
        {
            _dao = dao;
            _notificationManager = notificationManager;
            _loggerService = loggerService;
        }

        public async Task<Result> DeleteAccount(int accountId)
        {
            // Storing email for notification
            var userResult = await _dao.GetUser(accountId).ConfigureAwait(false);
            string email = userResult.Payload!.Email!;

            // Deleting account
            Result result = await DeleteUser(accountId).ConfigureAwait(false);
            if (!result.IsSuccessful)
            {
                result.ErrorMessage += "Unable to delete account. " + result.ErrorMessage;
                return result;
            }

            // Deleting notifications and settings
            Result notificationDeletion = await DeleteNotifications(accountId).ConfigureAwait(false);

            // Sending account deletion notification
            if (email is not null)
                _notificationManager.DeletionEmail(email);

            return result;
        }

        // TODO: GOLD PLATING implement GetListingsBookings for notifications
        //public async Task<Result<List<Dictionary<string, object>>>> GetListingsBookings(int accountId)
        //{

        //}

        // TODO: GOLD PLATING implement NotifyListingBookings to notify affiliated users of deleted account
        //private async Task<Result> NotifyListingsBookings(int accountId, List<Dictionary<string, object>> listingsBookings)
        //{
        //    Result result = new Result(); 
        //    List<int> futureBookingId = new List<int>();
        //    List<int> futureListingId = new List<int>();
        //    foreach (var listingBooking in listingsBookings)
        //    {
        //        if (listingBooking["ListingId"] is not null && (int)listingBooking["ListingId"] == accountId)
        //        {
        //            futureListingId.Add((int)listingBooking["ListingId"]);
        //        }
        //        else if ((int)listingBooking["BookingId"] == accountId)
        //        {
        //            futureBookingId.Add((int)listingBooking["ListingId"]);
        //        }
        //    }

        //    foreach (var bookedID in futureBookingI)
        //    {
        //        // TODO NOTIFY USERS WHO HAVE BOOKED WITH THE DELETED ACCOUNT THAT THEY ARE CANCELED
        //    }
        //    foreach (var listedID in futureListingId)
        //    {
        //        // TODO NOTIFY LISTING OWNERS THAT THE DELETED ACCOUNT HAS CANCELED THEIR BOOKING
        //    }
        //    return result;
        //}

        public async Task<Result<int>> CountAdmin()
        {
            Result<int> result = await _dao.CountAdmin();
            return result;
        }

        private async Task<Result> DeleteUser(int accountId)
        {
            return await _dao.Delete(accountId).ConfigureAwait(false);
        }

        private async Task<Result> DeleteNotifications(int accountId)
        {
            Result notificationDeletion = await _notificationManager.DeleteNotificationInformation(accountId).ConfigureAwait(false);
            if (!notificationDeletion.IsSuccessful)
            {
                _loggerService.Log(Models.LogLevel.INFO, Category.BUSINESS,
                    $"Account has been deleted, but unable to delete Notifications. ID: {accountId}", null);
            }
            return notificationDeletion;
        }


    }
}
