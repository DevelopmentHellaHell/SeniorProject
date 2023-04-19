using DevelopmentHell.Hubba.Models;
using Microsoft.Data.SqlClient;
using System.Windows.Markup;

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
        public async Task<Result> BatchInsert(string table, List<string> keys, List<List<object>> values)
        {
            using (SqlCommand insertQuery = new SqlCommand())
            {
                string columnString = string.Join(", ", keys);
                string valueString = string.Join(", ", values.Select(row => "(" + string.Join(", ", Enumerable.Range(0, keys.Count).Select(i => "@param" + i.ToString() + "_" + row.GetHashCode().ToString())) + ")"));

                for (int i = 0; i < keys.Count; i++)
                {
                    for (int j = 0; j < values.Count; j++)
                    {
                        insertQuery.Parameters.AddWithValue("@param" + i.ToString() + "_" + values[j].GetHashCode().ToString(), values[j][i]);
                    }
                }

                insertQuery.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES {2}", table, columnString, valueString);

                return await SendQuery(insertQuery).ConfigureAwait(false);
            }
        }

        public async Task<Result> InsertOutput(string table, Dictionary<string, object> values, string ouputColumn)
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
                
                var tableVar = "@INSERTEDBOOKING";
                insertQuery.CommandText = string.Format(
                    "DECLARE {4} TABLE ({3} INT); \n" +
                    "INSERT INTO {0} " +
                    "({1}) " +
                    "OUTPUT INSERTED.{3} INTO {4} " +
                    "VALUES ({2}); \n" +
                    "SELECT BookingId FROM {4}", 
                    table, columnString, valueString, ouputColumn, tableVar);

                return await SendQuery(insertQuery).ConfigureAwait(false);
            }
        }
    }
}