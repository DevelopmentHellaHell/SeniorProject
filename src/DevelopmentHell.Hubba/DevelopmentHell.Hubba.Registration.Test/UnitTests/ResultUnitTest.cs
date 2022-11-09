using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Registration.Test.UnitTests;

[TestClass]
public class ResultUnitTest
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
        var expectedTrue = true;
        var expectedFalse = false;

        //Act
        var actual1 = new Result();
        var actual2 = new Result();
        actual1.IsSuccessful = true;
        actual2.IsSuccessful = false;

        //Assert
        Assert.IsNotNull(actual1);
        Assert.IsNotNull(actual2);
        Assert.IsTrue(actual1.IsSuccessful == expectedTrue);
        Assert.IsTrue(actual2.IsSuccessful == expectedFalse);
    }

    [TestMethod]
    public void ShouldReturnCorrectErrorMessage()
    {
        //Arrange
        string expected = "Error";

        //Act
        var actual1 = new Result();
        var actual2 = new Result();
        actual1.IsSuccessful = true;
        actual2.IsSuccessful = false;
        actual2.ErrorMessage = "Error";

        //Assert
        Assert.IsNotNull(actual1.ErrorMessage);
        Assert.IsNotNull(actual2.ErrorMessage);
        Assert.AreEqual(expected, actual2.ErrorMessage);
    }

}