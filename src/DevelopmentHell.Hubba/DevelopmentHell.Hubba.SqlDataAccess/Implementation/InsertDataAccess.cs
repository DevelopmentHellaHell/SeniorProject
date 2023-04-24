using DevelopmentHell.Hubba.Models;
using Microsoft.Data.SqlClient;

namespace DevelopmentHell.Hubba.SqlDataAccess.Implementations
{

    internal class InsertDataAccess : AlterTableDataAccessBase
    {
        public InsertDataAccess(string connectionString) : base(connectionString)
        {
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

        public async Task<Result<List<Dictionary<string, object>>>> InsertWithOutput(string table, Dictionary<string, object> values, string outputColumn)
        {
            string outputQuery = "";
            string selectDropQuery = "";
            if (outputColumn != null)
            {
                outputQuery = "OUTPUT INSERTED." + outputColumn + " INTO @PrimaryKey";
                selectDropQuery = "SELECT " + outputColumn + " FROM @PrimaryKey; DROP TABLE #PrimaryKey;";
            }

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

                insertQuery.CommandText = string.Format("DECLARE @PrimaryKey TABLE({0} INT);", outputColumn) +
                    string.Format("INSERT INTO  {0} ({1}) {2} VALUES ({3}); {4}",
                    table, columnString, outputQuery, valueString, selectDropQuery);
                return await SendQueryWithOutput(insertQuery).ConfigureAwait(false);
            }

        }
    }
}
