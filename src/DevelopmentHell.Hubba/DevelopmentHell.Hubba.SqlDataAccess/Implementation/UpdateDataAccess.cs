using Microsoft.Data.SqlClient;
using System.Text;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration;

namespace DevelopmentHell.Hubba.SqlDataAccess.Implementation
{
    public class UpdateDataAccess
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
                return new Result();
            }
            catch (Exception e)
            {
                return new Result(false, e.Message);
            }
        }

        public async Task<Result> Update(string table, Tuple<string, object> key, Dictionary<string, object> values)
        {
            using (SqlCommand insertQuery = new SqlCommand())
            {

                bool first = true;
                StringBuilder sb = new();
                foreach (var pair in values)
                {
                    if (!first)
                    {
                        sb.Append(", ");
                    }
                    first = false;
                    sb.Append(pair.Key + " = @" + pair.Key);

                    insertQuery.Parameters.Add(new SqlParameter(pair.Key, pair.Value));
                }
                insertQuery.Parameters.Add(new SqlParameter(key.Item1, key.Item2));
                insertQuery.CommandText = string.Format("UPDATE {0} SET {1} WHERE {2} = {3}", table, sb.ToString(), key.Item1, key.Item1);

                return await SendQuery(insertQuery).ConfigureAwait(false);
            }
        }

    }
}