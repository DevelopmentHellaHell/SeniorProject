namespace DevelopmentHell.Hubba.Registration.Test;

using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Implementation;

[TestClass]
public class PassphraseValidationUnitTest
{
    [TestMethod]
    public void ShouldCreateInstanceWithCtor()
    {
        // Arrange
        var expected = typeof(PassphraseValidation);

        // Act
        var actual = new PassphraseValidation();

        // Assert
        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.GetType() == expected);
    }

    [TestMethod]
    public void ShouldCheckForInvalidPassphrase()
    {
        //TODO: check invalid passphrase
        // Arrange
        List<String> badPassphrases = new List<String>();
        badPassphrases.Add("Test Case Reg(08)"); // invalid special char
        badPassphrases.Add("2smol-9"); // length < 8
        badPassphrases.Add("joe.123@"); // no uppercase letter
        badPassphrases.Add("JOE.123!"); // no lowercase letter
        badPassphrases.Add("!@#$%^&*"); // no letter, number
        badPassphrases.Add("joe!@#$%"); // no number
        badPassphrases.Add("Joe!@#$%"); // no number
        badPassphrases.Add("1234%^&*"); // no letter

        foreach (String badPassphrase in badPassphrases)
        {
            var expected = new Result();
            expected.IsSuccessful = false;
            //Act
            var inputValidation = new Models.InputValidation();
            var actual = inputValidation.ValidatePassphrase(badPassphrase);
            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
        }
    }
}
//References: