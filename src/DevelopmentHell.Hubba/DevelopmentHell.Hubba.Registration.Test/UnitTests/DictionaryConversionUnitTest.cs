
using DevelopmentHell.Hubba.Registration.Implementation;

namespace DevelopmentHell.Hubba.Registration.Test;

[TestClass]
public class DictionaryConversionUnitTest
{
    [TestMethod]
    public void ShouldCreateInstanceWithCtor()
    {
        // Arrange
        var expected = typeof(DictonaryConversion);

        // Act
        var actual = new DictonaryConversion();

        // Assert
        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.GetType() == expected);
    }

    [TestMethod]
    public void ShouldReturnACorrectConvertion()
    {
        //Arrange

        //Act

        //Assert
    }

}