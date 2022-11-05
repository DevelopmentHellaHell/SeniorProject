using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using DevelopmentHell.Hubba.Models;


namespace DevelopmentHell.Hubba.SqlDataAccess.Implementation
{

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
            catch (Exception e)
            {
                return new Result(false, e.Message);
            }
        }

        public Result Insert(string table, Dictionary<string, object> values)
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
                insertQuery.CommandText = string.Format("INSERT into {0}({1}) VALUES ({2})", table, columnString, valueString);

                return SendQuery(insertQuery);
            }
        }
    }
}
