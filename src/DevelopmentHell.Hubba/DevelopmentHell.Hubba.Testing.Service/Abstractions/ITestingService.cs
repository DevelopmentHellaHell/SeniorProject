﻿using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.Tests;

namespace DevelopmentHell.Hubba.Testing.Service.Abstractions
{
    public interface ITestingService
    {
        void DecodeJWT(string token, string? jwtKey = null);
        Task<Result> DeleteDatabaseRecords(Databases db);
        Task<Result> DeleteTableRecords(Databases db, Tables t);
        Task<Result> DeleteAllRecords();
        Databases? GetDatabase(string dbStr);
        Tables? GetTable(Databases db, string tStr);
    }
}
