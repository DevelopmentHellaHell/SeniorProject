using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.Tests;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;


namespace DevelopmentHell.Hubba.Testing.Service
{
    public class TestingService
    {
        public TestingService() { }

        public void DecodeJWT(string token)
        {

            if (token is not null)
            {
                // Parse the JWT token and extract the principal
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["JwtKey"]!);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                try
                {
                    SecurityToken validatedToken;
                    var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                    Thread.CurrentPrincipal = principal;
                    return;
                }
                catch (Exception)
                {
                    // Handle token validation errors
                    Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "DefaultUser") }));
                    return;
                }
            }
        }

        private static readonly Dictionary<Databases, Tuple<string, string, Dictionary<Tables, string>>> _databaseStructure = new()
        {
            {
                Databases.LOGS,
                new (
                    "DevelopmentHell.Hubba.Logs",
                    ConfigurationManager.AppSettings["LogsConnectionString"]!,
                    new ()
                    {
                        { Tables.LOGS, ConfigurationManager.AppSettings["LogsTable"]! }
                    }
                )
            },
            {
                Databases.USERS,
                new (
                    "DevelopmentHell.Hubba.Users",
                    ConfigurationManager.AppSettings["UsersConnectionString"]!,
                    new ()
                    {
                        { Tables.RECOVERY_REQUESTS, ConfigurationManager.AppSettings["RecoveryRequestsTable"]! },
                        { Tables.USER_ACCOUNTS, ConfigurationManager.AppSettings["UserAccountsTable"]! },
                        { Tables.USER_LOGINS, ConfigurationManager.AppSettings["UserLoginsTable"]! },
                        { Tables.USER_OTPS, ConfigurationManager.AppSettings["UserOTPsTable"]! }
                    }
                )
            },
        };

        public async Task<Result> DeleteDatabaseRecords(Databases db)
        {
            Result result = new Result();
            var dbT = _databaseStructure[db];
            foreach (string tValue in dbT.Item3.Values)
            {
                DeleteDataAccess deleteDataAccess = new DeleteDataAccess(_databaseStructure[db].Item2);
                Result deleteResult = await deleteDataAccess.Delete(tValue, null).ConfigureAwait(false);
                if (!deleteResult.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = deleteResult.ErrorMessage;
                    return result;
                }
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task<Result> DeleteTableRecords(Databases db, Tables t)
        {
            var dbT = _databaseStructure[db];
            var tValue = dbT.Item3[t];
            DeleteDataAccess deleteDataAccess = new DeleteDataAccess(_databaseStructure[db].Item2);
            return await deleteDataAccess.Delete(tValue, null).ConfigureAwait(false);
        }

        public async Task<Result> DeleteAllRecords()
        {
            Result result = new Result();
            foreach (Databases db in Enum.GetValues(typeof(Databases)))
            {
                Result deleteResult = await DeleteDatabaseRecords(db).ConfigureAwait(false);
                if (!deleteResult.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = deleteResult.ErrorMessage;
                    return result;
                }
            }

            result.IsSuccessful = true;
            return result;
        }

        public Databases? GetDatabase(string dbStr)
        {
            foreach (var kvp in _databaseStructure)
            {
                if (Enum.GetName(kvp.Key)!.ToUpper() == dbStr.ToUpper()) return kvp.Key;
            }

            return null;
        }

        public Tables? GetTable(Databases db, string tStr)
        {
            foreach (var kvp in _databaseStructure[db].Item3)
            {
                if (Enum.GetName(kvp.Key)!.ToUpper() == tStr.ToUpper()) return kvp.Key;
            }

            return null;
        }
    }
}