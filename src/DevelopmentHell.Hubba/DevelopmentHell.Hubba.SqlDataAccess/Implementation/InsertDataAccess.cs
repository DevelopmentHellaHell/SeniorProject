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
    }
}
