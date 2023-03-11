using DevelopmentHell.Hubba.Models;
using Microsoft.Data.SqlClient;
using System.Text;

namespace DevelopmentHell.Hubba.SqlDataAccess.Implementations
{
	public class DeleteDataAccess
	{
		private string _connectionPath;
		public DeleteDataAccess(string connectionString)
		{
			_connectionPath = connectionString;
		}
		private async Task<Result> SendQuery(SqlCommand query)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(_connectionPath))
				{
					query.Connection = conn;

					await conn.OpenAsync().ConfigureAwait(false);
					await query.ExecuteNonQueryAsync().ConfigureAwait(false);
				}
				// TODO: figure out what to fill these with
				return new Result()
				{
					IsSuccessful = true,
				};
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

		public async Task<Result> Delete(string source, List<Comparator>? filters)
		{

			//TODO add implementation for group by, order by, having
			using (SqlCommand insertQuery = new SqlCommand())
			{
				bool first = true;
				StringBuilder sbFilter = new();
				string where = "";
				if (filters is not null)
				{
					foreach (var filter in filters)
					{
						if (!first)
						{
							sbFilter.Append(" AND ");
						}
						first = false;
						sbFilter.Append($"{filter.Key} {filter.Op} @{filter.Key}");

						insertQuery.Parameters.Add(new SqlParameter(filter.Key.ToString(), filter.Value.ToString()));
					}
					where = $" WHERE {sbFilter.ToString()}";
				}

				insertQuery.CommandText = $"DELETE FROM {source}";

				return await SendQuery(insertQuery).ConfigureAwait(false);
			}
		}
	}
}
