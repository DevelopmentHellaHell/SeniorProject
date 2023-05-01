using Azure;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class BookingsDataAccess : IBookingsDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private SelectDataAccess _selectDataAccess;
        private UpdateDataAccess _updateDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private string _tableName;

        public BookingsDataAccess(string connectionString, string tableName)
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

            var insertResult = await _insertDataAccess.InsertOutput(
                _tableName,
                new Dictionary<string, object>()
                {
                    { nameof(Booking.UserId), booking.UserId },
                    { nameof(Booking.ListingId), booking.ListingId },
                    { nameof(Booking.FullPrice), booking.FullPrice },
                    { nameof(Booking.BookingStatusId), (int)booking.BookingStatusId },
                    { nameof(Booking.CreationDate), DateTime.Now },
                    { nameof(Booking.LastEditUser), booking.LastEditUser }
                },
                nameof(Booking.BookingId)
            ).ConfigureAwait(false);

            if (!insertResult.IsSuccessful)
            {
                return new(Result.Failure(insertResult.ErrorMessage));
            }
            
            return Result<int>.Success(insertResult.Payload);
        }
        /// <summary>
        /// Delete rows
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns>Bool in Payload</returns>
        public async Task<Result<bool>> DeleteBooking(List<Tuple<string, object>> filters)
        {
            List<Comparator> deleteFilters = new();

            foreach (var filter in filters)
            {
                deleteFilters.Add(new Comparator(filter.Item1, "=", filter.Item2));
            }

            var deleteResult = await _deleteDataAccess.Delete( _tableName,deleteFilters).ConfigureAwait(false);
            if(!deleteResult.IsSuccessful)
            {
                return new(Result.Failure(deleteResult.ErrorMessage));
            }
            return new(Result.Success());
        }
        /// <summary>
        /// Execute query to look up Bookings
        /// Map Bookings Entity with Booking Model
        /// </summary>
        /// <param name="filters"></param>
        /// <returns>List<Booking> in Payload</returns>
        public async Task<Result<List<Booking>>> GetBooking(List<Tuple<string,object>> filters)
        {
            if (filters.Count == 0)
            {
                return new(Result.Failure("Invalid filter", StatusCodes.Status400BadRequest));
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
                nameof(Booking.LastEditUser)
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
                return new(Result.Failure(selectResult.ErrorMessage));
            }
            Result<List<Booking>> result = new(Result.Success());
            result.Payload = new List<Booking>();

            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload.Count < 1)
            {
                return new(Result.Failure("No booking found", StatusCodes.Status404NotFound));
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
                        LastEditUser = (int)row[nameof(Booking.LastEditUser)]
                    }); ;
                }
            }
            return result;
        }
        /// <summary>
        /// Update row
        /// </summary>
        /// <param name="booking"></param>
        /// <returns>Bool in Payload</returns>
        public async Task<Result<bool>> UpdateBooking(Dictionary<string,object> values, List<Comparator> filters)
        {
            var  updateResult = await _updateDataAccess.Update (_tableName, filters, values).ConfigureAwait(false);
            if(!updateResult.IsSuccessful)
            {
                return new(Result.Failure(updateResult.ErrorMessage, updateResult.StatusCode));
            }
            return new (Result.Success());
        }

        public async Task<Result<List<BookingHistory>>> GetBookingHistory(int userId)
        {
            Result<List<BookingHistory>> result = new Result<List<BookingHistory>>();
            var selectResult = await _selectDataAccess.Select(
                SQLManip.InnerJoinTables(
                    new Joiner(
                        _tableName,
                        "Listings",
                        nameof(BookingHistory.ListingId),
                        nameof(BookingHistory.ListingId))),
                new List<string>()
                {
                    nameof(BookingHistory.BookingId),
                    nameof(BookingHistory.ListingId),
                    nameof(BookingHistory.FullPrice),
                    nameof(BookingHistory.BookingStatusId),
                    nameof(BookingHistory.Title),
                    nameof(BookingHistory.Location)
                },
                new List<Comparator>()
                { 
                    new Comparator("UserId", "=", userId)
                }
            ).ConfigureAwait(false);
            if (!selectResult.IsSuccessful || selectResult.Payload is null) 
            {
                result.IsSuccessful = false;
                return result;
            }

            if (selectResult.Payload.Count < 1)
            {
                return new(Result.Failure("No booking found", StatusCodes.Status404NotFound));
            }
            else
            {
                foreach(var row in selectResult.Payload)
                {
                    result.Payload!.Add(new BookingHistory()
                    {
                        BookingId = (int)row[nameof(BookingHistory.BookingId)],
                        ListingId = (int)row[nameof(BookingHistory.ListingId)],
                        FullPrice = (int)row[nameof(BookingHistory.FullPrice)],
                        BookingStatusId = (int)row[nameof(BookingHistory.BookingStatusId)],
                        Title = (string)row[nameof(BookingHistory.Title)],
                        Location = (string)row[nameof(BookingHistory.Location)]
                    }); ;
                }
            }
            result.IsSuccessful = true;
            return result;
        }
    }
}
