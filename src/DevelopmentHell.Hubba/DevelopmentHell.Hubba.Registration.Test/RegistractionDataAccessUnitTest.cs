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

        public void ShouldRegisterNewAccountIntoDatabase()
        {
            // TODO: fill out test case
            // Arrange

            // Act

            // Assert
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
