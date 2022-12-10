using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using System.Configuration;
using System.Text;

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
            Result<List<object>> accountCheck = await _selectDataAccess.Select(_tableName, new List<string>() { "*" }, new List<Comparator>() { new("UserAccountId", "=", accountId) }).ConfigureAwait(false);
            if (accountCheck.Payload.Count > 0)
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

        public async Task<Result<byte[]>> GetOTP(int accountId)
        {
            DateTime now = DateTime.UtcNow;
            Result<List<object>> selectResult = await _selectDataAccess.Select(
			    SQLManip.InnerJoinTables(new Joiner("UserOTPs", "UserAccount", "UserAccountId", "Id")),
                new() { "Passphrase" },
                new()
                {
                    new("UserAccountId", "=", accountId),
                    new("Expiration", ">", now)
                }
            ).ConfigureAwait(false);

            Result<byte[]> result = new Result<byte[]>();
			if (!selectResult.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = selectResult.ErrorMessage;
				return result;
			}

			List<object> payload = selectResult.Payload;
			if (payload.Count <= 0)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "No UserOTP selected with email and time.";
				return result;
			}

			if (payload.Count > 1)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Multiple UserOTPs selected.";
				return result;
			}

            result.IsSuccessful = true;
            result.Payload = (byte[])(payload[0]);
            return result;
		}

		public async Task<Result> Delete(int accountId)
        {
            return await _deleteDataAccess.Delete(
                _tableName,
                new List<Comparator>()
                {
                    new("UserAccountId", "=", accountId)
                }
            ).ConfigureAwait(false);
        }
    }
}
