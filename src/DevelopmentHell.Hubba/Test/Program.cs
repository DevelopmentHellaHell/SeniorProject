// See https://aka.ms/new-console-template for more information
using DevelopmentHell.Hubba.Models.Tests;
using DevelopmentHell.Hubba.SqlDataAccess;

Console.WriteLine("Hello, World!");

TestsDataAccess testsDataAccess = new TestsDataAccess();
var result = await testsDataAccess.DeleteDatabaseRecords(Database.USERS);
//var result = await testsDataAccess.DeleteTableRecords(Database.USERS, Tables.USER_OTPS);
Console.WriteLine(result.IsSuccessful);
Console.WriteLine(result.ErrorMessage);