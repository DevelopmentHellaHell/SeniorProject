﻿using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using System.Configuration;
using System.Diagnostics;


namespace DevelopmentHell.Hubba.Registration.Test.IntegrationTests
{
    [TestClass]
    public class IntegrationTest
    {
        [TestMethod]

        public async Task Reg01()
        {
            // Arrange
            var expected = typeof(Result);
            var stopwatch = new Stopwatch();
            var expectedTime = 5;
            string email = "TestEmail1@gmail.com";
            string jsonString =
            @"{
              ""Email"": ""TestEmail1@gmail.com"",
              ""Passphrase"": ""Test Case Reg-01"",
             ""BirthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager(jsonString);
            var actual = await manager.createAccount().ConfigureAwait(false);
            stopwatch.Stop();
            var actualTime = stopwatch.ElapsedMilliseconds / 1000;

            // Assert
            Console.WriteLine(actual.ErrorMessage);
            var AssertSAO = new SelectDataAccess(String.Format(@"Server={0};Database=DevelopmentHell.Hubba.Accounts;Integrated Security=True;Encrypt=False", ConfigurationManager.AppSettings["AccountServer"]));
            Result usernameResult = await AssertSAO.Select("Accounts", new List<string>() { "Username", "Id" }, new Dictionary<string, object> { { "Email", email } });
            if (usernameResult.Payload is null || actual.Payload is null)
            {
                Assert.IsTrue(false);
            }
            else
            {
                //Console.WriteLine("{0} : {1}", actual.Payload, (string)((List<List<object>>)usernameResult.Payload)[0][0] + ((List<List<object>>)usernameResult.Payload)[0][1]);
                Assert.IsNotNull(actual);
                Assert.IsTrue(actual.GetType() == expected);
                Assert.IsTrue(actual.IsSuccessful);
                Assert.IsTrue(actual.Payload.Equals((string)((List<List<object>>)usernameResult.Payload)[0][0] + ((List<List<object>>)usernameResult.Payload)[0][1]));
                Assert.IsTrue(actualTime <= expectedTime);
            }
        }

        [TestMethod]
        public async Task Reg02()
        {
            // Arrange
            var expected = typeof(Result);
            var stopwatch = new Stopwatch();
            var expectedTime = 5;
            string email = "TestEmail2@gmail.com";
            string jsonString =
            @"{
              ""Email"": ""TestEmail2@gmail.com"",
              ""Passphrase"": ""Test Case Reg-02"",
             ""BirthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager(jsonString);
            var actual = await manager.createAccount().ConfigureAwait(false);
            stopwatch.Stop();
            var actualTime = stopwatch.ElapsedMilliseconds / 1000;

            // Assert
            var AssertSAO = new SelectDataAccess(String.Format(@"Server={0};Database=DevelopmentHell.Hubba.Accounts;Integrated Security=True;Encrypt=False", ConfigurationManager.AppSettings["AccountServer"]));
            Result usernameResult = await AssertSAO.Select("Accounts", new List<string>() { "Username", "Id" }, new Dictionary<string, object> { { "Email", email } });
            if (usernameResult.Payload is null || actual.Payload is null)
            {
                Assert.IsTrue(false);
            }
            else
            {
                //Console.WriteLine("{0} : {1}", actual.Payload, (string)((List<List<object>>)usernameResult.Payload)[0][0] + ((List<List<object>>)usernameResult.Payload)[0][1]);
                Assert.IsNotNull(actual);
                Assert.IsTrue(actual.GetType() == expected);
                Assert.IsTrue(actual.IsSuccessful);
                Assert.IsTrue(actual.Payload.Equals((string)((List<List<object>>)usernameResult.Payload)[0][0] + ((List<List<object>>)usernameResult.Payload)[0][1]));
                Assert.IsTrue(actualTime <= expectedTime);
            }
        }


        [TestMethod]
        public async Task Reg03()
        {
            // Arrange
            var expected = typeof(Result);
            var stopwatch = new Stopwatch();
            var expectedTime = 5;
            Result expectedResult = new Result(false, "Email provided is invalid. Retry or contact admin.");
            string jsonString =
            @"{
              ""Email"": ""Test3.com"",
              ""Passphrase"": ""Test Case Reg-03"",
             ""BirthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager(jsonString);
            var actual = await manager.createAccount().ConfigureAwait(false);
            stopwatch.Stop();
            var actualTime = stopwatch.ElapsedMilliseconds / 1000;

            // Assert

            //Console.WriteLine("{0}, {1}", actual.IsSuccessful, actual.ErrorMessage);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.GetType() == expected);
            Assert.IsTrue(actual.IsSuccessful == expectedResult.IsSuccessful);
            if (actual.ErrorMessage is null)
            {
                Assert.IsTrue(false);
            }
            else
            {
                Assert.IsTrue(actual.ErrorMessage.Equals(expectedResult.ErrorMessage));
                Assert.IsTrue(actualTime <= expectedTime);
            }
        }

        [TestMethod]
        public async Task Reg04()
        {
            // Arrange
            var expected = typeof(Result);
            var stopwatch = new Stopwatch();
            var expectedTime = 5;
            Result expectedResult = new Result(false, "Email provided is invalid. Retry or contact admin.");
            string jsonString =
            @"{
              ""Email"": ""Test4@com"",
              ""Passphrase"": ""Test Case Reg-04"",
             ""BirthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager(jsonString);
            var actual = await manager.createAccount().ConfigureAwait(false);
            stopwatch.Stop();
            var actualTime = stopwatch.ElapsedMilliseconds / 1000;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.GetType() == expected);
            Assert.IsTrue(actual.IsSuccessful == expectedResult.IsSuccessful);
            if (actual.ErrorMessage is null)
            {
                Assert.IsTrue(false);
            }
            else
            {
                Assert.IsTrue(actual.ErrorMessage.Equals(expectedResult.ErrorMessage));
                Assert.IsTrue(actualTime <= expectedTime);
            }
        }

        [TestMethod]
        public async Task Reg05()
        {
            // Arrange
            var expected = typeof(Result);
            var stopwatch = new Stopwatch();
            var expectedTime = 5;
            Result expectedResult = new Result(false, "Email provided is invalid. Retry or contact admin.");
            string jsonString =
            @"{
              ""Email"": ""Test5@gmail."",
              ""Passphrase"": ""Test Case Reg-05"",
             ""BirthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager(jsonString);
            var actual = await manager.createAccount().ConfigureAwait(false);
            stopwatch.Stop();
            var actualTime = stopwatch.ElapsedMilliseconds / 1000;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.GetType() == expected);
            Assert.IsTrue(actual.IsSuccessful == expectedResult.IsSuccessful);
            if (actual.ErrorMessage is null)
            {
                Assert.IsTrue(false);
            }
            else
            {
                Assert.IsTrue(actual.ErrorMessage.Equals(expectedResult.ErrorMessage));
                Assert.IsTrue(actualTime <= expectedTime);
            }
        }

        [TestMethod]
        public async Task Reg06()
        {
            // Arrange
            var expected = typeof(Result);
            var stopwatch = new Stopwatch();
            var expectedTime = 5;
            Result expectedResult = new Result(false, "Email provided is invalid. Retry or contact admin.");
            string jsonString =
            @"{
              ""Email"": ""Test6@.com"",
              ""Passphrase"": ""Test Case Reg-06"",
             ""BirthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager(jsonString);
            var actual = await manager.createAccount().ConfigureAwait(false);
            stopwatch.Stop();
            var actualTime = stopwatch.ElapsedMilliseconds / 1000;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.GetType() == expected);
            Assert.IsTrue(actual.IsSuccessful == expectedResult.IsSuccessful);
            if (actual.ErrorMessage is null)
            {
                Assert.IsTrue(false);
            }
            else
            {
                Assert.IsTrue(actual.ErrorMessage.Equals(expectedResult.ErrorMessage));
                Assert.IsTrue(actualTime <= expectedTime);
            }
        }

        [TestMethod]
        public async Task Reg07()
        {
            // Arrange
            var expected = typeof(Result);
            var stopwatch = new Stopwatch();
            var expectedTime = 5;
            Result expectedResult = new Result(false, "Email provided already exists. Retry or contact admin.");
            string jsonString =
            @"{
              ""Email"": ""TestEmail1@gmail.com"",
              ""Passphrase"": ""Test Case Reg-07"",
             ""BirthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager(jsonString);
            var actual = await manager.createAccount().ConfigureAwait(false);
            stopwatch.Stop();
            var actualTime = stopwatch.ElapsedMilliseconds / 1000;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.GetType() == expected);
            Assert.IsTrue(actual.IsSuccessful == expectedResult.IsSuccessful);
            if (actual.ErrorMessage is null)
            {
                Assert.IsTrue(false);
            }
            else
            {
                Assert.IsTrue(actual.ErrorMessage.Equals(expectedResult.ErrorMessage));
                Assert.IsTrue(actualTime <= expectedTime);
            }
        }

        
        [TestMethod]
        public async Task Reg08()
        {
            // Arrange
            var expected = typeof(Result);
        var stopwatch = new Stopwatch();
            var expectedTime = 5;
            Result expectedResult = new Result(false, "Passphrase provided is invalid. Retry or contact admin.");
            string jsonString =
            @"{
              ""Email"": ""TestEmail8@gmail.com"",
              ""Passphrase"": ""Test Case Reg(08)"",
             ""BirthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager(jsonString);
            var actual = await manager.createAccount().ConfigureAwait(false);
            stopwatch.Stop();
            var actualTime = stopwatch.ElapsedMilliseconds / 1000;

            // Assert
            Console.WriteLine("{0} : {1}", actual.IsSuccessful, actual.ErrorMessage);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.GetType() == expected);
            Assert.IsTrue(actual.IsSuccessful == expectedResult.IsSuccessful);
            if (actual.ErrorMessage is null)
            {
                Assert.IsTrue(false);
            }
            else
            {
                Assert.IsTrue(actual.ErrorMessage.Equals(expectedResult.ErrorMessage));
                Assert.IsTrue(actualTime <= expectedTime);
            }
        }
        

        [TestMethod]
        public async Task Reg09()
        {
            // Arrange
            var expected = typeof(Result);
            var stopwatch = new Stopwatch();
            var expectedTime = 5;
            Result expectedResult = new Result(false, "Passphrase provided is invalid. Retry or contact admin.");
            string jsonString =
            @"{
              ""Email"": ""TestEmail9@gmail.com"",
              ""Passphrase"": ""2smol-9"",
             ""BirthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager(jsonString);
            var actual = await manager.createAccount().ConfigureAwait(false);
            stopwatch.Stop();
            var actualTime = stopwatch.ElapsedMilliseconds / 1000;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.GetType() == expected);
            Assert.IsTrue(actual.IsSuccessful == expectedResult.IsSuccessful);
            if (actual.ErrorMessage is null)
            {
                Assert.IsTrue(false);
            }
            else
            {
                Assert.IsTrue(actual.ErrorMessage.Equals(expectedResult.ErrorMessage));
                Assert.IsTrue(actualTime <= expectedTime);
            }
        }

        [TestMethod]

        public async Task Reg10()
        {
            // Arrange
            var expected = typeof(Result);
            var stopwatch = new Stopwatch();
            var expectedTime = 5;
            string email = "TestEmail10@gmail.com";
            string jsonString =
            @"{
              ""Email"": ""TestEmail10@gmail.com"",
              ""Passphrase"": ""Test Case Reg-10"",
             ""BirthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager(jsonString);
            var actual = await manager.createAccount().ConfigureAwait(false);
            Thread.Sleep(10000);
            stopwatch.Stop();
            var actualTime = stopwatch.ElapsedMilliseconds / 1000;

            // Assert
            //Console.WriteLine(actual.ErrorMessage);
            var AssertSAO = new SelectDataAccess(String.Format(@"Server={0};Database=DevelopmentHell.Hubba.Accounts;Integrated Security=True;Encrypt=False", ConfigurationManager.AppSettings["AccountServer"]));
            
            Result usernameResult = await AssertSAO.Select("Accounts", new List<string>() { "Username", "Id" }, new Dictionary<string, object> { { "Email", email } });
            if (usernameResult.Payload is null || actual.Payload is null)
            {
                Assert.IsTrue(false);
            }
            else
            {
                //Console.WriteLine("{0} : {1}", actual.Payload, (string)((List<List<object>>)usernameResult.Payload)[0][0] + ((List<List<object>>)usernameResult.Payload)[0][1]);
                Assert.IsNotNull(actual);
                Assert.IsTrue(actual.GetType() == expected);
                Assert.IsTrue(actual.IsSuccessful);
                Assert.IsTrue(actual.Payload.Equals((string)((List<List<object>>)usernameResult.Payload)[0][0] + ((List<List<object>>)usernameResult.Payload)[0][1]));
                Assert.IsTrue(actualTime >= expectedTime);
            }
        }


        [TestMethod]
        public async Task Reg11()
        {
            // Arrange
            var expected = typeof(Result);
            var stopwatch = new Stopwatch();
            var expectedTime = 5;
            Result expectedResult = new Result(false, "Age requirement not reached.");
            string jsonString =
            @"{
              ""Email"": ""TestEmail11@gmail.com"",
              ""Passphrase"": ""Test Case Reg-11"",
             ""BirthDate"": ""2015-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager(jsonString);
            var actual = await manager.createAccount().ConfigureAwait(false);
            stopwatch.Stop();
            var actualTime = stopwatch.ElapsedMilliseconds / 1000;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.GetType() == expected);
            Assert.IsTrue(actual.IsSuccessful == expectedResult.IsSuccessful);
            if (actual.ErrorMessage is null)
            {
                Assert.IsTrue(false);
            }
            else
            {
                Assert.IsTrue(actual.ErrorMessage.Equals(expectedResult.ErrorMessage));
                Assert.IsTrue(actualTime <= expectedTime);
            }
        }

        
    }
}
