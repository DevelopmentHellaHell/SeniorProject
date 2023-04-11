using DevelopmentHell.Hubba.Models;
using Microsoft.Data.SqlClient;
using System.Text;

namespace DevelopmentHell.Hubba.SqlDataAccess.Implementations
{
    internal class UpdateDataAccess
    {
        private string connectionPath;
        public UpdateDataAccess(string inPath)
        {
            connectionPath = inPath;
        }

        private async Task<Result> SendQuery(SqlCommand query)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionPath))
                {
                    query.Connection = conn;

                    conn.Open();
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

        public async Task<Result> Update(string table, List<Comparator> filters, Dictionary<string, object> values)
        {
            using (SqlCommand insertQuery = new SqlCommand())
            {

                bool first = true;
                StringBuilder valueSb = new();
                StringBuilder filterSb = new();
                foreach (var pair in values)
                {
                    if (!first)
                    {
                        valueSb.Append(", ");
                    }
                    first = false;
                    valueSb.Append(pair.Key + " = @" + pair.Key);

                    insertQuery.Parameters.Add(new SqlParameter(pair.Key, pair.Value));
                }
                first = true;
                foreach (var filter in filters)
                {

                    if (!first)
                    {
                        filterSb.Append(", ");
                    }
                    first = false;
                    filterSb.Append($"{filter.Key} {filter.Op} @{filter.Key}");

                    insertQuery.Parameters.Add(new SqlParameter(filter.Key.ToString(), filter.Value.ToString()));
                }
                insertQuery.CommandText = string.Format("UPDATE {0} SET {1} WHERE {2}", table, valueSb.ToString(), filterSb.ToString());

                return await SendQuery(insertQuery).ConfigureAwait(false);
            }
        }
    }
}