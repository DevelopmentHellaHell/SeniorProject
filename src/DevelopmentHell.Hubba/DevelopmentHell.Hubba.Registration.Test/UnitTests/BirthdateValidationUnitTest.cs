using DevelopmentHell.Hubba.Registration.Implementation;

namespace DevelopmentHell.Hubba.Registration.Test;

[TestClass]
public class BirthdateValidationUnitTest
{
    [TestMethod]
    public void ShouldCreateInstanceWithCtor()
    {
        // Arrange
        var expected = typeof(BirthdateValidation);

        // Act
        var actual = new BirthdateValidation();

        // Assert
        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.GetType() == expected);
    }

    [TestMethod]
    public void ShouldReturnCorrectResultForBadBirthdate()
    {
        //Arrange
        var expected = new Result();
        expected.IsSuccessful = false;
        expected.ErrorMessage = "Age requirement not reached.";
        var expectedBD = new DateTime(2018, 1, 1);

        //Act
        var actual = BirthdateValidation.validate(expectedBD);

        //Assert
        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
        Assert.IsTrue(actual.ErrorMessage == expected.ErrorMessage);
    }
    [TestMethod]
    public void ShouldReturnCorrectResultForGoodBirthdate()
    {
        //Arrange
        var expected = new Result();
        expected.IsSuccessful = true;
        var expectedBD = new DateTime(1985, 12, 1);

        //Act
        var actual = BirthdateValidation.validate(expectedBD);

        //Assert
        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
        Assert.IsTrue(actual.ErrorMessage == expected.ErrorMessage);
    }
}
//References: