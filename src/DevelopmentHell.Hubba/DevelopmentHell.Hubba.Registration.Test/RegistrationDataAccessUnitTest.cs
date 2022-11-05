using DevelopmentHell.Hubba.SqlDataAccess;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using System.Data;

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
            Dictionary<string, object> newUserAccountCredentials = new()
            {
                { "username", username },
                { "email", email },
                { "passphrase", passphrase },
                { "last_interaction", last_interaction },
                { "admin_account", admin_account }
            };

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
            Tuple<string, object> key = new Tuple<string, object>("email", email);
            Dictionary<string, object> values = new()
            {
                { "age", 28 }
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
        [TestMethod]
        public void ShouldAccessExistingAccountInDatabase()
        {
            // TODO: fill out test case
            // Arrange
            var expected = typeof(SelectDataAccess);
            var expectedTableName = "Accounts";
            var expectedDatabaseName = "DevelopmentHell.Hubba.Accounts";
            var connectionString = String.Format(@"Server=localhost\SQLEXPRESS;Database={0};Integrated Security=True;Encrypt=False", expectedDatabaseName);
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
            string username = "coolkoala";
            string email = @"Email@random.com";
            string passphrase = "c0o1p4s5phra53";
            DateTime last_interaction = DateTime.Now;
            bool admin_account = false;
            Dictionary<string, object> newUserAccountCredentials = new()
            {
                { "username", username },
                { "email", email },
                { "passphrase", passphrase },
                { "last_interaction", last_interaction },
                { "admin_account", admin_account }
            };
            var actualInsert = new InsertDataAccess(connectionString);
            actualInsert.Insert(expectedTableName, newUserAccountCredentials);


            // Act
            var actual = new SelectDataAccess(connectionString);
            DevelopmentHell.Hubba.SqlDataAccess.Result result = actual.Select(expectedTableName, expectedColumns, values);
            result = actual.Select(expectedTableName, new List<string>{ "COUNT(username)"}, values);

            // Assert
            if (result is not null)
            {
                if (result.Payload is not null)
                {
                    List<List<object>> payload = (List<List<object>>)result.Payload;
                    
                    foreach(var row in payload)
                    {
                        Assert.IsTrue((string)row[0] == "Email@random.com");
                        Assert.IsTrue((string)row[1] == "coolkoala");
                        Assert.IsTrue((int)row[2] == 28);
                    }
                }
            }
            if (result is not null)
            {
                if (result.Payload is not null)
                {
                    List<List<object>> payload = (List<List<object>>)result.Payload;

                    foreach (var row in payload)
                    {
                        Assert.IsTrue((int)row[0] == 1);
                    }
                }
            }
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.GetType() == expected);
        }
    }

}
