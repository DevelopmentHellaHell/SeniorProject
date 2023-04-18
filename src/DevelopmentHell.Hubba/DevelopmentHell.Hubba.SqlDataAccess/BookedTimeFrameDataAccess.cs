using Azure;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class BookedTimeFrameDataAccess : IBookedTimeFrameDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private string _tableName;
        public BookedTimeFrameDataAccess (string connectionString, string tablename)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _tableName = tablename;
        }
        /// <summary>
        /// Insert rows into BookedTimeFrames table
        /// </summary>
        /// <param name="timeframes"></param>
        /// <returns>Result</returns>
        public async Task<Result> CreateBookedTimeFrames(int bookingId, List<BookedTimeFrame> timeframes)
        {
            Result result = new();
            if (timeframes.Count == 0)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "List of TimeFrames is empty";
                return result;
            }
            List<string> columns = new List<string>()
            {
                "BookingId",
                "ListingId",
                "AvailabilityId",
                "StartDateTime",
                "EndDateTime"
            };
            //pattern matching
            List<List<object>> values =  timeframes.Select(timeframe => new List<object>()
            {
                bookingId,
                timeframe.ListingId,
                timeframe.AvailabilityId,
                timeframe.StartDateTime,
                timeframe.EndDateTime
            }).ToList();

            Result insertResult = await _insertDataAccess.BatchInsert(_tableName, columns, values).ConfigureAwait(false);

            if (!insertResult.IsSuccessful) 
            {
                result.IsSuccessful = false;
                result.ErrorMessage = insertResult.ErrorMessage;
                return result;
            }
            return insertResult;
        }
       
        /// <summary>
        /// Get BookedTimeFrame by BookingId or ListingId
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="value"></param>
        /// <returns>List of BookedTimeFrame model in result.Payload</returns>
        public async Task<Result> GetBookedTimeFrames(string filter, int value)
        {
            Result<List<BookedTimeFrame>> result = new();
            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() 
                { 
                    "BookingId",
                    "ListingId",
                    "AvailabilityId",
                    "StartDateTime",
                    "EndDateTime"
                },
                new List<Comparator>()
                {
                    new Comparator(filter, "=", value)
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = selectResult.ErrorMessage;
                return result;
            }
            result.IsSuccessful = true;
            result.Payload = new();
            foreach (var row in selectResult.Payload)
            {
                int bookingId = (int)row["BookingId"];
                int listingId = (int)row["ListingId"];
                int availabilityId = (int)row["AvailabilityId"];
                DateTime startDateTime = (DateTime) row["StartDateTime"];
                DateTime endDateTime = (DateTime)row["EndDateTime"];

                result.Payload.Add( new BookedTimeFrame() 
                {
                    BookingId = bookingId,
                    ListingId = listingId,
                    AvailabilityId = availabilityId,
                    StartDateTime = startDateTime,
                    EndDateTime = endDateTime
                });
            }
            return result;
        }
        /// <summary>
        /// Delete rows in BookedTimeFrames table
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="listingId"></param>
        /// <returns>Result</returns>
        public async Task<Result> DeleteBookedTimeFrames(int bookingId, int listingId)
        {
            Result result = new Result();
            result = await _deleteDataAccess.Delete(
                _tableName
                ,new List<Comparator>() 
                    { 
                        new Comparator("BookingId","=",bookingId)
                        ,new Comparator("ListingId", "=", listingId)
                    }
                ).ConfigureAwait(false);
            return result;
        }
    }
}