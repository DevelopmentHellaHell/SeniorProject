using DevelopmentHell.Hubba.SqlDataAccess;

namespace DevelopmentHell.Hubba.Registration.Test
{
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
            var actual = new UpdateDataAccess(connectionString);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.GetType() == expected);
        }
        [TestMethod]
        public void ShouldRegisterNewAccountIntoDatabase()
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
            DateTime last_interaction = DateTime.Now;
            bool admin_account = false;
            Dictionary<string, object> newUserAccountCredentials = new Dictionary<string, object>();
            newUserAccountCredentials.Add("username", username);
            newUserAccountCredentials.Add("email", email);
            newUserAccountCredentials.Add("passphrase", passphrase);
            newUserAccountCredentials.Add("last_interaction", last_interaction);
            newUserAccountCredentials.Add("admin_account", admin_account);

            // Act
            var actual = new InsertDataAccess(connectionString);
            DevelopmentHell.Hubba.SqlDataAccess.Result result = actual.Insert(expectedTableName, newUserAccountCredentials);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.GetType() == expected);
        }

        [TestMethod]
        public void ShouldUpdateAccountInDatabase()
        {
            // TODO: fill out test case
            // Arrange
            var expected = typeof(UpdateDataAccess);
            var expectedTableName = "Accounts";
            var expectedDatabaseName = "DevelopmentHell.Hubba.Accounts";
            var connectionString = String.Format(@"Server=localhost\SQLEXPRESS;Database={0};Integrated Security=True;Encrypt=False", expectedDatabaseName);
            string email = @"Email@random.com";
            int age = 28;
            Tuple<string, object> key = new Tuple<string, object>("email", email);
            Dictionary<string, object> values = new()
            {
                { "age", age }
            };

            // Act
            var actual = new UpdateDataAccess(connectionString);
            DevelopmentHell.Hubba.SqlDataAccess.Result result = actual.Update(expectedTableName, key, values);
            //int account_id = (int)(result.Payload);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.GetType() == expected);
            //Assert.IsTrue((string)(actual.AccessEmail(account_id).Payload) == email);
            //Assert.IsTrue((string)(actual.AccessPassphrase(account_id).Payload) == passphrase);
        }
        public void ShouldAccessExistingAccountInDatabase()
        {
            // TODO: fill out test case
            // Arrange

            // Act

            // Assert
        }
    }

}
