using DevelopmentHell.Hubba.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace DevelopmentHell.Hubba.SqlDataAccess.Implementation
{
	public class SelectDataAccess
	{
		private string connectionPath;
		public SelectDataAccess(string inPath)
		{
			connectionPath = inPath;
		}

		private async Task<Result<List<Dictionary<string, object>>>> SendQuery(SqlCommand query)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(connectionPath))
				{
					query.Connection = conn;
					List<Dictionary<string, object>> payload = new();
					await conn.OpenAsync().ConfigureAwait(false);
					using (SqlDataReader reader = await query.ExecuteReaderAsync().ConfigureAwait(false))
					{
						while (reader.Read())
						{
							Dictionary<string, object> nextLine = new();
							IDataRecord dataRecord = reader;
							for (int i = 0; i < dataRecord.FieldCount; i++)
							{
								nextLine.Add(reader.GetName(i), dataRecord.GetValue(i));
							}
							payload.Add(nextLine);
						}
						return new Result<List<Dictionary<string, object>>>()
						{
							IsSuccessful = true,
							Payload = payload,
						};
					}
				}

			}
			catch (Exception e)
			{
				return new Result<List<Dictionary<string, object>>>()
				{
					IsSuccessful = false,
					ErrorMessage = e.Message,
				};
			}
		}

		public async Task<Result<List<Dictionary<string, object>>>> Select(string source, List<string> columns, List<Comparator> filters, string group = "", string order = "")
		{
			//TODO add implementation for group by, order by, having
			using (SqlCommand insertQuery = new SqlCommand())
			{

				bool first = true;
				StringBuilder sbFilter = new();
				StringBuilder sbColumn = new();
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
				first = true;
				foreach (string column in columns)
				{
					if (!first)
					{
						sbColumn.Append(", ");
					}
					first = false;
					sbColumn.Append(column);
				}

				string groupBy = "";
				if (group != String.Empty)
				{
					groupBy = $"GROUP BY {group}";
				}

				string orderBy = "";
				if (order != String.Empty)
				{
					orderBy = $"ORDER BY {order}";
				}

				insertQuery.CommandText = $"SELECT {sbColumn.ToString()} FROM {source} WHERE {sbFilter.ToString()} {groupBy} {orderBy}";
				return await SendQuery(insertQuery).ConfigureAwait(false);
			}
		}
	}
}