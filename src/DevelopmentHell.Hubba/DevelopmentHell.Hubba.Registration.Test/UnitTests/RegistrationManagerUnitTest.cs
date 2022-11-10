using Microsoft.Identity.Client;
using DevelopmentHell.Hubba.Models;
namespace DevelopmentHell.Hubba.Registration.Test;

[TestClass]
public class RegistrationManagerUnitTest
{
    [TestMethod]
    public async Task ShouldCreateInstanceWithParameterCtor()
    {
        // Arrange
        var expected = typeof(Result);
        var expectedJsonString =
        @"{
            ""Email"": ""TestEmail100@gmail.com"",
            ""PassphraseHash"": ""ShouldCreateInstanceWithParameterCtor"",
            ""BirthDate"": ""2001-01-01""
        }";
        // Act
        var actualManager = new RegistrationManager();
        var actual = await actualManager.createAccount(expectedJsonString);

        // Assert
        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.GetType() == expected);
    }
}
//References: