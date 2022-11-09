﻿using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration;

namespace DevelopmentHell.Hubba.SqlDataAccess.Implementation
{
    public class SelectDataAccess
    {
        private string connectionPath;
        public SelectDataAccess(string inPath)
        {
            connectionPath = inPath;
        }

        private async Task<Result> SendQuery(SqlCommand query, int columnLength)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionPath))
                {
                    query.Connection = conn;
                    List<List<object>> payload = new();
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await query.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (reader.Read())
                        {
                            List<object> row = new();
                            IDataRecord dataRecord = reader;
                            for (int i = 0; i < columnLength; i++)
                            {
                                row.Add(dataRecord[i]);
                            }
                            payload.Add(row);
                        }
                        return new Result(true, "", payload);
                    }
                }

            }
            catch (Exception e)
            {
                return new Result(false, e.Message);
            }
        }

        public async Task<Result> Select(string table, List<string> columns, Dictionary<string, object> filters)
        {
            //TODO add implementation for group by, order by, having
            using (SqlCommand insertQuery = new SqlCommand())
            {

                bool first = true;
                StringBuilder sbFilter = new();
                StringBuilder sbColumn = new();
                foreach (var pair in filters)
                {
                    if (!first)
                    {
                        sbFilter.Append(" AND ");
                    }
                    first = false;
                    sbFilter.Append(pair.Key + " = @" + pair.Key);

                    insertQuery.Parameters.Add(new SqlParameter(pair.Key, pair.Value));
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
                insertQuery.CommandText = string.Format("SELECT {0} FROM {1} WHERE {2}", sbColumn.ToString(), table, sbFilter.ToString());

                return await SendQuery(insertQuery, columns.Count).ConfigureAwait(false);
            }
        }

    }
}