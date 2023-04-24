using DevelopmentHell.Hubba.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Text;

namespace DevelopmentHell.Hubba.SqlDataAccess.Implementations
{
    internal class SelectDataAccess
    {
        private string connectionPath;
        public SelectDataAccess(string inPath)
        {
            connectionPath = inPath;
        }

        private async Task<Result<List<Dictionary<string, object>>>> SendQuery(SqlCommand query)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionPath))
                {
                    query.Connection = conn;
                    List<Dictionary<string, object>> payload = new();
                    await conn.OpenAsync().ConfigureAwait(false);
                    using (SqlDataReader reader = await query.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, object> nextLine = new();
                            IDataRecord dataRecord = reader;
                            for (int i = 0; i < dataRecord.FieldCount; i++)
                            {
                                nextLine.Add(reader.GetName(i), dataRecord.GetValue(i));
                            }
                            payload.Add(nextLine);
                        }
                        return new Result<List<Dictionary<string, object>>>()
                        {
                            IsSuccessful = true,
                            Payload = payload,
                        };
                    }
                }

            }
            catch (Exception e)
            {
                return new Result<List<Dictionary<string, object>>>()
                {
                    IsSuccessful = false,
                    ErrorMessage = e.Message,
                };
            }
        }

        public async Task<Result<List<Dictionary<string, object>>>> Select(string source, List<string> columns, List<Comparator> filters, string group = "", string order = "", int count = -1, int offset = 0)
        {
            //TODO add implementation for group by, order by, having
            using (SqlCommand insertQuery = new SqlCommand())
            {

                bool first = true;
                StringBuilder sbFilter = new();
                StringBuilder sbColumn = new();
                foreach (var filter in filters)
                {
                    if (!first)
                    {
                        sbFilter.Append(" AND ");
                    }
                    first = false;
                    sbFilter.Append($"{filter.Key} {filter.Op} @{filter.Key.ToString()!.Replace(".","")}");

                    insertQuery.Parameters.Add(new SqlParameter(filter.Key.ToString()!.Replace(".", ""), filter.Value.ToString()));
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

                string groupBy = "";
                if (group != String.Empty)
                {
                    groupBy = $"GROUP BY {group}";
                }

                string orderBy = "";
                if (order != String.Empty)
                {
                    orderBy = $"ORDER BY {order}";
                }

                string top = "";
                if (count != -1)
                {
                    top = $"TOP {count}";
                }

                string offsetString = "";
                if (offset > 0)
                {
                    offsetString = $"OFFSET {offset} ROWS";
                }

                insertQuery.CommandText = $"SELECT {top} {sbColumn.ToString()} FROM {source} WHERE {sbFilter.ToString()} {groupBy} {orderBy} {offsetString}";
                return await SendQuery(insertQuery).ConfigureAwait(false);
            }
        }
        public async Task<Result<List<Dictionary<string, object>>>> SelectWhereIn(string source, List<string> columns, string key, List<string> inValues)
        {
            using (SqlCommand insertQuery = new SqlCommand())
            {
                bool first = true;
                StringBuilder sbInValues = new();
                foreach(string value in inValues)
                {
                    if (!first)
                    {
                        sbInValues.Append(", ");
                    }
                    first = false;
                    sbInValues.Append($"'{value}'");
                }

                first = true;
                StringBuilder sbColumn = new();
                foreach (string column in columns)
                {
                    if (!first)
                    {
                        sbColumn.Append(", ");
                    }
                    first = false;
                    sbColumn.Append(column);
                }
                if (sbInValues.Length == 0)
                    sbInValues.Append("''");
                insertQuery.CommandText = $"SELECT {sbColumn} FROM {source} WHERE {key} IN ({sbInValues})";
                return await SendQuery(insertQuery).ConfigureAwait(false);
            }
        }
        public async Task<Result<List<Dictionary<string, object>>>> SelectInnerJoin(List<string> columns, List<Comparator> filters, 
            string table1, string table2, string columnJoin1, string columnJoin2)
        {
            using (SqlCommand insertQuery = new SqlCommand())
            {
                bool first = true;
                StringBuilder sbFilter = new();
                StringBuilder sbColumn = new();
                foreach (var filter in filters)
                {
                    if (!first)
                    {
                        sbFilter.Append(" AND ");
                    }
                    first = false;
                    sbFilter.Append($"{filter.Key} {filter.Op} @{filter.Key}");

                    insertQuery.Parameters.Add(new SqlParameter(filter.Key.ToString(), filter.Value.ToString()));
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

                insertQuery.CommandText = $"SELECT {sbColumn.ToString()} FROM {table1} INNER JOIN {table2} ON " +
                    $"{table1}.{columnJoin1} = {table2}.{columnJoin2} WHERE {sbFilter.ToString()}";
                return await SendQuery(insertQuery).ConfigureAwait(false);
            }
        }
    }
}