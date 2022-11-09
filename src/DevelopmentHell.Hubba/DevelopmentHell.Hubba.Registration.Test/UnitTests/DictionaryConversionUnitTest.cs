
using DevelopmentHell.Hubba.Registration.Implementation;

namespace DevelopmentHell.Hubba.Registration.Test;

[TestClass]
public class DictionaryConversionUnitTest
{
    [TestMethod]
    public void ShouldCreateInstanceWithCtor()
    {
        // Arrange
        var expected = typeof(DictionaryConversion);

        // Act
        var actual = new DictionaryConversion();

        // Assert
        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.GetType() == expected);
    }

    [TestMethod]
    public void ShouldReturnACorrectConversion()
    {
        //Arrange
        var acc = new Account { Email = "jo@gmail.com" };
        Dictionary<string, object> expected = new()
        {
            { "Id", 0},
            { "Email", "jo@gmail.com"},
            { "PassphraseHash", ""},
            { "PassphraseSalt", ""},
            { "Username", ""},
            { "AdminAccount", false},
            { "BirthDate", new DateTime()},
        };

        //Act
        var actual = DictionaryConversion.ObjectToDictionary(acc);

        //Assert
        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.Equals(expected));
    }

}