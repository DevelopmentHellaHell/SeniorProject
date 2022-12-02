using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess
{

    public class RegistrationDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private SelectDataAccess _selectDataAccess;
        private readonly string _tableName = "Accounts";

        public RegistrationDataAccess(string connectionString)
        {
        _insertDataAccess = new InsertDataAccess(connectionString);
        _selectDataAccess = new SelectDataAccess(connectionString);
        }

        public async Task<Result> InsertAccount(Dictionary<string, object> values)
        {
           var insertResult = await _insertDataAccess.Insert(_tableName, values).ConfigureAwait(false);
           return insertResult!;
        }
        
        public async Task<Result> SelectAccount(List<string> query, Dictionary<string, object> values)
        {
            var selectResult = await _selectDataAccess.Select(_tableName, query, values).ConfigureAwait(false);
            return selectResult!;
        }

    }
}
