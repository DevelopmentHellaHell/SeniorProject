using DevelopmentHell.Hubba.Models;
using Microsoft.Data.SqlClient;
using System.Text;

namespace DevelopmentHell.Hubba.SqlDataAccess.Implementations
{
    public class DeleteDataAccess : AlterTableDataAccessBase
    {
        public DeleteDataAccess(string connectionString) : base(connectionString)
        {
        }

        public async Task<Result> Delete(string source, List<Comparator>? filters)
        {
            using (SqlCommand deleteCommand = new SqlCommand())
            {
                StringBuilder sbFilter = new();
                if (filters is not null && filters.Count > 0)
                {
                    sbFilter.Append(" WHERE ");
                    bool first = true;
                    foreach (var filter in filters)
                    {
                        if (!first)
                        {
                            sbFilter.Append(" AND ");
                        }
                        first = false;
                        if (filter.Op.ToLower() == "in" && filter.Value is string inValues)
                        {
                            
                            string[] valueArray = inValues.Split(',');
                            string[] paramNames = new string[valueArray.Length];
                            for (int i = 0; i < valueArray.Length; i++)
                            {
                                string paramName = $"{filter.Key}_in{i}";
                                deleteCommand.Parameters.AddWithValue(paramName, valueArray[i]);
                                paramNames[i] = $"@{paramName}";
                            }
                            sbFilter.Append($"{filter.Key} IN ({string.Join(",", paramNames)})");
                        }
                        else
                        {
                            // Add parameter normally
                            sbFilter.Append($"{filter.Key} {filter.Op} @{filter.Key}");
                            deleteCommand.Parameters.AddWithValue(filter.Key.ToString(), filter.Value);
                        }
                    }
                }

                deleteCommand.CommandText = $"DELETE FROM {source} {sbFilter.ToString()}";
                return await SendQuery(deleteCommand).ConfigureAwait(false);
            }
        }

        public async Task<Result> DeleteWhereIn(string source, string key, List<string> inValues)
        {
            using (SqlCommand insertQuery = new SqlCommand())
            {
                bool first = true;
                StringBuilder sbInValues = new();
                foreach (string value in inValues)
                {
                    if (!first)
                    {
                        sbInValues.Append(", ");
                    }
                    first = false;
                    sbInValues.Append($"'{value}'");
                }

                insertQuery.CommandText = $"DELETE FROM {source} WHERE {key} IN ({sbInValues})";
                return await SendQuery(insertQuery).ConfigureAwait(false);
            }
        }
    }
}
