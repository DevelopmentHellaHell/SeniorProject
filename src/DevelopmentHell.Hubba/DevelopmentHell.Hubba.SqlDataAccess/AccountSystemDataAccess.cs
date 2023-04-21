using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class AccountSystemDataAccess
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
        public async Task<Result> SaveEmailAlterations(string email, string newSalt, string newEmail, string newHashPassword)
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
                    {"PasswordSalt", newSalt},
                    {"PasswordHash", newHashPassword},
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
    }
}

