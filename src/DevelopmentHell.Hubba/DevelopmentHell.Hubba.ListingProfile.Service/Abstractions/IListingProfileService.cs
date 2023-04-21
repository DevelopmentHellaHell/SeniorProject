using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;

namespace DevelopmentHell.Hubba.ListingProfile.Service.Abstractions
{
    public interface IListingProfileService
    {
        Task<Result> CreateListing(int ownerId, string title);

        Task<Result<ListingViewDTO>> GetListing(int listingId);

        Task<Result<int>> GetListingOwnerId(int listingId);

        Task<Result<List<ListingAvailabilityDTO>>> GetListingAvailabilities(int listingId);

        Task<Result<List<ListingViewDTO>>> GetUserListings(int ownerId);

        Task<Result> UpdateListing(ListingEditorDTO listing);

        Task<Result> UpdateListingAvailabilities(List<ListingAvailabilityDTO> listingAvailabilities);

        Task<Result> DeleteListing(int listingId);

        Task<Result> PublishListing(int listingId);

        Task<Result<bool>> CheckListingHistory(int listingId, int userId);

        Task<Result<List<ListingRatingViewDTO>>> GetListingRatings(int listingId);

        Task<Result<bool>> CheckListingRating(int listingId, int userId);

        Task<Result> AddListingAvailabilities(List<ListingAvailabilityDTO> listingAvailabilities);

        Task<Result> DeleteListingAvailabilities(List<ListingAvailabilityDTO> listingAvailabilities);

        Task<Result> DeleteRating(int listingId, int userId);

        Task<Result> UpdateRating(ListingRatingEditorDTO listingRating);

        Task<Result> AddRating(int listingId, int userId, int rating, string? comment, bool anonymous);


    }
}