using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class UserLoginDataAccess : IUserLoginDataAccess
    {

        private InsertDataAccess _insertDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private UpdateDataAccess _updateDataAccess;
        private string _tableName;
        public UserLoginDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _updateDataAccess = new UpdateDataAccess(connectionString);
            _tableName = tableName;
        }
        public async Task<Result> AddLogin(int accoundId, string ipAddress)
        {
            Result insertResult = await _insertDataAccess.Insert(
                _tableName, 
                new Dictionary<string, object>() {
                    { "Id" , accoundId },
                    { "IPAddress", ipAddress}
                }
            ).ConfigureAwait(false);

            return insertResult;
        }


        public async Task<Result<string[]>> GetIPAddress(int accountId)
        {
            Result<string[]> result = new Result<string[]>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "IPAddress" },
                new List<Comparator>()
                {
                    new Comparator("Id", "=", accountId),
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = selectResult.ErrorMessage;
                return result;
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;

            result.IsSuccessful = true;
            if (payload.Count > 0) result.Payload = (string[])payload[0].Values.ToArray();
            return result;
        }

        public async Task<Result> Delete(int accountId)
        {
            Result deleteResult = await _deleteDataAccess.Delete(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator("Id", "=", accountId),
                }
            ).ConfigureAwait(false);

            return deleteResult;
        }
    }
}