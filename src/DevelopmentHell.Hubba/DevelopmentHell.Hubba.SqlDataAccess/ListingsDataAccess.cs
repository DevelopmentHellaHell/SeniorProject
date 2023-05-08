
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class ListingsDataAccess : IListingsDataAccess
    {
        
        private readonly InsertDataAccess _insertDataAccess;
        private readonly UpdateDataAccess _updateDataAccess;
        private readonly SelectDataAccess _selectDataAccess;
        private readonly DeleteDataAccess _deleteDataAccess;
        private readonly ExecuteDataAccess _executeDataAccess;
        private readonly string _ownerIdColumn = "OwnerId";
        private readonly string _titleColumn = "Title";
        private readonly string _descriptionColumn = "Description";
        private readonly string _publishedColumn = "Published";
        private readonly string _creationDateColumn = "CreationDate";
        private readonly string _lastEditedColumn = "LastEdited";
        private readonly string _locationColumn = "Location";
        private readonly string _listingIdColumn = "ListingId";
        private readonly string _priceColumn = "Price";
        private readonly string _tableName;

        public ListingsDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _updateDataAccess = new UpdateDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _executeDataAccess = new ExecuteDataAccess(connectionString);
            _tableName = tableName;
        }
        public async Task<Result> CreateListing(int ownerId, string title)
        {
            Result result = new Result();
            Result insertResult = await _insertDataAccess.Insert(
                _tableName,
                new Dictionary<string, object>()
                {
                    { _ownerIdColumn, ownerId },
                    { _titleColumn, title },
                    { _publishedColumn, 0 },
                    { _creationDateColumn, DateTime.Now },
                    { _lastEditedColumn, DateTime.Now }
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
                new List<string>() { _listingIdColumn, _ownerIdColumn, _titleColumn, _descriptionColumn, _locationColumn, _priceColumn, _lastEditedColumn, _creationDateColumn, _publishedColumn },
                new List<Comparator>()
                {
                    new Comparator(_listingIdColumn, "=", listingId),
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
                OwnerId = (int)payload.First()[_ownerIdColumn],
                ListingId = (int)payload.First()[_listingIdColumn],
                Title = (string)payload.First()[_titleColumn],
                Description = payload.First()[_descriptionColumn] == DBNull.Value ? null : (string)payload.First()[_descriptionColumn],
                Location = payload.First()[_locationColumn] == DBNull.Value ? null : (string)payload.First()[_locationColumn],
                Price = payload.First()[_priceColumn] == DBNull.Value ? null : (double?)Convert.ToDouble((payload.First()[_priceColumn])),
                LastEdited = (DateTime)payload.First()[_lastEditedColumn],
                Published = (bool)payload.First()[_publishedColumn]
            };

            return result;
        }

        public async Task<Result<int>> GetListingOwnerId(int listingId)
        {
            Result<int> result = new Result<int>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { _ownerIdColumn },
                new List<Comparator>()
                {
                    new Comparator(_listingIdColumn, "=", listingId),
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Cannot retrieve specified listing.";
                return result;
            }

            result.IsSuccessful = true;
            result.Payload = (int)selectResult.Payload[0][_ownerIdColumn];
            return result;
        }

        public async Task<Result> UpdateListing(ListingEditorDTO listing)
        {
            Result result = new Result();

            var values = new Dictionary<string, object>();
            foreach (var column in listing.GetType().GetProperties())
            {
                var value = column.GetValue(listing);
                if ((column.Name == _descriptionColumn || column.Name == _priceColumn || column.Name == _locationColumn) && value is null)
                {
                    values[column.Name] = DBNull.Value;
                }
                if (value is null || column.Name == _listingIdColumn || column.Name == _ownerIdColumn || column.Name == _titleColumn) continue;
                values[column.Name] = value;
            }

            values[_lastEditedColumn] = DateTime.Now;

            Result updateResult = await _updateDataAccess.Update(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator(_listingIdColumn, "=", listing.ListingId!),
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
                new List<string>() { _listingIdColumn, _ownerIdColumn, _titleColumn, _descriptionColumn, _locationColumn, _priceColumn, _lastEditedColumn, _creationDateColumn, _publishedColumn },
                new List<Comparator>()
                {
                    new Comparator(_ownerIdColumn, "=", ownerId),
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
                    else if (prop.Name == _priceColumn)
                    {
                        prop.SetValue(listing, Convert.ToDouble(item.Value));
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
                    new Comparator(_listingIdColumn, "=", listingId),
                }
            ).ConfigureAwait(false);

            return deleteResult;
        }

        public async Task<Result> PublishListing(int listingId)
        {
            var values = new Dictionary<string, object>();
            values[_publishedColumn] = true;

            Result updateResult = await _updateDataAccess.Update(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator(_listingIdColumn, "=", listingId),
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
                new List<string>() { _listingIdColumn },
                new List<Comparator>()
                {
                    new Comparator(_ownerIdColumn, "=", ownerId),
                    new Comparator(_titleColumn, "=", title),
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Cannot retrieve specified listing ID.";
                return result;
            }

            result.IsSuccessful = true;
            result.Payload = (int)selectResult.Payload[0][_listingIdColumn];
            return result;
        }

        public async Task<Result<List<Dictionary<string, object>>>> Curate(int offset = 0) {
			var result = await _executeDataAccess.Execute("CurateListings", new Dictionary<string, object>() {
				{ "Offset", offset },
			}).ConfigureAwait(false);

			return result;
		}

		public async Task<Result<List<Dictionary<string, object>>>> Search(string query, int offset = 0, double FTTWeight = 0.5, double RWeight = 0.25, double RCWeight = 0.25)
		{
			var result = await _executeDataAccess.Execute("SearchListings", new Dictionary<string, object>()
			{
				{ "Query", query },
				{ "Offset", offset },
				{ "FTTableRankWeight", FTTWeight },
				{ "RatingsRankWeight", RWeight },
				{ "RatingsCountRankWeight", RCWeight },
			}).ConfigureAwait(false);

			return result;
		}

        public async Task<Result> UnpublishListing(int listingId)
        {
            var values = new Dictionary<string, object>();
            values[_publishedColumn] = false;

            Result updateResult = await _updateDataAccess.Update(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator(_listingIdColumn, "=", listingId),
                },
                values
            ).ConfigureAwait(false);

            return updateResult;
        }
        public async Task<Result<List<BookingHistory>>> GetBookingHistorySearch(int userId, string query)
        {
            List<BookingHistory> result = new List<BookingHistory>();
            var queryResult = await _executeDataAccess.Execute("SearchBookingHistory", new Dictionary<string, object>()
            {
                {"Query", query },
                {"UserId", userId}
            }).ConfigureAwait(false);

            if (!queryResult.IsSuccessful)
            {
                return new(Result.Failure("Reservation Access Error"));
            }
            if (queryResult.Payload is null)
            {
                return new(Result.Failure("There were no reservations with your search. "));
            }
            foreach (var row in queryResult.Payload)
            {
                result.Add(new BookingHistory()
                {
                    BookingId = (int)row[nameof(BookingHistory.BookingId)],
                    ListingId = (int)row[nameof(BookingHistory.ListingId)],
                    FullPrice = decimal.ToDouble((decimal)row[nameof(BookingHistory.FullPrice)]),
                    BookingStatusId = (int)row[nameof(BookingHistory.BookingStatusId)],
                    Title = (string)row[nameof(BookingHistory.Title)],
                    Location = (string)row[nameof(BookingHistory.Location)]
                });
            }

            return Result<List<BookingHistory>>.Success(result);
        }
    }
}
