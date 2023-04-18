using Azure;
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
    public class BookingDataAccess : IBookingDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private SelectDataAccess _selectDataAccess;
        private UpdateDataAccess _updateDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private string _tableName;

        public BookingDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _updateDataAccess = new UpdateDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _tableName = tableName;
        }
        /// <summary>
        /// Insert rows
        /// </summary>
        /// <param name="booking"></param>
        /// <returns>
        /// BookingId:int from the inserted row
        /// </returns>
        public async Task<Result> CreateBooking(Booking booking)
        {
            //TODO: double check if ListingId existed in Listings table
            
            Result insertResult = await _insertDataAccess.InsertOutput(
                _tableName,
                new Dictionary<string, object>()
                {
                    { "UserId", booking.UserId },
                    { "ListingId", booking.ListingId },
                    { "FullPrice", booking.FullPrice },
                    { "BookingStatusId", booking.BookingStatusId },
                    { "CreateDate", DateTime.Now },
                    { "LastModifyUser", booking.LastModifyUser }
                },
                "BookingId"
            ).ConfigureAwait(false);

            if (!insertResult.IsSuccessful)
            {
                return insertResult;
            }
            return (Result<int>)insertResult;
        }
        /// <summary>
        /// Delete rows
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns>
        /// Bool
        /// </returns>
        public async Task<Result> DeleteBooking(List<Tuple<string, object>> filters)
        {
            Result deleteResult = new();
            List<Comparator> deleteFilters = new();

            foreach (var filter in filters)
            {
                deleteFilters.Add(new Comparator(filter.Item1, "=", filter.Item2));
            }

            deleteResult = await _deleteDataAccess.Delete( _tableName,deleteFilters).ConfigureAwait(false);
            return deleteResult;
        }
        /// <summary>
        /// Execute query to look up Bookings
        /// Map Bookings Entity with Booking Model
        /// </summary>
        /// <param name="filters"></param>
        /// <returns>
        /// List<Booking>
        /// </returns>
        public async Task<Result> GetBooking(List<Tuple<string,object>> filters)
        {
            Result<List<Booking>> result = new();
            result.Payload = new List<Booking>();
            
            if (filters.Count == 0)
            {
                return new Result()
                {
                    IsSuccessful = false,
                    ErrorMessage = "Filter is empty"
                };
            }
            List<Comparator> comparators = new();
            foreach (var filter in filters)
            {
                comparators.Add(new Comparator(filter.Item1,"=", filter.Item2));
            }
            
            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                SQLManip.InnerJoinTables(new Joiner(_tableName, "BookingStatuses", "BookingStatusId", "BookingStatusId")),
                new List<string>()
                {
                    "BookingId",
                    "UserId",
                    "ListingId",
                    _tableName + ".BookingStatusId",
                    "BookingStatus",
                    "FullPrice"
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
                result.ErrorMessage = "No booking found";
                return result;
            }
            else
            {
                foreach (var row in payload)
                {
                    result.Payload.Add(new Booking()
                    {
                        BookingId = (int)row["BookingId"],
                        UserId = (int)row["UserId"],
                        ListingId = (int)row["ListingId"],
                        BookingStatusId = (BookingStatus)row["BookingStatusId"],
                        FullPrice = Convert.ToSingle(row["FullPrice"]),
                    }); ;
                }
            }
            result.IsSuccessful = true;
            return result;
        }
        /// <summary>
        /// Update row
        /// </summary>
        /// <param name="booking"></param>
        /// <returns>
        /// Bool
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Result> UpdateBooking(Booking booking)
        {
            Dictionary<string, object> values = new()
            {
                {"BookingStatusId", (int)booking.BookingStatusId },
                {"LastModifyUser", booking.UserId },
            };
            Result updateResult = await _updateDataAccess.Update
                (
                _tableName,
                new List<Comparator>() 
                { 
                    new Comparator("BookingId","=", booking.BookingId)
                },
                values
                ).ConfigureAwait(false);
            if(!updateResult.IsSuccessful)
            {
                return new Result()
                {
                    IsSuccessful = false,
                    ErrorMessage = updateResult.ErrorMessage
                };
            }
            return updateResult;
        }
    }
}
