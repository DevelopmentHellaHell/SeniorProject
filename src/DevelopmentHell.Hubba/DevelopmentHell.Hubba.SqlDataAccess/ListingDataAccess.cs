using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class ListingDataAccess : IListingDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private UpdateDataAccess _updateDataAccess;
        private string _tableName;

        public ListingDataAccess(string connectionString, string tableName)
        {
            _tableName = tableName;
            _insertDataAccess = new InsertDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _updateDataAccess = new UpdateDataAccess(connectionString);
        }

        public async Task<Result> GetListing(int listingId)
        {
            Result<List<ListingModel>> result = new();
            result.Payload = new List<ListingModel>();

            List<Comparator> comparators = new()
            {
                new Comparator("ListingId", "=", listingId)
            };

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>()
                {
                    "ListingId",
                    "OwnerId",
                    "Title",
                    "Description",
                    "Price",
                    "Address",
                    "Published"
                },
                comparators
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = selectResult.ErrorMessage;
                return result;
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload.Count < 1)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "No listing found";
                return result;
            }
            else
            {
                foreach (var row in payload)
                {
                    result.Payload.Add(new ListingModel()
                    {
                        ListingId = (int)row["ListingId"],
                        OwnerId = (int)row["OwnerId"],
                        Title = (string)row["Title"],
                        Description = (string)row["Description"],
                        Price = Convert.ToSingle(row["Price"]),
                        Address = (string)row["Address"],
                        Published = (bool)row["Published"]
                    }); ;
                }
            }
            result.IsSuccessful = true;
            return result;
        }


        public Task<Result> GetListingAvailabilityByMonth(int listingId)
        {
            throw new NotImplementedException();
        }
    }
}
