namespace DevelopmentHell.Hubba.Registration.Test.UnitTests;

using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Implementation;

[TestClass]
public class EmailValidationUnitTest
{
    [TestMethod]
    public void ShouldCreateInstanceWithCtor()
    {
        // Arrange
        var expected = typeof(EmailValidation);

        // Act
        var actual = new EmailValidation();

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
            var expected = new Result(true);
            expected.IsSuccessful = true;

        //Act
            var actual = EmailValidation.validate(goodEmail);

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
            var expected = new Result(false, "Email provided is invalid. Retry or contact admin.");

            //Act
            var actual = EmailValidation.validate(badEmail);
            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
            Assert.IsTrue(actual.ErrorMessage == expected.ErrorMessage);
        }
    }
}
//References:
// Email test cases: https://www.rhyous.com/2010/06/15/csharp-email-regular-expression/