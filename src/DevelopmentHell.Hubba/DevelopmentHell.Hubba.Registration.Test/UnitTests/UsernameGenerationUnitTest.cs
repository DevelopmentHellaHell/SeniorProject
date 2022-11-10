
using DevelopmentHell.Hubba.Registration.Implementation;

namespace DevelopmentHell.Hubba.Registration.Test;

[TestClass]
public class UsernameGenerationUnitTest
{
    [TestMethod]
    public void ShouldCreateInstanceWithCtor()
    {
        // Arrange
        var expected = typeof(UsernameGeneration);

        // Act
        var actual = new UsernameGeneration();

        // Assert
        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.GetType() == expected);
    }

    [TestMethod]
    public void ShouldReturnRandomUsername()
    {
        //Arrange
        var expected = new UsernameGeneration().generateUsername();

        //Act
        var actual = new UsernameGeneration().generateUsername();

        //Assert
        Assert.IsNotNull(actual);
        Assert.IsFalse(actual == expected);
    }

}