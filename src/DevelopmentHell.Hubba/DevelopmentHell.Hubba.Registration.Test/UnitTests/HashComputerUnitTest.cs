using DevelopmentHell.Hubba.Models;


namespace DevelopmentHell.Hubba.Registration.Test;

[TestClass]
public class HashComputerUnitTest
{
    [TestMethod]
    public void ShouldCreateInstanceWithCtor()
    {
        // Arrange
        var expected = typeof(Result);

        // Act
        var actual = new Result();

        // Assert
        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.GetType() == expected);
    }

    [TestMethod]
    public void ShouldReturnCoorectIsSuccessfulValue()
    {
        //Arrange

        //Act

        //Assert
       
    }

    [TestMethod]
    public void ShouldReturnAHash()
    {
        //Arrange

        //Act

        //Assert
    }

}