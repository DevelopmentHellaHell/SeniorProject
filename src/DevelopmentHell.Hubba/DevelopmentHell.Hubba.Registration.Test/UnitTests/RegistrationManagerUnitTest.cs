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
        var actual = new RegistrationManager(expectedJsonString);

        // Assert
        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.GetType() == expected);
    }


    [TestMethod]
    public void ShouldCreateNewAccountWithCreateAccount()
    {
        //TODO: myRegistration.CreateAccount();
        // Actual

        // Act

        // Assert
    }
}
//References: