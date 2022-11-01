namespace DevelopmentHell.Hubba.Registration.Test;

[TestClass]
public class RegistrationManagerUnitTest
{
    [TestMethod]
    public void ShouldCreateInstanceWithCtor()
    {
        // Arrange
        var expected = typeof(RegistrationManager);

        // Act
        var actual = new RegistrationManager();

        // Assert
        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.GetType() == expected);
    }

    [TestMethod]
    public void ShouldCreateInstanceWithParameterCtor()
    {
        // Arrange
        var expected = typeof(RegistrationManager);
        var expectedEmail = "email@gmail.com";
        var expectedPassphrase = "somepassword";

        // Act
        var actual = new RegistrationManager(expectedEmail, expectedPassphrase);

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
            expected.IsValid = true;
        //Act
            var registrationManager = new RegistrationManager(goodEmail, "");
            var actual = registrationManager.ValidateEmail();
        //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid == expected.IsValid);
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
            expected.IsValid = false;
            //Act
            var registrationManager = new RegistrationManager(badEmail, "");
            var actual = registrationManager.ValidateEmail();
            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid == expected.IsValid);
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
            expected.IsValid = false;
            //Act
            var registrationManager = new RegistrationManager(badPassphrase, "");
            var actual = registrationManager.ValidatePassphrase();
            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid == expected.IsValid);
        }
    }

    [TestMethod]
    public void ShouldCreateInstanceOfRegistrationService()
    {
        //TODO: call RegistrationService myRegistration = new RegistrationService();
        // Actual

        // Act

        // Assert
    }

    [TestMethod]
    public void ShouldCreateNewAccountWithCreateAccount()
    {
        //TODO: myRegistration.CreateAccount(email, passphrase);
        // Actual

        // Act

        // Assert
    }
}
//References:
// Email test cases: https://www.rhyous.com/2010/06/15/csharp-email-regular-expression/