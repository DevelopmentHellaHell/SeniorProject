﻿using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using System.Configuration;
using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Registration.Test.UnitTests;

[TestClass]
public class RegistrationSqlDataAccessUnitTest
{
    [TestMethod]
    public void ShouldCreateNewInstanceWithParameterCtor()
    {
        // Arrange
        var expected = typeof(InsertDataAccess);
        var expectedDatabaseName = "DevelopmentHell.Hubba.Accounts";
        var connectionString = String.Format(@"Server=localhost\SQLEXPRESS;Database={0};Integrated Security=True;Encrypt=False", expectedDatabaseName);

        // Act
        var actual = new InsertDataAccess(connectionString);

        // Assert
        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.GetType() == expected);

    }
    [TestMethod]
    public async Task ShouldRegisterNewAccountIntoDatabase()
    {
        // TODO: fill out test case
        // Arrange
        var expected = typeof(InsertDataAccess);
        var expectedTableName = "Accounts";
        var expectedDatabaseName = "DevelopmentHell.Hubba.Accounts";
        var connectionString = String.Format(@"Server=localhost\SQLEXPRESS;Database={0};Integrated Security=True;Encrypt=False", expectedDatabaseName);
        string username = "coolkoala";
        string email = @"Email@random.com";
        string passphrase = "c0o1p4s5phra53";
        int id = 5;
        DateTime lastInteraction = DateTime.Now;
        bool admin_account = false;
        Dictionary<string, object> newUserAccountCredentials = new()
        {
            { "username", username },
            { "email", email },
            { "passphrase", passphrase },
            { "lastInteraction", lastInteraction },
            { "id", id}
        };

        // Act
        var actual = new InsertDataAccess(connectionString);
        Result result = await actual.Insert(expectedTableName, newUserAccountCredentials).ConfigureAwait(false);

        // Assert
        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.GetType() == expected);
        Assert.IsTrue(result.IsSuccessful);
    }

    [TestMethod]
    public async Task SouldUpdateAccountInDatabase()
    {
        // TODO: fill out test case
        // Arrange
        var expected = typeof(UpdateDataAccess);
        var expectedTableName = "Accounts";
        var expectedDatabaseName = "DevelopmentHell.Hubba.Accounts";
        var connectionString = String.Format(@"Server=localhost\SQLEXPRESS;Database={0};Integrated Security=True;Encrypt=False", expectedDatabaseName);
        string email = @"Email@random.com";
        Tuple<string, object> key = new Tuple<string, object>("email", email);
        Dictionary<string, object> values = new()
        {
            { "age", 28 }
        };

        // Act
        var actual = new UpdateDataAccess(connectionString);
        Result result = await actual.Update(expectedTableName, key, values).ConfigureAwait(false);
        //int account_id = (int)(result.Payload);

        // Assert
        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.GetType() == expected);
        //Assert.IsTrue((string)(actual.AccessEmail(account_id).Payload) == email);
        //Assert.IsTrue((string)(actual.AccessPassphrase(account_id).Payload) == passphrase);
    }
    [TestMethod]
    public async Task ShouldAccessExistingAccountInDatabase()
    {
        // TODO: fill out test case
        // Arrange
        var expected = typeof(SelectDataAccess);
        var expectedTableName = "Accounts";
        var expectedDatabaseName = "DevelopmentHell.Hubba.Accounts";
        var connectionString = String.Format(@"Server={0};Database={1};Integrated Security=True;Encrypt=False", ConfigurationManager.AppSettings["AccountServer"],expectedDatabaseName);
        Dictionary<string, object> values = new()
        {
            { "age", 28 }
        };
        List<string> expectedColumns = new()
        {
            "email",
            "username",
            "age"
        };
        string username = "sillysnail";
        string email = @"snackinglobsters@random.com";
        string passphrase = "c0o1p4s5phra53";
        DateTime last_interaction = DateTime.Now;
        bool admin_account = false;
        Dictionary<string, object> newUserAccountCredentials = new()
        {
            { "username", username },
            { "email", email },
            { "passphrase", passphrase },
            { "lastInteraction", last_interaction },
            { "adminAccount", admin_account },
            { "birthDate", 28 }

        };
        var actualInsert = new InsertDataAccess(connectionString);
        var insertResult = await actualInsert.Insert(expectedTableName, newUserAccountCredentials).ConfigureAwait(false);
        Console.WriteLine(insertResult.ErrorMessage);
        Assert.IsTrue(insertResult.IsSuccessful);


        // Act
        var actual = new SelectDataAccess(connectionString);
        Result result = await actual.Select(expectedTableName, expectedColumns, values).ConfigureAwait(false);
        Result result2 = await actual.Select(expectedTableName, new List<string>{ "COUNT(username)"}, values).ConfigureAwait(false);

        // Assert
        if (result is not null)
        {
            if (result.Payload is not null)
            {
                List<List<object>> payload = (List<List<object>>)result.Payload;
                
                // existing account in database
                Assert.IsTrue((string)payload[0][0] == "Email@random.com");
                Assert.IsTrue((string)payload[0][1] == "coolkoala");
                Assert.IsTrue((int)payload[0][2] == 28);

                // new account in database
                Assert.IsTrue((string)payload[1][0] == "snackinglobsters@random.com");
                Assert.IsTrue((string)payload[1][1] == "sillysnail");
                Assert.IsTrue((int)payload[1][2] == 28);

            }
        }
        if (result2 is not null)
        {
            if (result2.Payload is not null)
            {
                List<List<object>> payload = (List<List<object>>)result2.Payload;

                foreach (var row in payload)
                {
                    Assert.IsTrue((int)row[0] == 2);
                }
            }
        }
        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.GetType() == expected);
    }
}

