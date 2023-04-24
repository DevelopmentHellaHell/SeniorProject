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

            //TODO add implementation for group by, order by, having
            using (SqlCommand insertQuery = new SqlCommand())
            {
                bool first = true;
                StringBuilder sbFilter = new();
                string where = "";
                if (filters is not null)
                {
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
                    where = $" WHERE {sbFilter.ToString()}";
                }

                insertQuery.CommandText = $"DELETE FROM {source} {where}";

                return await SendQuery(insertQuery).ConfigureAwait(false);
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
