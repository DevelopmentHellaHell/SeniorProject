using Microsoft.Identity.Client;

namespace DevelopmentHell.Hubba.Registration.Test;

[TestClass]
public class RegistrationManagerUnitTest
{
    [TestMethod]
    public void ShouldCreateInstanceWithParameterCtor()
    {
        // Arrange
        var expected = typeof(RegistrationManager);
        var expectedJsonString = "";
        // Act
        var actualManager = new RegistrationManager();
        var actual = actualManager.createAccount(expectedJsonString);

        // Assert
        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.GetType() == expected);
    }
}
//References: