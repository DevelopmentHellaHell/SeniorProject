using DevelopmentHell.Hubba.Models;


namespace DevelopmentHell.Hubba.Registration.Test;

[TestClass]
public class HashComputerUnitTest
{
    [TestMethod]
    public void ShouldCreateInstanceWithCtor()
    {
        // Arrange
        var expected = typeof(IHash);

        // Act
        var actual = new IHash();

        // Assert
        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.GetType() == expected);
    }

    [TestMethod]
    public void ShouldCreateInstanceWithParameterCtor()
    {
        // Arrange
        var expected = typeof(IHash);
        var expectedText = "Pet !234";

        // Act
        var actual = new IHash(expectedText);

        // Assert
        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.GetType() == expected);
    }

    [TestMethod]
    public void ShouldReturnAHashResult()
    {
        //Arrange
        var expected = new Result();
        expected.IsSuccessful = true;
        expected.Payload = "idryoo9nIPMvcjfHexhcFO7UxpDPQmwcE2dwtZkIf5Y=";

        //Act
        var actual = new IHash("Pet !234").ComputeHash();

        //Assert
        Assert.IsNotNull(actual);
        Assert.AreEqual(actual, expected);
    }

}