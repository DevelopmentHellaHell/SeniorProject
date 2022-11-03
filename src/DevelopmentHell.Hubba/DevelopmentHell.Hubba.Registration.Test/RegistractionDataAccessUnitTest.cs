using DevelopmentHell.Hubba.SqlDataAccess;

namespace DevelopmentHell.Hubba.Registration.Test
{
    [TestClass]
    public class RegistrationSqlDataAccessUnitTest
    {
        [TestMethod]
        public void ShouldCreateNewInstanceWithDefaultCtor()
        {
            // Arrange
            var expected = typeof(RegistrationDataAccess);

            // Act
            var actual = new RegistrationDataAccess();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.GetType() == expected);
        }
        [TestMethod]
        public void ShouldCreateNewInstanceWithParameterCtor()
        {
            // Arrange
            var expected = typeof(RegistrationDataAccess);
            var expectedDatabaseName = "DevelopmentHell.Hubba.Accounts";
            var expectedTableName = "Accounts";

            // Act
            var actual = new RegistrationDataAccess(expectedDatabaseName, expectedTableName);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.GetType() == expected);
        }
        [TestMethod]
        public void ShouldRegisterNewAccountIntoDatabase()
        {
            // TODO: fill out test case
            // Arrange
            var expected = typeof(RegistrationDataAccess);
            var expectedTableName = "Accounts";
            var expectedDatabaseName = "DevelopmentHell.Hubba.Accounts";
            string email = @"thirdEmail@random.com";
            string passphrase = "c0o1p4s5phra53";
            Dictionary<string, string> newUserAccountCredentials = new Dictionary<string, string>();
            newUserAccountCredentials.Add("email", email);
            newUserAccountCredentials.Add("passphrase", passphrase);

            // Act
            var actual = new RegistrationDataAccess(expectedDatabaseName, expectedTableName);
            DevelopmentHell.Hubba.SqlDataAccess.Result result = actual.InsertNewAccount(newUserAccountCredentials);
            //int account_id = (int)(result.Payload);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.GetType() == expected);
            //Assert.IsTrue((string)(actual.AccessEmail(account_id).Payload) == email);
            //Assert.IsTrue((string)(actual.AccessPassphrase(account_id).Payload) == passphrase);
        }
        [TestMethod]
        public void ShouldUpdateAccountInDatabase()
        {
            // TODO: fill out test case
            // Arrange
            var expected = typeof(RegistrationDataAccess);
            var expectedTableName = "Accounts";
            var expectedDatabaseName = "DevelopmentHell.Hubba.Accounts";
            string email = @"secondEmail@random.com";
            string username = "secondbestusername" +
                "";

            // Act
            var actual = new RegistrationDataAccess(expectedDatabaseName,expectedTableName);
            DevelopmentHell.Hubba.SqlDataAccess.Result result = actual.UpdateAccountUsername(email, username);
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
