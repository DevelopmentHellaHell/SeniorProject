using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class AccountSystemDataAccess : IAccountSystemDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private UpdateDataAccess _updateDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private string _tableName;

        public AccountSystemDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _updateDataAccess = new UpdateDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _tableName = tableName;
        }

        //Saves new password entered by user
        public async Task<Result> SavePassword(string newHashPassword, string email)
        {
            Result result = new Result();

            Result updateResult = await _updateDataAccess.Update(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator("Email", "=", email)
                },
                new Dictionary<string, object>()
                {
                    {"PasswordHash", newHashPassword}
                }
            ).ConfigureAwait(false);
            if (!updateResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = updateResult.ErrorMessage;
                return updateResult;
            }

            result.IsSuccessful = true;
            return result;
        }

        //Saves new email and salt after verification
        public async Task<Result> SaveEmailAlterations(string email, string newEmail)
        {
            Result result = new Result();

            Result updateResult = await _updateDataAccess.Update(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator("Email", "=", email)
                },
                new Dictionary<string, object>()
                {
                    {"Email", newEmail}
                }
            ).ConfigureAwait(false);
            if (!updateResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = updateResult.ErrorMessage;
                return updateResult;
            }

            result.IsSuccessful = true;
            return result;
        }

        //Helper function to get credentials for email alterations
        public async Task<Result<List<Dictionary<string, object>>>> GetPasswordData(int userId)
        {
            Result<List<Dictionary<string, object>>> result = new Result<List<Dictionary<string, object>>>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "PasswordHash", "PasswordSalt"},
                new List<Comparator>()
                {
                    new Comparator("Id", "=", userId),
                }
            ).ConfigureAwait(false);
            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = selectResult.ErrorMessage;
                return result;
            }

            //check payload
            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "An Error has ocurred retrieving data. ";
                return result;
            }
            if (payload.Count > 1)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Invalid number of Account Data selected.";
                return result;
            }

            result.IsSuccessful = true;
            return result;

        }

        public async Task<Result> UpdateUserName(int userId, string firstName, string lastName)
        {
            Result updateResult = await _updateDataAccess.Update(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator("Id", "=", userId),
                },
                new Dictionary<string, object>
                {
                    {"FirstName", firstName},
                    {"LastName", lastName}
                }
            ).ConfigureAwait(false);

            return updateResult;
        }

        public async Task<Result<List<Dictionary<string, object>>>> GetAccountSettings(int userId)
        {
            Result<List<Dictionary<string, object>>> result = new Result<List<Dictionary<string, object>>>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() {"FirstName", "LastName"},
                new List<Comparator>() 
                { 
                    new Comparator("Id", "=", userId)
                }
            ).ConfigureAwait(false);

            List<Dictionary<string, object>> payload = selectResult.Payload!;
            if (payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "An Error has ocurred retrieving data. ";
                return result;
            }
            if (payload.Count > 1)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Invalid number of Account Settings selected.";
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }
    }
}

