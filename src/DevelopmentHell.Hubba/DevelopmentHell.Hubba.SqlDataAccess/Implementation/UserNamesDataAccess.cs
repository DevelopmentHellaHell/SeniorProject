using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess.Implementation
{
    public class UserNamesDataAccess : IUserNamesDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private SelectDataAccess _selectDataAccess;
        private UpdateDataAccess _updateDataAccess;
        private string _tableName;

        public UserNamesDataAccess(string connectionString,  string tableName)
        {
            _tableName = tableName;
            _insertDataAccess = new(connectionString);
            _selectDataAccess = new(connectionString);
            _updateDataAccess = new(connectionString);
        }
        public async Task<Result<Dictionary<string, object>>> GetData(int id)
        {
            var selResult = await _selectDataAccess.Select(_tableName, new() { "*" }, new() { new("UserAccountId", "=", id) });
            if (!selResult.IsSuccessful || selResult.Payload!.Count != 1)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = "Unable to get proper name information on users"
                };
            }
            return new()
            {
                IsSuccessful = true,
                Payload = selResult.Payload![0]
            };
        }

        public async Task<Result> Insert(int id, string? firstName, string? lastName, string? userName)
        {
            Dictionary<string, object> insertData = new() {
                { "UserAccountId",id },
                { "FirstName", firstName == null ? DBNull.Value : firstName},
                { "LastName",  lastName  == null ? DBNull.Value : lastName},
                { "UserName",  userName  == null ? DBNull.Value : userName},
            };
            return await _insertDataAccess.Insert(_tableName, insertData).ConfigureAwait(false);
        }

        public Task<Result> Update(int id, Dictionary<string, object> data)
        {
            throw new NotImplementedException();
        }
    }
}
