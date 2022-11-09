
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
        var acc = new Account { Email = "jo@gmail.com", PassphraseHash = "testing password", Username = "test username" };
        Dictionary<string, object> expected = new()
        {
            { "Email", "jo@gmail.com"},
            { "PassphraseHash", "testing password"},
            { "Username", "test username"},
            { "AdminAccount", false},
            { "BirthDate", new DateTime()},
        };

        //Act
        var actual = DictionaryConversion.ObjectToDictionary(acc);

        //Assert
        Assert.IsNotNull(actual);

        foreach (var item in actual)
        {
            Assert.IsTrue(actual.Keys.Contains(item.Key));
            Assert.IsTrue(actual[item.Key] == item.Value);
        }
    }

}