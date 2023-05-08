using System.Security.Claims;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.Tests;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;

namespace DevelopmentHell.Hubba.Testing.Service.Implementations
{
    public class TestingService : ITestingService
    {
        private readonly string _jwtKey;
        private TestsDataAccess _testsDataAccess;

        public TestingService(string jwtKey, TestsDataAccess testsDataAccess)
        {
            _jwtKey = jwtKey;
            _testsDataAccess = testsDataAccess;
        }

        public void DecodeJWT(string accessToken, string? idToken = null)
        {
            var jwtHandlerService = new JWTHandlerService(_jwtKey);
            if (accessToken is not null)
            {
                if (jwtHandlerService.ValidateJwt(accessToken))
                {
                    var principal = jwtHandlerService.GetPrincipal(accessToken);
                    Thread.CurrentPrincipal = principal;
                }
                else
                {
                    Thread.CurrentPrincipal = null;
                }
            }

            if (idToken is not null && accessToken is not null && Thread.CurrentPrincipal is not null)
            {
                if (!jwtHandlerService.ValidateJwt(idToken))
                {
                    Thread.CurrentPrincipal = null;
                }
            }

            if (Thread.CurrentPrincipal is null)
            {
                Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("role", "DefaultUser") }));
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
            return await _testsDataAccess.DeleteAllRecords().ConfigureAwait(false);
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