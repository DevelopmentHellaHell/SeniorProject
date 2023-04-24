using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface IListingsDataAccess
    {
        Task<Result> CreateListing(int ownerId, string title);

        Task<Result<List<Listing>>> GetUserListings(int ownerId);

        Task<Result<Listing>> GetListing(int listingId);

        Task<Result> UpdateListing(ListingEditorDTO listing);

        Task<Result<int>> GetListingOwnerId(int listingId);

        Task<Result> DeleteListing(int listingId);
        Task<Result> PublishListing(int listingId);

        Task<Result<int>> GetListingId(int ownerId, string title);
        Task<Result<List<Dictionary<string, object>>>> Curate(int offset = 0);
        Task<Result<List<Dictionary<string, object>>>> Search(string query, int offset = 0, double FTTWeight = 0.5, double RWeight = 0.25, double RCWeight = 0.25);
        Task<Result> UnpublishListing(int listingId);
    }
}
