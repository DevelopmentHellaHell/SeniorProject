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
            var expectedTableName = "Accounts";

            // Act
            var actual = new RegistrationDataAccess(expectedTableName);

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
            string email = @"secondEmail@random.com";
            string passphrase = "c0o1p4s5phra53";
            List<object> values = new List<object>();
            values.Add(email);
            values.Add(passphrase);

            // Act
            var actual = new RegistrationDataAccess(expectedTableName);
            DevelopmentHell.Hubba.SqlDataAccess.Result result = actual.InsertNewAccount("DevelopmentHell.Hubba.Accounts", "accounts", values);
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
            string email = @"randomEmail@random.com";
            string username = "SuperAmazingAwesomeUsername";

            // Act
            var actual = new RegistrationDataAccess(expectedTableName);
            DevelopmentHell.Hubba.SqlDataAccess.Result result = actual.UpdateAccountUsername("DevelopmentHell.Hubba.Accounts", "accounts", email, username);
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
