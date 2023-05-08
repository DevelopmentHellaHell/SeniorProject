using DevelopmentHell.Hubba.Models;
using Microsoft.Data.SqlClient;

namespace DevelopmentHell.Hubba.SqlDataAccess.Implementation
{
    public class DataAccessBase
    {
        readonly string _connectionPath;
        DataAccessBase(string connectionString)
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
                    await query.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
                // TODO: figure out what to fill these with
                return new Result()
                {
                    IsSuccessful = true,
                };
            }
            catch (Exception e)
            {
                return new Result()
                {
                    IsSuccessful = false,
                    ErrorMessage = e.Message,
                };
            }
        }
    }
}
