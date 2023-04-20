
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class ListingHistoryDataAccess : IListingHistoryDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private UpdateDataAccess _updateDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private string _tableName;

        public ListingHistoryDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _updateDataAccess = new UpdateDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _tableName = tableName;
        }
        public async Task<Result<int>> CountListingHistory(int listingId, int userId)
        {
            Result<int> result = new Result<int>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "COUNT(*) as Users" },
                new List<Comparator> {
                    new Comparator("ListingId", "=", listingId),
                    new Comparator("UserId", "=", userId),
                }
            ).ConfigureAwait(false);
            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to retrieve count.";
                return result;
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            result.IsSuccessful = true;
            result.Payload = (int)payload[0]["Users"];
            return result;
        }

        public async Task<Result> AddUser(int listingId, int userId)
        {
            Result result = new Result();
            Result insertResult = await _insertDataAccess.Insert(
                _tableName,
                new Dictionary<string, object>()
                {
                    { "ListingId", listingId },
                    { "UserId", userId }
                }
            ).ConfigureAwait(false);

            if (!insertResult.IsSuccessful)
            {
                if (insertResult.ErrorMessage!.ToLower().Contains("foreign key constraint"))
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = "Listing doesn't exist.";
                    return result;
                }
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to add user.";
                return result;
            }

            return insertResult;
        }
        public async Task<Result> DeleteUser(int listingId, int userId)
        {
            Result deleteResult = await _deleteDataAccess.Delete(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator("ListingId", "=", listingId),
                    new Comparator("UserId", "=", userId),
                }
            ).ConfigureAwait(false);

            return deleteResult;
        }
    }
}
