using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
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
              ""email"": ""TestEmail1@gmail.com"",
              ""passphrase"": ""Test Case Reg-01"",
             ""birthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager(jsonString);
            var actual = await manager.createAccount().ConfigureAwait(false);
            stopwatch.Stop();
            var actualTime = stopwatch.ElapsedMilliseconds / 1000;

            // Assert
            Console.WriteLine(actual.ErrorMessage);
            var AssertSAO = new SelectDataAccess(@"Server=localhost\SQLEXPRESS;Database=DevelopmentHell.Hubba.Accounts;Integrated Security=True;Encrypt=False");
            Result usernameResult = await AssertSAO.Select("Accounts", new List<string>() { "username", "id" }, new Dictionary<string, object> { { "email", email } });
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
              ""email"": ""TestEmail2@gmail.com"",
              ""passphrase"": ""Test Case Reg-02"",
             ""birthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager(jsonString);
            var actual = await manager.createAccount().ConfigureAwait(false);
            stopwatch.Stop();
            var actualTime = stopwatch.ElapsedMilliseconds / 1000;

            // Assert
            var AssertSAO = new SelectDataAccess(@"Server=localhost\SQLEXPRESS;Database=DevelopmentHell.Hubba.Accounts;Integrated Security=True;Encrypt=False");
            Result usernameResult = await AssertSAO.Select("Accounts", new List<string>() { "username", "id" }, new Dictionary<string, object> { { "email", email } });
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
              ""email"": ""Test3.com"",
              ""passphrase"": ""Test Case Reg-03"",
             ""birthDate"": ""2001-01-01""
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
              ""email"": ""Test4@com"",
              ""passphrase"": ""Test Case Reg-04"",
             ""birthDate"": ""2001-01-01""
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
              ""email"": ""Test5@gmail."",
              ""passphrase"": ""Test Case Reg-05"",
             ""birthDate"": ""2001-01-01""
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
              ""email"": ""Test6@.com"",
              ""passphrase"": ""Test Case Reg-06"",
             ""birthDate"": ""2001-01-01""
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
              ""email"": ""TestEmail1@gmail.com"",
              ""passphrase"": ""Test Case Reg-07"",
             ""birthDate"": ""2001-01-01""
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

        /*
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
              ""email"": ""TestEmail8@gmail.com"",
              ""passphrase"": ""Test Case Reg(08)"",
             ""birthDate"": ""2001-01-01""
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
            }
        }
        */

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
              ""email"": ""TestEmail9@gmail.com"",
              ""passphrase"": ""2smol-9"",
             ""birthDate"": ""2001-01-01""
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
              ""email"": ""TestEmail10@gmail.com"",
              ""passphrase"": ""Test Case Reg-10"",
             ""birthDate"": ""2001-01-01""
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
            var AssertSAO = new SelectDataAccess(@"Server=localhost\SQLEXPRESS;Database=DevelopmentHell.Hubba.Accounts;Integrated Security=True;Encrypt=False");
            Result usernameResult = await AssertSAO.Select("Accounts", new List<string>() { "username", "id" }, new Dictionary<string, object> { { "email", email } });
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
              ""email"": ""TestEmail11@gmail.com"",
              ""passphrase"": ""Test Case Reg-11"",
             ""birthDate"": ""2015-01-01""
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
