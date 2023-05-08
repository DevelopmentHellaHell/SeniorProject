using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;

namespace DevelopmentHell.ListingProfile.Manager.Abstractions
{
    public interface IListingProfileManager
    {
        Task<Result> CreateListing(string title);
        Task<Result<List<ListingViewDTO>>> ViewUserListings();

        Task<Result<Dictionary<string, object>>> ViewListing(int listingId);

        Task<Result> EditListing(ListingEditorDTO listing);

        Task<Result> EditListingAvailabilities(ListingAvailabilitiesReactDTO listingAvailabilities);

        Task<Result> EditListingFiles(int listingId, List<string>? deleteFileNames, List<Tuple<string, string>>? addListingFiles);

        Task<Result> DeleteListing(int listingId);

        Task<Result> PublishListing(int listingId);
        Task<Result> UnpublishListing(int listingId);

        Task<Result> AddRating(int listingId, int rating, string? comment, bool anonymous);

        Task<Result> EditRating(ListingRatingEditorDTO listingRating);

        Task<Result> DeleteRating(int listingId);

        Task<Result<List<string>>> GetListingFiles(int listingId);

        Task<Result<string>> HasListingHistory(int listingId);
    }
}
