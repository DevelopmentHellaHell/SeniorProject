using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using Microsoft.AspNetCore.Http;

namespace DevelopmentHell.ListingProfile.Manager.Abstractions
{
    public interface IListingProfileManager
    {
        Task<Result> CreateListing(string title);//

        Task<Result<List<ListingViewDTO>>> ViewUserListings();//

        Task<Result<List<object>>> ViewListing(int listingId);//only title => need to try some with listingavailabilities, files, and ratings

        Task<Result> EditListing(ListingEditorDTO listing);//

        Task<Result> EditListingAvailabilities(List<ListingAvailabilityDTO> listingAvailabilities);//

        Task<Result> EditListingFiles(List<IFormFile> listingFiles);

        Task<Result> DeleteListing(int listingId);//

        Task<Result> PublishListing(int listingId);

        Task<Result> AddRating(int listingId, int rating, string? comment, bool anonymous);//

        Task<Result> EditRating(ListingRatingEditorDTO listingRating);//

        Task<Result> DeleteRating(int listingId);//
    }
}
