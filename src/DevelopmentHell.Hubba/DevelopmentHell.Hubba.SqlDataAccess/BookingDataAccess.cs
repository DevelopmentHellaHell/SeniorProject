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
        private UpdateDataAccess _updateDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private string _tableName;

        public BookingDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _updateDataAccess = new UpdateDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _tableName = tableName;
        }
        public async Task<Result> CreateBooking(Booking booking)
        {
            Result insertResult = await _insertDataAccess.Insert(
                _tableName,
                new Dictionary<string, object>()
                {
                    { "UserId", booking.UserId },
                    { "ListingId", booking.ListingId },
                    { "FullPrice", booking.FullPrice },
                    { "BookingStatusId", booking.BookingStatusId },
                    { "CreateDate", DateTime.Now },
                    { "LastModifyUser", booking.LastModifyUser }
                }
            ).ConfigureAwait(false);

            return insertResult;
        }

        public Task<Result> DeleteBooking(int bookingId)
        {
            throw new NotImplementedException();
        }
        
        public async Task<Result> GetBooking(int userId, int listingId)
        {
            Result<List<Booking>> result = new Result<List<Booking>>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "BookingId" },
                new List<Comparator>()
                {
                    new Comparator("UserId", "=", userId),
                    new Comparator("ListingId", "=", listingId)
                }
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
