using System.Data;
using System.Text;
using DevelopmentHell.Hubba.Models;
using Microsoft.Data.SqlClient;

namespace DevelopmentHell.Hubba.SqlDataAccess.Implementation
{
    internal class ExecuteDataAccess
    {
        private string _connectionPath;
        public ExecuteDataAccess(string connectionPath)
        {
            _connectionPath = connectionPath;
        }

        private async Task<Result<List<Dictionary<string, object>>>> SendQuery(SqlCommand query)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionPath))
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

        public async Task<Result<List<Dictionary<string, object>>>> Execute(string procedure, Dictionary<string, object> parameters)
        {
            using (SqlCommand executeQuery = new SqlCommand())
            {
                bool first = true;
                StringBuilder valueSb = new();
                foreach (var pair in parameters)
                {
                    if (!first)
                    {
                        valueSb.Append(", ");
                    }
                    first = false;
                    valueSb.Append($"@{pair.Key} = @V_{pair.Key}");

                    executeQuery.Parameters.Add(new SqlParameter($"V_{pair.Key}", pair.Value));
                }
                executeQuery.CommandText = string.Format("EXECUTE {0} {1}", procedure, valueSb.ToString());

                return await SendQuery(executeQuery).ConfigureAwait(false);
            }
        }
    }
}
