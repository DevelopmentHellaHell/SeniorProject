using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    var queryResult = await query.ExecuteNonQueryAsync().ConfigureAwait(false);
                    return new (Result.Success());
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
    }
}
