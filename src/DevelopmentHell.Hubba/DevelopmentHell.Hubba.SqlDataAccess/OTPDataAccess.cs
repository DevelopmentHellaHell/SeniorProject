using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
	public class OTPDataAccess : IOTPDataAccess
	{
		private UpdateDataAccess _updateDataAccess;
		private SelectDataAccess _selectDataAccess;
		private DeleteDataAccess _deleteDataAccess;
		private InsertDataAccess _insertDataAccess;
		private readonly string _tableName;

		public OTPDataAccess(string connectionString, string tableName)
		{
			_insertDataAccess = new(connectionString);
			_selectDataAccess = new(connectionString);
			_deleteDataAccess = new(connectionString);
			_updateDataAccess = new(connectionString);
			_tableName = tableName;
		}
		public async Task<Result> NewOTP(int accountId, byte[] encryptedOTP, DateTime expiration)
		{
			Result result = new Result();

			Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(_tableName,
				new List<string>() { "UserAccountId" },
				new List<Comparator>() {
					new("UserAccountId", "=", accountId)
				}
			).ConfigureAwait(false);

			if (!selectResult.IsSuccessful || selectResult.Payload is null)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = selectResult.ErrorMessage;
				return result;
			}

			if (selectResult.Payload.Count > 1)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = "Multiple UserOTPs selected.";
				return result;
			}

			if (selectResult.Payload.Count <= 0)
			{
				return await Insert(accountId, encryptedOTP, expiration).ConfigureAwait(false);
			}
			else
			{
				return await Update(accountId, encryptedOTP, expiration).ConfigureAwait(false);
			}
		}
		private async Task<Result> Insert(int accountId, byte[] encryptedOTP, DateTime expiration)
		{
			return await _insertDataAccess.Insert(_tableName, new() {
				{ "UserAccountID", accountId },
				{ "Expiration", expiration },
				{ "Passphrase", encryptedOTP }
			}).ConfigureAwait(false);
		}
		private async Task<Result> Update(int accountId, byte[] encryptedOTP, DateTime expiration)
		{

			Result updateResult = await _updateDataAccess.Update(_tableName, new() { new("UserAccountId", "=", accountId) }, new()
			{
				{ "Expiration", expiration },
				{ "Passphrase", encryptedOTP },
			}).ConfigureAwait(false);
			return updateResult;
		}

		public async Task<Result<byte[]>> GetOTP(int accountId)
		{
			DateTime now = DateTime.Now;
			Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
				_tableName,
				new() { "Passphrase" },
				new()
				{
					new("UserAccountId", "=", accountId),
					new("Expiration", ">", now)
				}
			).ConfigureAwait(false);

			Result<byte[]> result = new Result<byte[]>();
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
				result.ErrorMessage = "Multiple UserOTPs selected.";
				return result;
			}

			result.IsSuccessful = true;
			if (payload.Count > 0) result.Payload = (byte[])(payload[0]["Passphrase"]);
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
