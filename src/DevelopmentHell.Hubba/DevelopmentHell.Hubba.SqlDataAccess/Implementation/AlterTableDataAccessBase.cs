using System.Data;
using DevelopmentHell.Hubba.Models;
using Microsoft.Data.SqlClient;

namespace DevelopmentHell.Hubba.SqlDataAccess.Implementations
{
    public class AlterTableDataAccessBase
    {
        private readonly string _connectionPath;
        public AlterTableDataAccessBase(string connectionString)
        {
            _connectionPath = connectionString;
        }

        protected async Task<Result> SendQuery(SqlCommand query)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionPath))
                {
                    query.Connection = conn;
                    await conn.OpenAsync().ConfigureAwait(false);
                    int rowsAffected = await query.ExecuteNonQueryAsync().ConfigureAwait(false);
                    return Result<int>.Success(rowsAffected);
                }
            }
            catch (Exception e)
            {
                return new(Result.Failure(e.Message));
            }
        }
        protected async Task<Result> SendScalarQuery(SqlCommand query)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionPath))
                {
                    query.Connection = conn;
                    await conn.OpenAsync().ConfigureAwait(false);
                    var queryResult = await query.ExecuteScalarAsync().ConfigureAwait(false);
                    return Result<int>.Success(Convert.ToInt32(queryResult));
                }
            }
            catch (Exception e)
            {
                return new(Result.Failure(e.Message));
            }
        }

        protected async Task<Result<List<Dictionary<string, object>>>> SendQueryWithOutput(SqlCommand query)
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
    }
}
