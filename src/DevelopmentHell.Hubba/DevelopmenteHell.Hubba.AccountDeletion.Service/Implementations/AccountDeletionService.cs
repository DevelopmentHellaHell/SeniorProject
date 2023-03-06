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

        public async Task<Result> DeleteAccountNotifyListingsBookings(int accountID)
        {
            Result result = await _dao.Delete(accountID);
            if (!result.IsSuccessful)
            {
                return result;
            }

            // TODO: decide Listing table column headers
            //if (futureBookingID.Count > 0 || futureListingID.Count > 0)
            //{
            //    foreach (var bookedID in futureBookingID)
            //    {
            //        // TODO NOTIFY USERS WHO HAVE BOOKED WITH THE DELETED ACCOUNT THAT THEY ARE CANCELED
            //    }
            //    foreach (var listedID in futureListingID)
            //    {
            //        // TODO NOTIFY LISTING OWNERS THAT THE DELETED ACCOUNT HAS CANCELED THEIR BOOKING
            //    }
            //}

            return result;
        }

        public async Task<Result<int>> CountAdmin()
        {
            Result<int> result = await _dao.CountAdmin();
            return result;
        }
    }
}
