

using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class ListingAvailabilitiesDataAccess : IListingAvailabilitiesDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private UpdateDataAccess _updateDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private string _tableName;

        public ListingAvailabilitiesDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _updateDataAccess = new UpdateDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _tableName = tableName;
        }

        public async Task<Result> AddListingAvailabilities(List<ListingAvailabilityDTO> listingAvailabilities)
        {
            Result result = new Result();
            List<List<Dictionary<string, object>>> insertList = new();
            List<string> keys = new List<string>()
            {
                "ListingId",
                "StartTime",
                "EndTime"
            };
            List<List<object>> values = listingAvailabilities.Select(avail => new List<object>()
                {
                    avail.ListingId,
                    avail.StartTime,
                    avail.EndTime
                }).ToList();
            Console.WriteLine(listingAvailabilities.Count.ToString());
            Console.WriteLine("Values count: " + values.Count);
            Result insertResult = await _insertDataAccess.BatchInsert(
                _tableName, keys, values).ConfigureAwait(false);

            if (!insertResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = insertResult.ErrorMessage;
                return result;
            }

            return insertResult;
        }

        public async Task<Result> DeleteListingAvailabilities(List<ListingAvailabilityDTO> listingAvailabilities)
        {
            int listingId = (int)listingAvailabilities[0].ListingId!;
            int aID = (int)listingAvailabilities[0].AvailabilityId!;

            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            bool first = true;
            foreach (ListingAvailabilityDTO listingAvailability in listingAvailabilities)
            {
                if (!first)
                {
                    sb.Append(", ");
                }
                sb.Append(listingAvailability.AvailabilityId!.ToString());
                //aID = (int)listingAvailability.AvailabilityId!;
                first = false;
                
            }
            sb.Append(")");
            Console.WriteLine(listingId.ToString());
            Console.WriteLine(aID.ToString());
            //Console.WriteLine(sb.ToString());

            Result deleteResult = await _deleteDataAccess.Delete(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator("ListingId", "=", listingId),
                    new Comparator("AvailabilityId", "=", aID),
                    //new Comparator("AvailabilityId", "in", sb.ToString()),
                }
            ).ConfigureAwait(false);
            Console.WriteLine(deleteResult.ErrorMessage);

            return deleteResult;
        }

        public async Task<Result<List<ListingAvailability>>> GetListingAvailabilities(int listingId)
        {
            Result<List<ListingAvailability>> result = new Result<List<ListingAvailability>>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "ListingId", "AvailabilityId", "StartTime", "EndTime" },
                new List<Comparator>()
                {
                    new Comparator("ListingId", "=", listingId),
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to retrieve specified listing availabilities.";
                return result;
            }


            result.IsSuccessful = true;
            List<ListingAvailability> listingAvailabilities = new();
            List<Dictionary<string, object>> payload = selectResult.Payload;
            foreach (var listItem in payload)
            {
                var listingAvailability = new ListingAvailability();
                var listingAvailabilityType = listingAvailability.GetType();
                foreach (var item in listItem)
                {
                    listingAvailabilityType.GetProperty(item.Key)!.SetValue(listingAvailability, item.Value, null);
                }
                listingAvailabilities.Add(listingAvailability);
            }
            result.Payload = listingAvailabilities;


            return result;
        }

        public async Task<Result> UpdateListingAvailability(ListingAvailabilityDTO listingAvailability)
        {
            Result result = new Result();
            var values = new Dictionary<string, object>();
            foreach (var column in listingAvailability.GetType().GetProperties())
            {
                var value = column.GetValue(listingAvailability);

                if (value is null || column.Name == "ListingId" || column.Name == "AvailabilityId") continue;
                values[column.Name] = value;
            }
            Result updateResult = await _updateDataAccess.Update(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator("ListingId", "=", listingAvailability.ListingId!),
                    new Comparator("AvailabilityId", "=", listingAvailability.AvailabilityId!),
                },
                values
            ).ConfigureAwait(false);
            if (!updateResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Unable to update availability.";
                return result;
            }
            result.IsSuccessful = true;
            return result;
        }
    }
}