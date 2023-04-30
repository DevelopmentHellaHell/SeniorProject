using Azure.Core;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class UserAccountDataAccess : IUserAccountDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private UpdateDataAccess _updateDataAccess;
        private string _tableName;
        private readonly string _hash = "PasswordHash";
        private readonly string _salt = "PasswordSalt";
        private readonly string _email = "Email";
        private readonly string _id = "Id";
        private readonly string _firstName = "FirstName";
        private readonly string _lastName = "LastName";
        public UserAccountDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _updateDataAccess = new UpdateDataAccess(connectionString);
            _tableName = tableName;
        }

        public async Task<Result> CreateUserAccount(string email, HashData password, string role = "VerifiedUser")
        {
            Result insertResult = await _insertDataAccess.Insert(
                _tableName,
                new Dictionary<string, object>()
                {
                    { "Email", email },
                    { _hash, Convert.ToBase64String(password.Hash!) },
                    { _salt, password.Salt! },
                    { "LoginAttempts", 0 },
                    { "FailureTime", DBNull.Value },
                    { "Disabled", false },
                    { "Role", role }
                }
            ).ConfigureAwait(false);

            return insertResult;
        }

        public async Task<Result<int>> GetId(string email)
        {
            Result<int> result = new Result<int>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "Id" },
                new List<Comparator>()
                {
                    new Comparator("Email", "=", email),
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = selectResult.ErrorMessage;
                return result;
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload.Count > 1)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Invalid number of UserAccounts selected.";
                return result;
            }

            result.IsSuccessful = true;
            if (payload.Count > 0) result.Payload = (int)payload[0]["Id"];
            return result;
        }

        public async Task<Result<int>> GetId(string email, HashData password)
        {
            Result<int> result = new Result<int>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "Id" },
                new List<Comparator>()
                {
                    new Comparator("Email", "=", email),
                    new Comparator(_hash, "=", password.Hash!),
                    new Comparator(_salt, "=", password.Salt!)
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = selectResult.ErrorMessage;
                return result;
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload.Count > 1)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Invalid number of UserAccounts selected.";
                return result;
            }

            result.IsSuccessful = true;
            if (payload.Count > 0) result.Payload = (int)payload[0]["Id"];
            return result;
        }

        public async Task<Result<UserAccount>> GetUser(int id)
        {
            Result<UserAccount> result = new Result<UserAccount>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "*" },
                new List<Comparator>()
                {
                    new Comparator("Id","=",id),
                }
            ).ConfigureAwait(false);
            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = selectResult.ErrorMessage;
                return result;
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload.Count > 1)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Invalid number of UserAccounts selected.";
                return result;
            }

            result.IsSuccessful = true;
            if (payload.Count > 0) result.Payload = new UserAccount()
            {
                Id = id,
                Email = (string)payload.First()["Email"],
                LoginAttempts = (int)payload.First()["LoginAttempts"],
                FailureTime = payload.First()["FailureTime"] == payload.First()["FailureTime"] ? null : payload.First()["FailureTime"],
                Disabled = (bool)payload.First()["Disabled"],
                Role = (string)payload.First()["Role"],
                CellPhoneNumber = payload.First()["CellPhoneNumber"] == DBNull.Value ? null : (string)payload.First()["CellPhoneNumber"],
                CellPhoneProvider = payload.First()["CellPhoneProvider"] == DBNull.Value ? null : (CellPhoneProviders)payload.First()["CellPhoneProvider"]
            };
            return result;
        }

        public async Task<Result<UserAccount>> GetUser(string email)
        {
            return await GetUser((await GetId(email).ConfigureAwait(false)).Payload).ConfigureAwait(false);
        }

        public async Task<Result<UserAccount>> GetAttempt(int id)
        {
            Result<UserAccount> result = new Result<UserAccount>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "LoginAttempts", "FailureTime" },
                new List<Comparator>()
                {
                    new Comparator("Id", "=", id),
                }
            ).ConfigureAwait(false);
            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = selectResult.ErrorMessage;
                return result;
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload.Count > 2)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Invalid number of UserAccounts selected.";
                return result;
            }

            result.IsSuccessful = true;
            if (payload.Count > 0) result.Payload = new UserAccount()
            {
                LoginAttempts = (int)payload[0]["LoginAttempts"],
                FailureTime = payload[0]["FailureTime"] == DBNull.Value ? null : payload[0]["FailureTime"],
            };
            return result;
        }

        public async Task<Result<UserAccount>> GetHashData(string email)
        {
            Result<UserAccount> result = new Result<UserAccount>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { _hash, _salt },
                new List<Comparator>()
                {
                    new Comparator("Email", "=", email),
                }
            ).ConfigureAwait(false);
            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = selectResult.ErrorMessage;
                return result;
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload.Count > 1)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Invalid number of UserAccounts selected.";
                return result;
            }

            result.IsSuccessful = true;
            if (payload.Count > 0) result.Payload = new UserAccount()
            {
                PasswordHash = (string)payload.First()[_hash],
                PasswordSalt = (string)payload.First()[_salt],
            };
            return result;
        }

        public async Task<Result<bool>> GetDisabled(int id)
        {
            Result<bool> result = new Result<bool>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "Disabled" },
                new List<Comparator>()
                {
                    new Comparator("Id", "=", id),
                }
            ).ConfigureAwait(false);
            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = selectResult.ErrorMessage;
                return result;
            }

            List<Dictionary<string, object>> payload = selectResult.Payload;
            if (payload.Count > 1)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Invalid number of UserAccounts selected.";
                return result;
            }

            result.IsSuccessful = true;
            if (payload.Count > 0) result.Payload = (bool)payload[0]["Disabled"];
            return result;
        }

        public async Task<Result> Update(UserAccount userAccount)
        {
            var values = new Dictionary<string, object>();
            foreach (var column in userAccount.GetType().GetProperties())
            {
                var value = column.GetValue(userAccount);
                if (value is null || column.Name == "Id") continue;
                values[column.Name] = value;
            }

            Result updateResult = await _updateDataAccess.Update(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator("Id", "=", userAccount.Id),
                },
                values
            ).ConfigureAwait(false);

            return updateResult;
        }

        public async Task<Result> Delete(int id)
        {
            Result deleteResult = await _deleteDataAccess.Delete(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator("Id", "=", id),
                }
            ).ConfigureAwait(false);

            return deleteResult;
        }

        public async Task<Result> Delete(string email)
        {
            Result deleteResult = await _deleteDataAccess.Delete(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator("Email", "=", email),
                }
            ).ConfigureAwait(false);

            return deleteResult;
        }
        public async Task<Result<int>> CountAdmin()
        {
            Result<int> result = new Result<int>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "COUNT(*) as AdminCount" },
                new List<Comparator>()
                {
                    new Comparator("Role","=", "AdminUser"),
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
            result.Payload = (int)payload[0]["AdminCount"];

            return result;
        }

        public async Task<Result> SetEnabledStatus(int id, bool enabled)
        {
            var setResult = await _updateDataAccess.Update(_tableName, new() { new("Id", "=", id) }, new() { { "Disabled", enabled ? 0 : 1 } });
            if (!setResult.IsSuccessful)
            {
                return new()
                {
                    IsSuccessful = false,
                    ErrorMessage = "Unable to set Update Result: " + setResult.ErrorMessage
                };
            }
            return setResult;
        }

        public async Task<Result<string>> GetEmail(int id)
        {
            Result<string> result = new Result<string>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { "Email" },
                new List<Comparator>()
                {
                    new Comparator("Id","=",id),
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload.Count != 1)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = selectResult.ErrorMessage;
                return result;
            }

            result.IsSuccessful = true;
            result.Payload = (string)selectResult.Payload[0]["Email"];
            return result;
        }

        //Saves new password entered by user
        public async Task<Result> SavePassword(string newHashPassword, string email)
        {
            Result result = new Result();

            Result updateResult = await _updateDataAccess.Update(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator(_email, "=", email)
                },
                new Dictionary<string, object>()
                {
                    {_hash, newHashPassword}
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
        public async Task<Result> SaveEmailAlterations(int userId, string newEmail)
        {
            Result result = new Result();

            Result updateResult = await _updateDataAccess.Update(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator(_id, "=", userId)
                },
                new Dictionary<string, object>()
                {
                    {_email, newEmail}
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
        public async Task<Result<PasswordInformation>> GetPasswordData(int userId)
        {
            Result<PasswordInformation> result = new Result<PasswordInformation>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { _hash, _salt },
                new List<Comparator>()
                {
                    new Comparator(_id, "=", userId),
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

            result.Payload = new PasswordInformation()
            {
                PasswordHash = (string)payload.First()[_hash],
                PasswordSalt = (string)payload.First()[_salt]
            };

            return result;

        }

        public async Task<Result> UpdateUserName(int userId, string? firstName, string? lastName)
        {
            Result updateResult = new Result();

            if (firstName == null) 
            {
                updateResult = await _updateDataAccess.Update(
                    _tableName,
                    new List<Comparator>()
                    {
                        new Comparator(_id, "=", userId),
                    },
                    new Dictionary<string, object>
                    {
                        {_lastName, lastName!}
                    }
                ).ConfigureAwait(false);

                return updateResult;
            }
            else if (lastName == null)
            {
                updateResult = await _updateDataAccess.Update(
                    _tableName,
                    new List<Comparator>()
                    {
                    new Comparator(_id, "=", userId),
                    },
                    new Dictionary<string, object>
                    {
                    {_firstName, firstName!}
                    }
                ).ConfigureAwait(false);

                return updateResult;
            }

            updateResult = await _updateDataAccess.Update(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator(_id, "=", userId),
                },
                new Dictionary<string, object>
                {
                    {_firstName, firstName!},
                    {_lastName, lastName!}
                }
            ).ConfigureAwait(false);

            return updateResult;
        }

        public async Task<Result<AccountSystemSettings>> GetAccountSettings(int userId)
        {
            Result<AccountSystemSettings> result = new Result<AccountSystemSettings>();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new List<string>() { _firstName, _lastName },
                new List<Comparator>()
                {
                    new Comparator(_id, "=", userId)
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
            result.Payload = new AccountSystemSettings()
            {
                FirstName = payload.First()[_firstName] == DBNull.Value ? null : (string)payload.First()[_firstName],
                LastName = payload.First()[_lastName] == DBNull.Value ? null : (string)payload.First()[_lastName]
            };


            return result;
        }
    }
}

