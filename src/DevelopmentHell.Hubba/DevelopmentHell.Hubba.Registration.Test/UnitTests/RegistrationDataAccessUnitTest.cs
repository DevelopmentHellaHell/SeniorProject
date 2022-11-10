using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using DevelopmentHell.Hubba.Models;
using System.Configuration;

namespace DevelopmentHell.Hubba.Registration.Test
{
    [TestClass]
    public class RegistrationSqlDataAccessUnitTest
    {
		private static string expectedDatabaseName = "DevelopmentHell.Hubba.Accounts";
		private static string connectionString = String.Format(@"Server={0};Database={1};Encrypt=false;User Id=DevelopmentHell.Hubba.SqlUser.Registration;Password=password", ConfigurationManager.AppSettings["AccountServer"], expectedDatabaseName);

		[TestMethod]
        public void ShouldCreateNewInstanceWithParameterCtor()
        {
            // Arrange
            var expected = typeof(InsertDataAccess);

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
           
            string username = "coolkoala";
            string email = @"Email@random.com";
            string passphrase = "c0o1p4s5phra53";
            DateTime birthDate = new DateTime(2000, 01, 01);
            int id = 5;
            DateTime lastInteraction = DateTime.Now;
            bool adminAccount = false;
            Dictionary<string, object> newUserAccountCredentials = new()
            {
                { "Username", username },
                { "Email", email },
                { "PassphraseHash", "ShouldRegisterNewAccountIntoDatabaseHASH"},
                { "PassphraseSalt", "ShouldRegisterNewAccountIntoDatabaseSALT" },
                { "LastLogin", lastInteraction },
                { "Id", id},
                { "BirthDate", birthDate },
                { "AdminAccount", adminAccount }
            };

            // Act
            var actual = new InsertDataAccess(connectionString);
            Result result = await actual.Insert(expectedTableName, newUserAccountCredentials).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.GetType() == expected);
            Console.WriteLine(result.ErrorMessage);
            Assert.IsTrue(result.IsSuccessful);
        }

        [TestMethod]
        public async Task SouldUpdateAccountInDatabase()
        {
            // TODO: fill out test case
            // Arrange
            var expected = typeof(UpdateDataAccess);
            var expectedTableName = "Accounts";
            
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
            
            Dictionary<string, object> values = new()
            {
                { "age", 28 }
            };
            List<string> expectedColumns = new()
            {
                "Email",
                "Username",
                "BirthDate"
            };
            string username = "sillysnail";
            string email = @"snackinglobsters@random.com";
            string passphrase = "c0o1p4s5phra53";
            DateTime lastLogin = DateTime.Now;
            DateTime birthDate = new DateTime(2000, 01, 01);
            bool adminAccount = false;
            Dictionary<string, object> newUserAccountCredentials = new()
            {
                { "Id", 99 },
                { "Username", username },
                { "Email", email },
                { "PassphraseHash", "ShouldAccessExistingAccountInDatabaseHASH"},
                { "PassphraseSalt", "ShouldAccessExistingAccountInDatabaseSALT" },
                { "LastLogin", lastLogin },
                { "AdminAccount", adminAccount },
                { "BirthDate", birthDate }

            };
            var actualInsert = new InsertDataAccess(connectionString);
            var insertResult = await actualInsert.Insert(expectedTableName, newUserAccountCredentials).ConfigureAwait(false);
            Console.WriteLine(insertResult.ErrorMessage);
            Assert.IsTrue(insertResult.IsSuccessful);


            // Act
            var actual = new SelectDataAccess(connectionString);
            Result result = await actual.Select(expectedTableName, expectedColumns, values).ConfigureAwait(false);
            Result result2 = await actual.Select(expectedTableName, new List<string> { "COUNT(username)" }, values).ConfigureAwait(false);

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

}
