using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using System.Diagnostics;
using System.Configuration;
using DevelopmentHell.Hubba.Models;


namespace DevelopmentHell.Hubba.Registration.Test.IntegrationTests
{
	[TestClass]
    public class IntegrationTest
    {
		private static string expectedDatabaseName = "DevelopmentHell.Hubba.Accounts";
		private static string connectionString = String.Format(@"Server={0};Database={1};Encrypt=false;User Id=DevelopmentHell.Hubba.SqlUser.Registration;Password=password", ConfigurationManager.AppSettings["AccountServer"], expectedDatabaseName);

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
              ""PassphraseHash"": ""Test Case Reg-01"",
             ""BirthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager();
            var actual = await manager.createAccount(jsonString).ConfigureAwait(false);
            stopwatch.Stop();
            var actualTime = stopwatch.ElapsedMilliseconds / 1000;

            // Assert
            Console.WriteLine(actual.ErrorMessage);
            var AssertSAO = new SelectDataAccess(connectionString);
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
              ""PassphraseHash"": ""Test Case Reg-02"",
             ""BirthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager();
            var actual = await manager.createAccount(jsonString).ConfigureAwait(false);
            stopwatch.Stop();
            var actualTime = stopwatch.ElapsedMilliseconds / 1000;

            // Assert
            var AssertSAO = new SelectDataAccess(connectionString);
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
              ""PassphraseHash"": ""Test Case Reg-03"",
             ""BirthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager();
            var actual = await manager.createAccount(jsonString).ConfigureAwait(false);
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
              ""PassphraseHash"": ""Test Case Reg-04"",
             ""BirthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager();
            var actual = await manager.createAccount(jsonString).ConfigureAwait(false);
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
              ""PassphraseHash"": ""Test Case Reg-05"",
             ""BirthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager();
            var actual = await manager.createAccount(jsonString).ConfigureAwait(false);
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
              ""PassphraseHash"": ""Test Case Reg-06"",
             ""BirthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager();
            var actual = await manager.createAccount(jsonString).ConfigureAwait(false);
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
              ""PassphraseHash"": ""Test Case Reg-07"",
             ""BirthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager();
            var actual = await manager.createAccount(jsonString).ConfigureAwait(false);
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
              ""PassphraseHash"": ""Test Case Reg(08)"",
             ""BirthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager();
            var actual = await manager.createAccount(jsonString).ConfigureAwait(false);
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
            RegistrationManager manager = new RegistrationManager();
            var actual = await manager.createAccount(jsonString).ConfigureAwait(false);
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
              ""PassphraseHash"": ""Test Case Reg-10"",
             ""BirthDate"": ""2001-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager();
            var actual = await manager.createAccount(jsonString).ConfigureAwait(false);
            await Task.Delay(10000);
            stopwatch.Stop();
            var actualTime = stopwatch.ElapsedMilliseconds / 1000;

            // Assert
            Console.WriteLine(actual.ErrorMessage);
            var AssertSAO = new SelectDataAccess(connectionString);
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
            Result expectedResult = new Result(false, "Age requirement not met.");
            string jsonString =
            @"{
              ""Email"": ""TestEmail11@gmail.com"",
              ""PassphraseHash"": ""Test Case Reg-11"",
             ""BirthDate"": ""2015-01-01""
            }";

            // Act
            stopwatch.Start();
            RegistrationManager manager = new RegistrationManager();
            var actual = await manager.createAccount(jsonString).ConfigureAwait(false);
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
