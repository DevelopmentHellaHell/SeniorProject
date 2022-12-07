using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class OTPDataAccess : IOTPDataAccess
    {
        private UpdateDataAccess _updateDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private InsertDataAccess _insertDataAccess;
        private readonly string _tableName = ConfigurationManager.AppSettings["OTPTable"]!;

        public OTPDataAccess(string connectionString)
        {
            _insertDataAccess = new(connectionString);
            _selectDataAccess = new(connectionString);
            _deleteDataAccess = new(connectionString);
            _updateDataAccess = new(connectionString);
        }
        public async Task<Result> NewOTP(int accountId, string encryptedOTP)
        {
            var accountCheck = _selectDataAccess.Select(_tableName, new List<string>() { "*" }, new List<Comparator>() { new("UserAccountId", "=", accountId) }).Result.Payload;
            if ( ((List<Object>)accountCheck!).Count > 0)
            {
                return await Update(accountId, encryptedOTP).ConfigureAwait(false);
            }
            else
            {
                return await Insert(accountId, encryptedOTP).ConfigureAwait(false);
            }
        }
        private async Task<Result> Insert(int accountId, string encryptedOTP)
        {
            return await _insertDataAccess.Insert(_tableName, new() {
                { "UserAccountID", accountId },
                { "Expiration", DateTime.UtcNow.AddSeconds(Convert.ToDouble(ConfigurationManager.AppSettings["OTPExpirationOffsetSeconds"]!)) },
                { "UserOTP", encryptedOTP }
            }).ConfigureAwait(false);
        }
        private async Task<Result> Update(int accountId, string encryptedOTP)
        {
            DateTime expirationDateTime = DateTime.UtcNow.AddSeconds(Convert.ToDouble(ConfigurationManager.AppSettings["OTPExpirationOffsetSeconds"]!));
            Result updateResult = await _updateDataAccess.Update("UserOTP", new() { new("UserAccountId","=",accountId) }, new()
            {
                { "ExpirationDateTime", expirationDateTime },
                { "Passphrase", encryptedOTP },
            }).ConfigureAwait(false);
            return updateResult;
        }

        public async Task<Result> Check(int accountId, string encryptedOTP)
        {
            DateTime now = DateTime.UtcNow;
            Result selectResult = await _selectDataAccess.Select(
                SQLManip.InnerJoinTables(new Joiner("UserOTP", "UserAccount", "Email", "Email")),
                new() { "*" },
                new()
                {
                    new("Passphrase", "=", encryptedOTP),
                    new("UserAccountId", "=", accountId),
                    new(now, "<", "UserOTP.ExpirationDateTime")
                }
            ).ConfigureAwait(false);
            return selectResult;
        }

        public async Task<Result> Delete(int accountId)
        {
            return await _deleteDataAccess.Delete(_tableName, new() { new("UserAccountId", "=", accountId) }).ConfigureAwait(false);
        }
    }
}
