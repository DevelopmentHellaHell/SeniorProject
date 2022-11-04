using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public class Result
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public object? Payload { get; set; }

    }
    public class InsertDataAccess
    {
        private string connectionPath;
        public InsertDataAccess(string inPath)
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
            catch
            {
                return new Result();
            }
        }

        public Result Insert(string table, Dictionary<string, Object> values)
        {
            using (SqlCommand insertQuery = new SqlCommand())
            {
                string columnString = "";
                string valueString = "";
                bool first = true;
                foreach (KeyValuePair<string, Object> pair in values)
                {
                    if (!first)
                    {
                        columnString += ", ";
                        valueString += ", ";
                    }
                    first = false;
                    columnString += pair.Key;
                    valueString += '@' + pair.Key;


                    insertQuery.Parameters.Add(new SqlParameter(pair.Key, (string)(pair.Value)));
                }
                insertQuery.CommandText = String.Format("INSERT into {0}({1}) VALUES ({2})", table, columnString, valueString);

                return SendQuery(insertQuery);
            }
        }
    }
}
