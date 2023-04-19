﻿using Azure;
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
        public async Task<Result<bool>> CreateBookedTimeFrames(int bookingId, List<BookedTimeFrame> timeframes)
        {
            Result<bool> result = new() { IsSuccessful = false};
            if (timeframes.Count == 0)
            {
                result.ErrorMessage = "List of TimeFrames is empty";
                return result;
            }
            List<string> columns = new List<string>()
            {
                nameof(BookedTimeFrame.BookingId),
                nameof(BookedTimeFrame.ListingId),
                nameof(BookedTimeFrame.AvailabilityId),
                nameof(BookedTimeFrame.StartDateTime),
                nameof(BookedTimeFrame.EndDateTime)
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

            var insertResult = await _insertDataAccess.BatchInsert(_tableName, columns, values).ConfigureAwait(false);

            if (!insertResult.IsSuccessful) 
            {
                result.ErrorMessage = insertResult.ErrorMessage;
                return result;
            }
            result.IsSuccessful = true;
            result.Payload = true;
            return result;
        }
       
        /// <summary>
        /// Get BookedTimeFrame by BookingId or ListingId
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="value"></param>
        /// <returns>List of BookedTimeFrame model in result.Payload</returns>
        public async Task<Result<List<BookedTimeFrame>>> GetBookedTimeFrames(List<Tuple<string, object>> filters)
        {
            Result<List<BookedTimeFrame>> result = new();
            var comparators = new List<Comparator>();
            foreach (var filter in filters)
            {
                comparators.Add(new Comparator(filter.Item1, "=", filter.Item2));
            }

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() 
                { 
                    nameof(BookedTimeFrame.BookingId),
                    nameof(BookedTimeFrame.ListingId),
                    nameof(BookedTimeFrame.AvailabilityId),
                    nameof(BookedTimeFrame.StartDateTime),
                    nameof(BookedTimeFrame.EndDateTime)
                },
                comparators
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
                int bookingId = (int)row[nameof(BookedTimeFrame.BookingId)];
                int listingId = (int)row[nameof(BookedTimeFrame.ListingId)];
                int availabilityId = (int)row[nameof(BookedTimeFrame.AvailabilityId)];
                DateTime startDateTime = (DateTime) row[nameof(BookedTimeFrame.StartDateTime)];
                DateTime endDateTime = (DateTime)row[nameof(BookedTimeFrame.EndDateTime)];

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
        public async Task<Result<bool>> DeleteBookedTimeFrames(List<Tuple<string, object>> filters)
        {
            Result<bool> result = new() { IsSuccessful = false };
            List<Comparator> comparators = new();
            foreach (var filter in filters)
            {
                comparators.Add(new Comparator( filter.Item1, "=", filter.Item2 ));
            }
            
            var deleteResult = await _deleteDataAccess.Delete(_tableName, comparators).ConfigureAwait(false);
            if(!deleteResult.IsSuccessful)
            {
                result.ErrorMessage = deleteResult.ErrorMessage;
                return result;
            }
            result.IsSuccessful = true;
            return result;
        }
    }
}