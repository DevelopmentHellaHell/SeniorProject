using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    //TODO: move class Result into a new project called DevelopmentHell.Hubba.Models 
    public class Result
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public object? Payload { get; set; }
        public Result(bool IsSuccessful = true, string ErrorMessage = "", object? Payload = null)
        {
            this.IsSuccessful = IsSuccessful;
            this.ErrorMessage = ErrorMessage;
            this.Payload = Payload;
        }

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
            catch(Exception e)
            {
                return new Result(false, e.Message);
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


                    insertQuery.Parameters.Add(new SqlParameter(pair.Key, (pair.Value)));
                }
                insertQuery.CommandText = String.Format("INSERT into {0}({1}) VALUES ({2})", table, columnString, valueString);

                return SendQuery(insertQuery);
            }
        }
    }
}
