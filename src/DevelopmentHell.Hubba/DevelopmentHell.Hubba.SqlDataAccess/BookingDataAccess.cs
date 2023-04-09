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
                    //{ "CreateDate", DateTime.Now },
                    //{ "LastModifyUser", booking.LastModifyUser }
                }
            ).ConfigureAwait(false);

            return insertResult;
        }

        public Task<Result> DeleteBooking(int bookingId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> GetBookingId(int userId, int listingId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> GetBookingId(int userId, int listingId, int availabilityId)
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
