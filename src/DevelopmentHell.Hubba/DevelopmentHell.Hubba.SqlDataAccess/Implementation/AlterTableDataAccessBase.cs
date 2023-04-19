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
                    int rowsAffected = await query.ExecuteNonQueryAsync().ConfigureAwait(false);
                    Console.WriteLine("Rows affected: " + rowsAffected);
                }
            }
            catch (Exception e)
            {
                return new Result()
                {
                    IsSuccessful = false,
                    ErrorMessage = e.Message,
                };
            }
            return new Result()
            {
                IsSuccessful = true,
            };
        }
    }
}
