using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class AuthorizationDataAccess : IAuthorizationDataAccess
    {
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private InsertDataAccess _insertDataAccess;
        private readonly string _userRolesTableName;
        public AuthorizationDataAccess(string connectionString, string userRolesTableName)
        {
            _selectDataAccess = new(connectionString);
            _deleteDataAccess = new(connectionString);
            _insertDataAccess = new(connectionString);
            _userRolesTableName = userRolesTableName;
        }
        public async Task<Result<List<Role>>> GetRoles(int accountId)
        {
            Result<List<Role>> output = new();

            Result<List<Dictionary<string,object>>> selectResults = await _selectDataAccess.Select(
                _userRolesTableName,
                new() { "RoleNumber" },
                new()
                {
                    new("AccountId","=",accountId)
                }
            ).ConfigureAwait(false);

            if (!selectResults.IsSuccessful || selectResults.Payload is null)
            {
                output.IsSuccessful = false;
                output.ErrorMessage = (selectResults.ErrorMessage is null) ? "Select result empty" : selectResults.ErrorMessage;
                return output;
            }

            output.IsSuccessful = true;
            if (selectResults.Payload.Count == 0)
            {
                output.Payload = new()
                {
                    Role.DEFAULT
                };
            }
            else
            {
                output.Payload = new();
                foreach (Dictionary<string, object> line in selectResults.Payload)
                {
                    output.Payload.Add((Role)line["RoleNumber"]);
                }
            }
            return output;
        }

        public async Task<Result<bool>> hasAccess(int accountId, string claim)
        {
            Result<bool> output = new();

            Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
                SQLManip.InnerJoinTables(new List<Joiner>()
                {
                    new("UserAccounts","UserRoles","Id","AccountId"),
                    new("UserRoles","RoleClaims","RoleNumber","RoleNumber")
                }
                ),
                new List<string>() { "*" },
                new()
                {
                    new("UserAccounts.Id", "=", accountId),
                    new("RoleClaims.ClaimType","=",claim),
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                output.IsSuccessful = false;
                output.ErrorMessage = (selectResult.ErrorMessage is null) ? "Unable to Complete Select" : selectResult.ErrorMessage;
                return output;
            }

            if (selectResult.Payload.Count > 0)
            {
                output.IsSuccessful = true;
                output.Payload = true;
                return output;
            }

            output.IsSuccessful = true;
            output.Payload = false;
            return output;
        }

        public async Task<Result> GiveRole(int accountId, Role role)
        {
            Result output = new();

            Result<List<Dictionary<string, object>>> selectResults = await _selectDataAccess.Select(
                _userRolesTableName,
                new() { "RoleNumber" },
                new()
                {
                    new("AccountId","=",accountId),
                    new("RoleNumber","=",role),
                }
            ).ConfigureAwait(false);

            if (!selectResults.IsSuccessful || selectResults.Payload is null)
            {
                output.IsSuccessful = false;
                output.ErrorMessage = (selectResults.ErrorMessage is null) ? "Select result empty" : selectResults.ErrorMessage;
                return output;
            }

            if (selectResults.Payload.Count != 0)
            {
                output.IsSuccessful = false;
                output.ErrorMessage = "Account associated with given account ID already has given role";
                return output;
            }

            Result insertResult = await _insertDataAccess.Insert(
                _userRolesTableName,
                new() {
                    { "AccountId", accountId },
                    { "RoleNumber", role },
                }
            ).ConfigureAwait(false);

            if (insertResult.IsSuccessful)
            {
                output.IsSuccessful = true;
                return output;
            }

            output.IsSuccessful = false;
            return output;
        }

        public async Task<Result> RevokeRole(int accountId, Role role)
        {
            Result output = new();

            Result<List<Dictionary<string, object>>> selectResults = await _selectDataAccess.Select(
                _userRolesTableName,
                new() { "RoleNumber" },
                new()
                {
                    new("AccountId","=",accountId),
                    new("RoleNumber","=",role),
                }
            ).ConfigureAwait(false);

            if (!selectResults.IsSuccessful || selectResults.Payload is null)
            {
                output.IsSuccessful = false;
                output.ErrorMessage = (selectResults.ErrorMessage is null) ? "Select result empty" : selectResults.ErrorMessage;
                return output;
            }

            if (selectResults.Payload.Count == 0)
            {
                output.IsSuccessful = false;
                output.ErrorMessage = "Account associated with given account ID does not have given role";
                return output;
            }

            Result insertResult = await _deleteDataAccess.Delete(
                _userRolesTableName,
                new() {
                    new("AccountId", "=", accountId),
                    new("RoleNumber", "=", role),
                }
            ).ConfigureAwait(false);

            if (insertResult.IsSuccessful)
            {
                output.IsSuccessful = true;
                return output;
            }

            output.IsSuccessful = false;
            return output;
        }

        public async Task<Result> RevokeRoleAll(int accountId)
        {
            Result output = new();

            Result<List<Dictionary<string, object>>> selectResults = await _selectDataAccess.Select(
                _userRolesTableName,
                new() { "RoleNumber" },
                new()
                {
                    new("AccountId","=",accountId)
                }
            ).ConfigureAwait(false);

            if (!selectResults.IsSuccessful || selectResults.Payload is null)
            {
                output.IsSuccessful = false;
                output.ErrorMessage = (selectResults.ErrorMessage is null) ? "Select result empty" : selectResults.ErrorMessage;
                return output;
            }

            Result insertResult = await _deleteDataAccess.Delete(
                _userRolesTableName,
                new() {
                    new("AccountId", "=", accountId),
                }
            ).ConfigureAwait(false);

            if (insertResult.IsSuccessful)
            {
                output.IsSuccessful = true;
                return output;
            }

            output.IsSuccessful = false;
            return output;
        }

        public async Task<Result> SetRoles(int accountId, Role[] roles)
        {
            Result output = new();

            Result removeResult = await RevokeRoleAll(accountId).ConfigureAwait(false);
            if (!removeResult.IsSuccessful)
            {
                output.IsSuccessful = false;
                output.ErrorMessage = (removeResult.ErrorMessage is null) ? "Unable to remove roles" : removeResult.ErrorMessage;
                return output;
            }

            foreach (Role role in roles)
            {
                Result insertResult = await GiveRole(accountId, role).ConfigureAwait(false);
                if (!insertResult.IsSuccessful)
                {
                    output.IsSuccessful = false;
                    output.ErrorMessage = (insertResult.ErrorMessage is null) ? "Unable to insert all roles" : insertResult.ErrorMessage;
                    return output;
                }
            }
            output.IsSuccessful = true;
            return output;
        }
    }
}
