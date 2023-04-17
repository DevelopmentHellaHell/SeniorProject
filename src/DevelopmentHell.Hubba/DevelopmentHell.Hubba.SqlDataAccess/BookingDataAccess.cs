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
        private DeleteDataAccess _deleteDataAccess;
        private string _tableName;

        public BookingDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _tableName = tableName;
        }
        public async Task<Result> CreateBooking(Booking booking)
        {
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

        public async Task<Result> DeleteBooking(int bookingId)
        {
            Result deleteResult = new();

            deleteResult = await _deleteDataAccess.Delete
                (
                    _tableName,
                    new List<Comparator>()
                    { 
                        new Comparator("BookingId","=", bookingId)
                    }
                ).ConfigureAwait(false);
            return deleteResult;
        }
        
        public async Task<Result> GetBooking(Dictionary<string,int> filters)
        {
            Result<List<Booking>> result = new();
            result.Payload = new List<Booking>();

            List<Comparator> comparators = new();
            foreach (var filter in filters)
            {
                comparators.Add(new Comparator(filter.Key,"=", filter.Value));
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
                        BookingStatusId = (int)row["BookingStatusId"],
                        BookingStatus = new BookingStatus() 
                                        { 
                                            BookingStatusId = (int)row["BookingStatusId"],
                                            Status = (string)row["BookingStatus"]
                                        },
                        FullPrice = Convert.ToSingle(row["FullPrice"]),

                    });
                }
            }
            result.IsSuccessful = true;
            return result;
        }

        public Task<Result> GetBookingId(int userId, int listingId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> GetBookingStatus(int bookingId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> GetFullPrice(int bookingId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateBooking(Booking booking)
        {
            throw new NotImplementedException();
        }
    }
}
