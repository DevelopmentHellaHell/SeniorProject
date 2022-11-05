namespace DevelopmentHell.Hubba.Registration.Test;
using DevelopmentHell.Hubba.Models;


[TestClass]
public class InputValidationUnitTest
{
    [TestMethod]
    public void ShouldCreateInstanceWithCtor()
    {
        // Arrange
        var expected = typeof(InputValidation);

        // Act
        var actual = new InputValidation();

        // Assert
        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.GetType() == expected);
    }

    [TestMethod]
    public void ShouldCheckForValidEmail()
    {
        // Arrange
        List<String> goodEmails = new List<String>();
        goodEmails.Add("joe@home.org");
        goodEmails.Add("joe@joebob.name");
        goodEmails.Add("joe@home.com");
        goodEmails.Add("joe.bob@home.co");
        goodEmails.Add("joe_bob@home.com");
        goodEmails.Add("joe@his.home.place");
        goodEmails.Add("a@abc.org");
        goodEmails.Add("a@abc-xyz.org");
        goodEmails.Add("a@192.168.0.1");
        goodEmails.Add("a@10.1.100.1");

        foreach (String goodEmail in goodEmails)
        {
            var expected = new Result();
            expected.IsSuccessful = true;
        //Act
            var inputValidation = new InputValidation();
            var actual = inputValidation.ValidateEmail(goodEmail);
        //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
        }
    }

    [TestMethod]
    public void ShouldCheckForInvalidEmail()
    {
        // Arrange
        List<String> badEmails = new List<String>();
        badEmails.Add("joe"); 
        badEmails.Add("joe@home"); 
        badEmails.Add("j @@home"); 

        foreach (String badEmail in badEmails)
        {
            var expected = new Result();
            expected.IsSuccessful = false;
            //Act
            var inputValidation = new InputValidation();
            var actual = inputValidation.ValidateEmail(badEmail);
            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
        }
    }

    [TestMethod]
    public void ShouldCheckForInvalidPassphrase()
    {
        //TODO: check invalid passphrase
        // Arrange
        List<String> badPassphrases = new List<String>();
        badPassphrases.Add("joe"); // length < 8
        badPassphrases.Add("joe12345"); // no special char
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
            var inputValidation = new InputValidation();
            var actual = inputValidation.ValidatePassphrase(badPassphrase);
            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
        }
    }
}
//References:
// Email test cases: https://www.rhyous.com/2010/06/15/csharp-email-regular-expression/