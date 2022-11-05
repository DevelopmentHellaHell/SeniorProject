using System;

namespace DevelopmentHell.Hubba.Registration.Test;

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
    public void ShouldReturnCoorectIsValidValue()
    {
        //Arrange
        var expectedTrue = true;
        var expectedFalse = false;

        //Act
        var actual1 = new Result();
        var actual2 = new Result();
        actual1.IsValid = true;
        actual2.IsValid = false;

        //Assert
        Assert.IsNotNull(actual1);
        Assert.IsNotNull(actual2);
        Assert.IsTrue(actual1.IsValid == expectedTrue);
        Assert.IsTrue(actual2.IsValid == expectedFalse);
    }

    [TestMethod]
    public void ShouldReturnCorrectErrorMessage()
    {
        //Arrange
        var expected = "Error";

        //Act
        var actual1 = new Result();
        var actual2 = new Result();
        actual1.IsValid = true;
        actual2.IsValid = false;
        actual2.ErrorMessage = "Error";

        //Assert
        Assert.IsNull(actual1.ErrorMessage);
        Assert.IsNotNull(actual2.ErrorMessage);
    }

}