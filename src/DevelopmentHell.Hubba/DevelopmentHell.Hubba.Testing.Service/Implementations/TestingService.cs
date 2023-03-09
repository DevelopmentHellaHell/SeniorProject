using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.Tests;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;

namespace DevelopmentHell.Hubba.Testing.Service.Implementations
{
    public class TestingService : ITestingService
    {
        private TestsDataAccess _testsDataAccess;
        public TestingService(TestsDataAccess testsDataAccess)
        {
            _testsDataAccess = testsDataAccess;
        }

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

        public async Task<Result> DeleteDatabaseRecords(Databases db)
        {
            return await _testsDataAccess.DeleteDatabaseRecords(db).ConfigureAwait(false);
        }

        public async Task<Result> DeleteTableRecords(Databases db, Tables t)
        {
            return await _testsDataAccess.DeleteTableRecords(db, t).ConfigureAwait(false);
        }

        public async Task<Result> DeleteAllRecords()
        {
            return await _testsDataAccess.DeleteAllRecords();
        }

        public Databases? GetDatabase(string dbStr)
        {
            return _testsDataAccess.GetDatabase(dbStr);
        }

        public Tables? GetTable(Databases db, string tStr)
        {
            return _testsDataAccess.GetTable(db, tStr);
        }
    }
}