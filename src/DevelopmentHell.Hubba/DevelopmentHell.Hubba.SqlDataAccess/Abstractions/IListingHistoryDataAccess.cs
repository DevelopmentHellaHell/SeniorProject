using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface IListingHistoryDataAccess
    {
        Task<Result<int>> CountListingHistory(int listingId, int ownerId);

        Task<Result> AddUser(int listingId, int userId);

        Task<Result> DeleteUser(int listingId, int userId);
    }
}

