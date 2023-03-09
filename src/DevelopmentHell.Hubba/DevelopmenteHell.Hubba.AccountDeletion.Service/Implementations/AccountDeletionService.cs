using DevelopmentHell.Hubba.AccountDeletion.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.AccountDeletion.Service.Implementations
{
    public class AccountDeletionService : IAccountDeletionService
    {
        private IUserAccountDataAccess _dao;
        private ILoggerService _loggerService;

        public AccountDeletionService(IUserAccountDataAccess dao, ILoggerService loggerService)
        {
            _dao = dao;
            _loggerService = loggerService;
        }

        public async Task<Result> DeleteAccount(int accountId)
        {
            Result result = await _dao.Delete(accountId);
            if (!result.IsSuccessful)
            {
                result.ErrorMessage += "Unable to delete account. " + result.ErrorMessage;
                return result;
            }

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
    }
}
