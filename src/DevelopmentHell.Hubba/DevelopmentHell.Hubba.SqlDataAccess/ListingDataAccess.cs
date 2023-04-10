using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<Result> CreateListing(ListingModel listing)
        {
            Result insertResult = await _insertDataAccess.Insert(
                _tableName,
                new Dictionary<string, object>()
                {
                    { "OwnerId", listing.OwnerId },
                    { "Title", listing.Title },
                    { "Description", listing.Description },
                    { "Location", listing.Location },
                    { "Price", listing.Price },
                    { "Published", listing.Published }
                }
            ).ConfigureAwait(false);

            return insertResult;
        }

        public Task<Result> DeleteListing(int listingId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> GetDescription(int listingId)
        {
            Result<string> result = new Result<string>();
            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "Description" },
                new List<Comparator>()
                {
                    new Comparator("ListingId", "=", listingId),
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = selectResult.ErrorMessage;
                return result;
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload.Count > 1)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Invalid number of Listings selected.";
                return result;
            }

            result.IsSuccessful = true;
            if (payload.Count > 0) result.Payload = (string)payload[0]["Description"];
            return result;

        }

        public Task<Result> GetLastEdited(int listingId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> GetListingId(int ownerId)
        {
            Result result = new Result();
            try
            {
                result = await SkeletonSelectColumn("OwnerId", ownerId).ConfigureAwait(false);
            }
            catch (Exception ex) 
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public Task<Result> GetLocation(int listingId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> GetPrice(int listingId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> GetPublished(int listingId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> GetTitle(int listingId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateListing(ListingModel listing)
        {
            throw new NotImplementedException();
        }

        private async Task<Result> SkeletonSelectColumn(string columnName, object compareValue)
        {

            Result<string> result = new Result<string>();
            
            
            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { columnName },
                new List<Comparator>()
                {
                    new Comparator(columnName, "=", compareValue),
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = selectResult.ErrorMessage;
                return result;
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload.Count > 1)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Invalid number of rows selected.";
                return result;
            }

            result.IsSuccessful = true;
            if (payload.Count > 0) result.Payload = (string)payload[0][columnName];
            return result;
        }
    }
}
