

using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public enum Feature
    {
        Listing,
        Collaborator,
        Showcase
    }
    public class RatingDataAccess : IRatingDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private UpdateDataAccess _updateDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private string _tableName;



        public RatingDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _updateDataAccess = new UpdateDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _tableName = tableName;
        }

        public async Task<Result> AddRating(Feature feature, int id, int userId, int? rating, string? comment, bool? anonymous)
        {
            Result result = new Result();
            if (feature == Feature.Listing)
            {
                anonymous = anonymous ?? true;
            }
            Result insertResult = await _insertDataAccess.Insert(
                _tableName,
                new Dictionary<string, object>()
                {
                    { feature.ToString() + "Id", id },
                    { "UserId", userId },
                    { "Rating", rating },
                    { "Comment", comment },
                    { "Anonymous", anonymous },
                    { "LastEdited", DateTime.Now },
                    { "CreationDate", DateTime.Now }
                }
            ).ConfigureAwait(false);

            return insertResult;
        }

        public async Task<Result<Dictionary<int, double>>> GetAverageRating(Feature feature, int id)
        {
            Result<Dictionary<int, double>> result = new Result<Dictionary<int, double>>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { feature.ToString() + "Id", "AVG(CAST(Rating AS FLOAT)) as AvgRating" },
                new List<Comparator>()
                {
                    new Comparator(feature.ToString() + "Id", "=", id),
                },
                feature + "Id"
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Cannot retrieve average rating.";
                return result;
            }
            Dictionary<int, double> ratings = new Dictionary<int, double>();
            if (selectResult.Payload is not null)
            {
                foreach (Dictionary<string, object> ratingDict in selectResult.Payload)
                {
                    int ratingId = Convert.ToInt32(ratingDict[feature.ToString() + "Id"]);

                    double avgRating = Math.Round(Convert.ToDouble(selectResult.Payload[0]["AvgRating"]), 1, MidpointRounding.ToZero);

                    ratings.Add(ratingId, avgRating);
                }
            }
            

            result.IsSuccessful = true;
            result.Payload = ratings;
            return result;
        }

        public async Task<Result<List<ListingRating>>> GetListingRatings(int listingId)
        {
            Result<List<ListingRating>> result = new Result<List<ListingRating>>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "ListingId", "UserId", "Rating", "Comment", "Anonymous", "LastEdited", "CreationDate" },
                new List<Comparator>()
                {
                    new Comparator("ListingId", "=", listingId),
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Cannot retrieve rating.";
                return result;
            }

            result.IsSuccessful = true;
            List<ListingRating> listingRatings = new();
            List<Dictionary<string, object>> payload = selectResult.Payload;
            foreach (var listItem in payload)
            {
                var listingRating = new ListingRating()
                {
                    ListingId = (int)listItem["ListingId"],
                    UserId = (int)listItem["UserId"],
                    Comment = listItem["Comment"] == DBNull.Value ? null : (string)listItem["Comment"],
                    Rating = (int)listItem["Rating"],
                    CreationDate = (DateTime)listItem["CreationDate"],
                    LastEdited = (DateTime)listItem["LastEdited"],
                    Anonymous = (bool)listItem["Anonymous"]
                };
                listingRatings.Add(listingRating);
            }
            result.Payload = listingRatings;


            return result;
        }

        public async Task<Result<int>> CountRating(Feature feature, int id, int userId)
        {
            Result<int> result = new Result<int>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "COUNT(*) as Ratings" },
                new List<Comparator> {
                    new Comparator(feature.ToString() + "Id", "=", id),
                    new Comparator("UserId", "=", userId),
                }
            ).ConfigureAwait(false);
            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = selectResult.ErrorMessage;
                return result;
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            result.IsSuccessful = true;
            result.Payload = (int)payload[0]["Ratings"];
            return result;
        }

        public async Task<Result> DeleteRating(Feature feature, int id, int userId)
        {
            Result deleteResult = await _deleteDataAccess.Delete(
                 _tableName,
                 new List<Comparator>()
                 {
                    new Comparator(feature.ToString() + "Id", "=", id),
                    new Comparator("UserId", "=", userId),
                 }
             ).ConfigureAwait(false);

            return deleteResult;
        }

        public async Task<Result> UpdateListingRating(ListingRatingEditorDTO listingRating)
        {
            var values = new Dictionary<string, object>();
            foreach (var column in listingRating.GetType().GetProperties())
            {
                var value = column.GetValue(listingRating);
                if (column.Name == "Comment" && value is null)
                {
                    values[column.Name] = DBNull.Value;
                }
                if (value is null || column.Name == "ListingId" || column.Name == "UserId") continue;
                values[column.Name] = value;
            }

            values["LastEdited"] = DateTime.Now;

            Result updateResult = await _updateDataAccess.Update(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator("ListingId", "=", listingRating.ListingId!),
                    new Comparator("UserId", "=", listingRating.UserId!),
                },
                values
            ).ConfigureAwait(false);

            return updateResult;
        }

        public async Task<Result<List<Dictionary<string, object>>>> GetRating(Feature feature, int id, int userId)
        {
            Result<List<Dictionary<string, object>>> result = new Result<List<Dictionary<string, object>>>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "ListingId", "UserId", "Rating", "Comment", "Anonymous", "LastEdited", "CreationDate" },
                new List<Comparator>()
                {
                    new Comparator(feature.ToString() + "Id", "=", id),
                    new Comparator("UserId", "=", userId),
                }
            ).ConfigureAwait(false);
            if (!selectResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage= selectResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful= true;
            result.Payload = selectResult.Payload;
            return selectResult;
        }
    }
}