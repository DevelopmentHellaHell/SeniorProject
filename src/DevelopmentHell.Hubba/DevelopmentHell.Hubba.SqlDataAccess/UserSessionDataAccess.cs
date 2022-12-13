using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class UserSessionDataAccess
    {
        private InsertDataAccess _insertDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private UpdateDataAccess _updateDataAccess;
        private string _tableName;
        public UserSessionDataAccess(string connectionString, string tableName)
        {
            _insertDataAccess = new InsertDataAccess(connectionString);
            _selectDataAccess = new SelectDataAccess(connectionString);
            _deleteDataAccess = new DeleteDataAccess(connectionString);
            _updateDataAccess = new UpdateDataAccess(connectionString);
            _tableName = tableName;
        }

        public AuthCookieTicket ConvertToTicket(Dictionary<string,object> line)
        {
            return new()
            {
                AccountId = (int)line["AccountId"],
                SessionId = (int)line["SessionId"],
                LastActivity = line["LastActivity"] != DBNull.Value ? (DateTime)line["LastActivity"] : null,
                Expiration = line["Expiration"] != DBNull.Value ? (DateTime)line["Expiration"] : null,
                Self = line["Self"] != DBNull.Value ? (byte[])line["Self"] : null
            };
        }

        public async Task<Result<List<AuthCookieTicket>>> GetUserSessions(int accountId)
        {
            Result<List<AuthCookieTicket>> output = new();

            Result<List<Dictionary<string,object>>> selectResult = await _selectDataAccess.Select(
                _tableName,
                new() { "*" },
                new()
                {
                    new("AccountId","=",accountId)
                }
            ).ConfigureAwait(false);

            if (!selectResult.IsSuccessful || selectResult.Payload is null)
            {
                output.IsSuccessful = false;
                output.ErrorMessage = selectResult.ErrorMessage;
                return output;
            }

            output.Payload = new();

            foreach(var kvp in selectResult.Payload)
            {
                try
                {
                    output.Payload.Add(ConvertToTicket(kvp));
                } catch (Exception e)
                {
                    output.IsSuccessful = false;
                    output.ErrorMessage = "Issue in parsing Session data from db";
                    return output;
                }
            }
            output.IsSuccessful = true;
            return output;
        }

        public async Task<Result> InsertSession(int accountId, DateTime expiration, DateTime lastActivity)
        {
            Result output = new() { IsSuccessful = false };


            Result insertResult =  await _insertDataAccess.Insert(
                _tableName,
                new()
                {
                    { "AccountId", accountId },
                    { "LastActivity", lastActivity },
                    { "Expiration", expiration },
                }
            ).ConfigureAwait(false);

            if (!insertResult.IsSuccessful)
            {
                output.ErrorMessage = insertResult.ErrorMessage;
                return output;
            }


            output.IsSuccessful = true;
            return output;
        }

        public async Task<Result> UpdateSession(AuthCookieTicket updatedSession)
        {
            var values = new Dictionary<string, object>();
            foreach (var column in updatedSession.GetType().GetProperties())
            {
                var value = column.GetValue(updatedSession);
                if (value is null || column.Name == "AccountId" || column.Name == "SessionId") continue;
                values[column.Name] = value;
            }

            Result updateResult = await _updateDataAccess.Update(
                _tableName,
                new List<Comparator>()
                {
                    new Comparator("AccountId", "=", updatedSession.AccountId),
                },
                values
            ).ConfigureAwait(false);

            return updateResult;
        }

        public async Task<Result> PruneSessions()
        {
            return await _deleteDataAccess.Delete(
                _tableName,
                new()
                {
                    new("Expiration", "<", DateTime.UtcNow)
                }
            );
        }
        public async Task<Result> DeleteUserSessions(int accountId)
        {
            return await _deleteDataAccess.Delete(
                _tableName,
                new()
                {
                    new("AccountId", "=", accountId)
                }
            ).ConfigureAwait(false);
        }
    }
}
