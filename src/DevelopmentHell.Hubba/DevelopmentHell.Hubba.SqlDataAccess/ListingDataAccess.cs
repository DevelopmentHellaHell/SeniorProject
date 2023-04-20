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
        public async Task<Result<int>> CreateListing (ListingModel listing)
        {
            Result<int> result = new() { IsSuccessful = false };
            try
            {
                Dictionary<string,object> values = new()
                {
                    {nameof(ListingModel.OwnerId), listing.OwnerId },
                    {nameof(ListingModel.Title), listing.Title } ,
                    { nameof(ListingModel.Published), (bool)listing.Published } 
                };
                var addListing = await _insertDataAccess.InsertOutput(_tableName, values, nameof(ListingModel.ListingId)) as Result<int>;
                if(!addListing.IsSuccessful)
                {
                    result.ErrorMessage = addListing.ErrorMessage;
                    return result;
                }
                result.IsSuccessful = true;
                result.Payload = addListing.Payload;
                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                return result;
            }
        }
        public async Task<Result<ListingModel>> GetListingByListingId(int listingId)
        {
            Result<ListingModel> result = new() { IsSuccessful = false };

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
                    result.Payload = new ListingModel()
                    {
                        ListingId = (int)row["ListingId"],
                        OwnerId = (int)row["OwnerId"],
                        Title = (string)row["Title"],
                        Description = (string)row["Description"],
                        Price = Convert.ToSingle(row["Price"]),
                        Address = (string)row["Address"],
                        Published = (bool)row["Published"]
                    }; 
                }
            }
            result.IsSuccessful = true;
            return result;
        }


        public Task<Result<List<ListingAvailability>>> GetListingAvailabilityByMonth(int listingId)
        {
            throw new NotImplementedException();
        }
    }
}
