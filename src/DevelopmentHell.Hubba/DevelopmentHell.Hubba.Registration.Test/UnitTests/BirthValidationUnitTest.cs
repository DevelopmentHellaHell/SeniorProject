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

}
//References: