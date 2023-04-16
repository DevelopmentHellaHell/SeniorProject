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
        private UpdateDataAccess _updateDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private string _tableName;
        public BookedTimeFrameDataAccess (string connectionString, string tablename)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _updateDataAccess = new UpdateDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _tableName = tablename;
        }
        public Task<Result> CreateSingleBookedTimeFrame(BookedTimeFrame timeframe)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> CreateBookedTimeFrames(List<BookedTimeFrame> timeframes)
        {
            List<Dictionary<string, object>> valuesList = new List<Dictionary<string, object>>();
            foreach (var timeframe in timeframes)
            {
                valuesList.Add(new Dictionary<string, object>()
                                {
                                    { "BookingId", timeframe.BookingId },
                                    { "ListingId", timeframe.ListingId },
                                    { "AvailabilityId", timeframe.AvailabilityId},
                                    { "StartDateTime", timeframe.StartDateTime },
                                    { "EndDateTime", timeframe.EndDateTime }
                                });
            }

            Result insertResult = await _insertDataAccess.InsertAll(_tableName, valuesList).ConfigureAwait(false);

            return insertResult;
        }

        public Task<Result> GetBookedTimeFramesByBookingId(int bookingId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> GetBookedTimeFramesByListingId(int listingId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateBookedTimeFrame(BookedTimeFrame bookedTimeFrame)
        {
            throw new NotImplementedException();
        }

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