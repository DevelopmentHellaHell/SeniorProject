using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface IRatingDataAccess
    {
        Task<Result<Dictionary<int, double>>> GetAverageRating(Feature feature, int id);

        Task<Result<List<ListingRating>>> GetListingRatings(int listingId);

        Task<Result> AddRating(Feature feature, int id, int userId, int? rating, string? comment, bool? anonymous);

        Task<Result<int>> CountRating(Feature feature, int id, int userId);

        Task<Result> DeleteRating(Feature feature, int id, int userId);

        Task<Result> UpdateListingRating(ListingRatingEditorDTO listingRating);
        Task<Result<List<Dictionary<string, object>>>> GetRating(Feature feature, int id, int userId);
    }
}