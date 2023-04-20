using System;
using System.Collections.Generic;
using System.Data;
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
                    await query.ExecuteNonQueryAsync().ConfigureAwait(false);
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
