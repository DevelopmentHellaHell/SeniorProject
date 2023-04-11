﻿using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
	public class RecoveryRequestDataAccess : IRecoveryRequestDataAccess
	{
		private InsertDataAccess _insertDataAccess;
		private SelectDataAccess _selectDataAccess;
		private DeleteDataAccess _deleteDataAccess;
		private UpdateDataAccess _updateDataAccess;
		private string _tableName;
		public RecoveryRequestDataAccess(string connectionString, string tableName)
		{
			_insertDataAccess = new InsertDataAccess(connectionString);
			_selectDataAccess = new SelectDataAccess(connectionString);
			_deleteDataAccess = new DeleteDataAccess(connectionString);
			_updateDataAccess = new UpdateDataAccess(connectionString);
			_tableName = tableName;
		}

		public async Task<Result> AddManualRecovery(int accoundId)
		{
			Result insertResult = await _insertDataAccess.Insert(_tableName, new() {
				{ "Id" , accoundId },
				{ "RequestTime", DateTime.Now }
			}).ConfigureAwait(false);

			return insertResult;
		}


		public async Task<Result> GetAccounts()
		{
			Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
				_tableName,
				new List<string>() { "Id" },
				new List<Comparator>()
				{
					new Comparator(),
				}
			).ConfigureAwait(false);
			return selectResult;
		}

		public async Task<Result<int>> GetId(int accountId)
		{
			Result<int> result = new Result<int>();

			Result<List<Dictionary<string, object>>> selectResult = await _selectDataAccess.Select(
				_tableName,
				new List<string>() { "Id" },
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