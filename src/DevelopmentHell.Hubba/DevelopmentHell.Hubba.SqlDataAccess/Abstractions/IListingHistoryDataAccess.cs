using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface IListingHistoryDataAccess
    {
        Task<Result<int>> CountListingHistory(int listingId, int ownerId);

        Task<Result> AddUser(int listingId, int userId);

        Task<Result> DeleteUser(int listingId, int userId);

    }
}

