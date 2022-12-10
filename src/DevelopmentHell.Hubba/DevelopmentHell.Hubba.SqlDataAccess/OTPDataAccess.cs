using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using System.Configuration;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class OTPDataAccess : IOTPDataAccess
    {
        private UpdateDataAccess _updateDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private InsertDataAccess _insertDataAccess;
        private readonly string _tableName = "UserOTPs";

        public OTPDataAccess(string connectionString)
        {
            _insertDataAccess = new(connectionString);
            _selectDataAccess = new(connectionString);
            _deleteDataAccess = new(connectionString);
            _updateDataAccess = new(connectionString);
        }
        public async Task<Result> NewOTP(int accountId, byte[] encryptedOTP)
        {
            var accountCheck = await _selectDataAccess.Select(_tableName, new List<string>() { "*" }, new List<Comparator>() { new("UserAccountId", "=", accountId) }).ConfigureAwait(false);
            Console.WriteLine(accountCheck.ErrorMessage);
            if ( ((List<List<Object>>)accountCheck.Payload!).Count > 0)
            {
				return await Update(accountId, encryptedOTP).ConfigureAwait(false);
            }
            else
            {
                return await Insert(accountId, encryptedOTP).ConfigureAwait(false);
            }
        }
        private async Task<Result> Insert(int accountId, byte[] encryptedOTP)
        {
            return await _insertDataAccess.Insert(_tableName, new() {
                { "UserAccountID", accountId },
                { "Expiration", DateTime.UtcNow.AddSeconds(Convert.ToDouble(120)) },
                { "Passphrase", encryptedOTP }
            }).ConfigureAwait(false);
        }
        private async Task<Result> Update(int accountId, byte[] encryptedOTP)
        {
            DateTime expirationDateTime = DateTime.UtcNow.AddSeconds(Convert.ToDouble(120));
            Result updateResult = await _updateDataAccess.Update(_tableName, new() { new("UserAccountId","=",accountId) }, new()
            {
                { "Expiration", expirationDateTime },
                { "Passphrase", encryptedOTP },
            }).ConfigureAwait(false);
            return updateResult;
        }

        public async Task<Result> Check(int accountId, string encryptedOTP)
        {
            DateTime now = DateTime.UtcNow;
            Result selectResult = await _selectDataAccess.Select(
                SQLManip.InnerJoinTables(new Joiner("UserOTP", "UserAccount", "UserAccountId", "Id")),
                new() { "*" },
                new()
                {
                    new("Passphrase", "=", encryptedOTP),
                    new("UserAccountId", "=", accountId),
                    new(now, "<", "Expiration")
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
