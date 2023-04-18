using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;

namespace DevelopmentHell.ListingProfile.Manager.Abstractions
{
    public interface IListingProfileManager
    {
        Task<Result> CreateListing(string title);

        Task<Result> ViewUserListings();

        Task<Result> ViewListing(int listingId);

        Task<Result> EditListing(ListingEditorDTO listing);

        Task<Result> EditListingAvailabilities(ListingAvailabilityDTO[] listingAvailabilities);

        Task<Result> EditListingFiles(HubbaFile[] listingFiles);

        Task<Result> DeleteListing(int listingId);

        Task<Result> PublishListing(int listingId);

        Task<Result> AddRating(int listingId, int rating, string? comment, bool anonymous);

        Task<Result> EditRating(ListingRatingEditorDTO listingRating);

        Task<Result> DeleteRating(int listingId);
    }
}
