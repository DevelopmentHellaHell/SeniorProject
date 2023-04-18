
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class ListingsDataAccess : IListingDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private UpdateDataAccess _updateDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private string _tableName;

        public ListingsDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _updateDataAccess = new UpdateDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _tableName = tableName;
        }
        public async Task<Result> CreateListing(int ownerId, string title)
        {
            Result result = new Result();
            Result insertResult = await _insertDataAccess.Insert(
                _tableName,
                new Dictionary<string, object>()
                {
                    { "OwnerId", ownerId },
                    { "Title", title },
                    { "Published", 0 },
                    { "CreationDate", DateTime.Now },
                    { "LastEdited", DateTime.Now }
                }
            ).ConfigureAwait(false);

            if (!insertResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                if (insertResult.ErrorMessage!.ToLower().Contains("unique index 'uk01'"))
                {
                    result.ErrorMessage = "Cannot create multiple listings with the same title.";
                    return result;
                }
                result.ErrorMessage = "Unable to create listing.";
                return result;
            }

            return insertResult;
        }

        public async Task<Result<Listing>> GetListing(int listingId)
        {
            Result<Listing> result = new Result<Listing>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "ListingId", "OwnerId", "Title", "Description", "Location", "Price", "LastEdited", "CreationDate", "Published" },
                new List<Comparator>()
                {
                    new Comparator("ListingId", "=", listingId),
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null || selectResult.Payload.Count == 0)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Cannot retrieve specified listing.";
                return result;
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            result.IsSuccessful = true;
            result.Payload = new Listing()
            {
                OwnerId = (int)payload.First()["OwnerId"],
                ListingId = (int)payload.First()["ListingId"],
                Title = (string)payload.First()["Title"],
                Description = payload.First()["Description"] == DBNull.Value ? null : (string)payload.First()["Description"],
                Location = payload.First()["Location"] == DBNull.Value ? null : (string)payload.First()["Location"],
                Price = payload.First()["Price"] == DBNull.Value ? null : (double?)payload.First()["Price"],
                LastEdited = (DateTime)payload.First()["LastEdited"],
                Published = (bool)payload.First()["Published"]
            };

            return result;
        }

        public async Task<Result<int>> GetListingOwnerId(int listingId)
        {
            Result<int> result = new Result<int>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "OwnerId" },
                new List<Comparator>()
                {
                    new Comparator("ListingId", "=", listingId),
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Cannot retrieve specified listing.";
                return result;
            }

            result.IsSuccessful = true;
            result.Payload = (int)selectResult.Payload[0]["OwnerId"];
            return result;
        }

        public async Task<Result> UpdateListing(ListingEditorDTO listing)
        {
            Result result = new Result();

            var values = new Dictionary<string, object>();
            foreach (var column in listing.GetType().GetProperties())
            {
                var value = column.GetValue(listing);
                if (value is null || column.Name == "ListingId" || column.Name == "OwnerId") continue;
                values[column.Name] = value;
            }

            values["LastEdited"] = DateTime.Now;

            Result updateResult = await _updateDataAccess.Update(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator("ListingId", "=", listing.ListingId!),
                },
                values
            ).ConfigureAwait(false);

            if (!updateResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to update listing.";
            }

            result.IsSuccessful = true;
            return updateResult;
        }

        public async Task<Result<List<Listing>>> GetUserListings(int ownerId)
        {
            Result<List<Listing>> result = new Result<List<Listing>>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "ListingId", "OwnerId", "Title", "Description", "Location", "Price", "LastEdited", "CreationDate", "Published" },
                new List<Comparator>()
                {
                    new Comparator("OwnerId", "=", ownerId),
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Cannot retrieve listings.";
                return result;
            }

            result.IsSuccessful = true;
            List<Listing> listings = new();
            List<Dictionary<string, object>> payload = selectResult.Payload;
            foreach (var listItem in payload)
            {
                var listing = new Listing();
                var listingType = listing.GetType();
                foreach (var item in listItem)
                {
                    var prop = listingType.GetProperty(item.Key);
                    if (prop == null)
                        continue;

                    if (item.Value == DBNull.Value)
                    {
                        prop.SetValue(listing, null);
                    }
                    else if (prop.PropertyType == typeof(int?) && item.Value is string stringValue)
                    {
                        int intValue;
                        if (int.TryParse(stringValue, out intValue))
                        {
                            prop.SetValue(listing, intValue);
                        }
                    }
                    else
                    {
                        prop.SetValue(listing, item.Value);
                    }
                }
                listings.Add(listing);
            }


            result.Payload = listings;


            return result;
        }

        public async Task<Result> DeleteListing(int listingId)
        {
            Result deleteResult = await _deleteDataAccess.Delete(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator("ListingId", "=", listingId),
                }
            ).ConfigureAwait(false);

            return deleteResult;
        }

        public async Task<Result> PublishListing(int listingId)
        {
            var values = new Dictionary<string, object>();
            values["Published"] = true;

            Result updateResult = await _updateDataAccess.Update(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator("ListingId", "=", listingId),
                },
                values
            ).ConfigureAwait(false);

            return updateResult;
        }

        public async Task<Result<int>> GetListingId(int ownerId, string title)
        {
            Result<int> result = new Result<int>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "ListingId" },
                new List<Comparator>()
                {
                    new Comparator("OwnerId", "=", ownerId),
                    new Comparator("Title", "=", title),
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Cannot retrieve specified listing ID.";
                return result;
            }

            result.IsSuccessful = true;
            result.Payload = (int)selectResult.Payload[0]["ListingId"];
            return result;
        }
    }
}