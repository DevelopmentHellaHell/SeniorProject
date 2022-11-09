using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using DevelopmentHell.Hubba.Models;
using Microsoft.Identity.Client;


namespace DevelopmentHell.Hubba.SqlDataAccess
{
    
    public class RegistrationDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private SelectDataAccess _selectDataAccess;
        private static string _tableName = "Accounts";

        public RegistrationDataAccess(string connectionString)
        {
        _insertDataAccess = new InsertDataAccess(connectionString);
        _selectDataAccess = new SelectDataAccess(connectionString);
        }

        public async Task<Result> InsertAccount(Dictionary<string, object> values)
        {
           var insertResult = await _insertDataAccess.Insert(_tableName, values).ConfigureAwait(false);
           return insertResult;
        }
        
        public async Task<Result> SelectAccount(List<string> query, Dictionary<string, object> values)
        {
            var selectResult = await _selectDataAccess.Select(_tableName, query, values).ConfigureAwait(false);
            return selectResult;
        }

    }
}
