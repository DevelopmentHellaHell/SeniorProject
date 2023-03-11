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
            throw new NotImplementedException();
        }

        public async Task<Result<string>> InsertUpdate(int id, Dictionary<string, object> data)
        {
            throw new NotImplementedException();
        }
    }
}
