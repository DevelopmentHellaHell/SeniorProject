using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Registration.Test.IntegrationTests
{
    [TestClass]
    public class IntegrationTest
    {
        [TestMethod]

        public void createNewAccount()
        {
            // Arrange
            var expected = typeof(Result);
            var expectedEmail = "test@gmail.com";
            var expectedPassphrase = "HottestTest .";
            string jsonString =
@"{
  ""email"": ""test@email.com"",
  ""passphrase"": ""HottestTest .""
}
";

            // Act
            RegistrationManager manager = new RegistrationManager(jsonString);
            var actual = manager.createAccount();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.GetType() == expected);
            Assert.IsTrue(actual.IsSuccessful);
        }

    }
}
