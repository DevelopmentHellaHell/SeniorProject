using DevelopmentHell.Hubba.Models.Tests;
using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Testing.Service.Abstractions
{
    public interface ITestingService
    {
        void DecodeJWT(string token);
        Task<Result> DeleteDatabaseRecords(Databases db);
        Task<Result> DeleteTableRecords(Databases db, Tables t);
        Task<Result> DeleteAllRecords();
        Databases? GetDatabase(string dbStr);
        Tables? GetTable(Databases db, string tStr);
    }
}
