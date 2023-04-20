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
        /// <returns>BookingId:int </returns>
        public async Task<Result<int>> CreateBooking(Booking booking)
        {
            //TODO: double check if ListingId existed in Listings table
            Result<int> result = new() { IsSuccessful = false };

            // Bookings table has a trigger,
            // once inserted new entry, check Listings table for an existing ListingId
            // if there's no such ListingId, rollback and delete the inserted Booking
            var insertResult = await _insertDataAccess.InsertOutput(
                _tableName,
                new Dictionary<string, object>()
                {
                    { nameof(Booking.UserId), booking.UserId },
                    { nameof(Booking.ListingId), booking.ListingId },
                    { nameof(Booking.FullPrice), booking.FullPrice },
                    { nameof(Booking.BookingStatusId), booking.BookingStatusId },
                    { nameof(Booking.CreateDate), DateTime.Now },
                    { nameof(Booking.LastModifyUser), booking.LastModifyUser }
                },
                nameof(Booking.BookingId)
            ).ConfigureAwait(false);

            if (!insertResult.IsSuccessful)
            {
                result.ErrorMessage = insertResult.ErrorMessage;
                return result;
            }
            result.IsSuccessful = true;
            result.Payload = ((Result<int>)insertResult).Payload;
            return result;
        }
        /// <summary>
        /// Delete rows
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns>Bool in Payload</returns>
        public async Task<Result<bool>> DeleteBooking(List<Tuple<string, object>> filters)
        {
            Result<bool> result = new() { IsSuccessful = false };
            List<Comparator> deleteFilters = new();

            foreach (var filter in filters)
            {
                deleteFilters.Add(new Comparator(filter.Item1, "=", filter.Item2));
            }

            var deleteResult = await _deleteDataAccess.Delete( _tableName,deleteFilters).ConfigureAwait(false);
            if(!deleteResult.IsSuccessful)
            {
                result.ErrorMessage = deleteResult.ErrorMessage;
                return result;
            }
            result.IsSuccessful = true;
            return result;
        }
        /// <summary>
        /// Execute query to look up Bookings
        /// Map Bookings Entity with Booking Model
        /// </summary>
        /// <param name="filters"></param>
        /// <returns>List<Booking> in Payload</returns>
        public async Task<Result<List<Booking>>> GetBooking(List<Tuple<string,object>> filters)
        {
            Result<List<Booking>> result = new() { IsSuccessful = false };
            result.Payload = new List<Booking>();
            
            if (filters.Count == 0)
            {
                result.ErrorMessage = "Filter is empty";
                return result;
            }
            // prepare for string query
            List<string> columns = new()
            {
                nameof(Booking.BookingId),
                nameof(Booking.UserId),
                nameof(Booking.ListingId),
                _tableName + "." + nameof(Booking.BookingStatusId),
                nameof(BookingStatus),
                nameof(Booking.FullPrice),
                nameof(Booking.LastModifyUser)
            };
            List<Comparator> comparators = new();
            foreach (var filter in filters)
            {
                comparators.Add(new Comparator(filter.Item1,"=", filter.Item2));
            }
            
            var selectResult = await _selectDataAccess.Select(
                SQLManip.InnerJoinTables(
                    new Joiner(
                        _tableName, 
                        "BookingStatuses", 
                        nameof(Booking.BookingStatusId), 
                        nameof(Booking.BookingStatusId))),
                columns,
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
                        BookingId = (int)row[nameof(Booking.BookingId)],
                        UserId = (int)row[nameof(Booking.UserId)],
                        ListingId = (int)row[nameof(Booking.ListingId)],
                        BookingStatusId = (BookingStatus)row[nameof(Booking.BookingStatusId)],
                        FullPrice = Convert.ToSingle(row[nameof(Booking.FullPrice)]),
                        LastModifyUser = (int)row[nameof(Booking.LastModifyUser)]
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
        /// <returns>Bool in Payload</returns>
        public async Task<Result<bool>> UpdateBooking(Booking booking)
        {
            Result<bool> result = new() { IsSuccessful = false };

            Dictionary<string, object> values = new()
            {
                {nameof(Booking.BookingStatusId), (int)booking.BookingStatusId },
                {nameof(Booking.LastModifyUser), booking.LastModifyUser },
            };

            var updateResult = await _updateDataAccess.Update
                (
                _tableName,
                new List<Comparator>() 
                { 
                    new Comparator(nameof(Booking.BookingId),"=", booking.BookingId)
                },
                values
                ).ConfigureAwait(false);
            if(!updateResult.IsSuccessful)
            {
                result.ErrorMessage = updateResult.ErrorMessage;
                return result;
            }
            result.IsSuccessful = true;
            result.Payload = true;
            return result;
        }
    }
}
