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

        //Task<Result<double>> GetListingAverageRating(int listingId);

        Task<Result<List<ListingRatingViewDTO>>> GetListingRatings(int listingId);

        Task<Result<bool>> CheckListingRating(int listingId, int userId);


        Task<Result> AddListingAvailabilities(List<ListingAvailabilityDTO> listingAvailabilities);
        Task<Result> DeleteListingAvailabilities(List<ListingAvailabilityDTO> listingAvailabilities);

        //Task<Result<string>> GetUsername(int userId);
    }
}