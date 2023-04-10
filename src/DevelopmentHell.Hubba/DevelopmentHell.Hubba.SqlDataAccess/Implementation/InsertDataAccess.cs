using DevelopmentHell.Hubba.Models;
using Microsoft.Data.SqlClient;

namespace DevelopmentHell.Hubba.SqlDataAccess.Implementations
{

	internal class InsertDataAccess
	{
		private string connectionPath;
		public InsertDataAccess(string inPath)
		{
			connectionPath = inPath;
		}
		private async Task<Result> SendQuery(SqlCommand query)
		{
			try
			{
				Result result = new Result();
				int queryResult;
				using (SqlConnection conn = new SqlConnection(connectionPath))
				{
					query.Connection = conn;

					await conn.OpenAsync().ConfigureAwait(false);
					queryResult = await query.ExecuteNonQueryAsync().ConfigureAwait(false);
				}
				// TODO: figure out what to fill these with
				
				result.IsSuccessful = true;
                return result;
            }
			catch (Exception e)
			{
				return new Result()
				{
					IsSuccessful = false,
					ErrorMessage = e.Message,
				};
			}
		}

		public async Task<Result> Insert(string table, Dictionary<string, object> values)
		{
			using (SqlCommand insertQuery = new SqlCommand())
			{
				string columnString = "";
				string valueString = "";
				bool first = true;
				foreach (KeyValuePair<string, object> pair in values)
				{
					if (!first)
					{
						columnString += ", ";
						valueString += ", ";
					}
					first = false;
					columnString += pair.Key;
					valueString += '@' + pair.Key;


					insertQuery.Parameters.Add(new SqlParameter(pair.Key, pair.Value));
				}
				insertQuery.CommandText = string.Format("INSERT into {0} ({1}) VALUES ({2})", table, columnString, valueString);

				return await SendQuery(insertQuery).ConfigureAwait(false);
			}
		}
        public async Task<Result> InsertAll(string table, List<Dictionary<string, object>> valuesList)
        {
            using (SqlCommand insertQuery = new SqlCommand())
            {
                string columnString = "";
                string valueString = "";
                bool first = true;
                foreach (KeyValuePair<string, object> pair in valuesList[0])
                {
                    if (!first)
                    {
                        columnString += ", ";
                        valueString += ", ";
                    }
                    first = false;
                    columnString += pair.Key;
                    valueString += '@' + pair.Key;

                    insertQuery.Parameters.Add(new SqlParameter(pair.Key, DBNull.Value));
                }
				Result result = new Result();
                insertQuery.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ", table, columnString);

                for (int i = 0; i < valuesList.Count; i++)
                {
                    Dictionary<string, object> values = valuesList[i];
                    if (i > 0)
                    {
                        insertQuery.CommandText += ", ";
                    }
                    insertQuery.CommandText += "(" + valueString + ")";
                    for (int j = 0; j < insertQuery.Parameters.Count; j++)
                    {
                        insertQuery.Parameters[j].Value = values[insertQuery.Parameters[j].ParameterName];
                    }
                    result = await SendQuery(insertQuery).ConfigureAwait(false);

                }
				return result;
            }
        }
    }
}
