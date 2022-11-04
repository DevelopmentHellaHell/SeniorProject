using Microsoft.Data.SqlClient;
using System.Text;

namespace DevelopmentHell.Hubba.SqlDataAccess
{

    /*    public enum ResultStatus
        {
            Unknown = 0,
            Success = 1,
            Faulty = 2
        }*/

    public class UpdateDataAccess
    {
        private string connectionPath;
        public UpdateDataAccess(string inPath)
        {
            connectionPath = inPath;
        }

        private Result SendQuery(SqlCommand query)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionPath))
                {
                    query.Connection = conn;

                    conn.Open();
                    query.ExecuteNonQuery();
                }
                // TODO: figure out what to fill these with
                return new Result();
            }
            catch(Exception e)
            {
                return new Result();
            }
        }

        public Result Update(string table, Tuple<string, object> key, Dictionary<string, object> values)
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

                    insertQuery.Parameters.Add(new SqlParameter(pair.Key, (pair.Value)));
                }
                insertQuery.Parameters.Add(new SqlParameter(key.Item1, (key.Item2)));
                insertQuery.CommandText = String.Format("Update {0} SET {1} WHERE {2} = {3}", table, sb.ToString(), key.Item1, key.Item1);

                return SendQuery(insertQuery);
            }
        }

    }
}