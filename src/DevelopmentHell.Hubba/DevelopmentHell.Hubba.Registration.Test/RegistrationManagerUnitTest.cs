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
        goodEmails.Add("joe.bob@home.com");
        goodEmails.Add("joe_bob@home.com");
        goodEmails.Add("joe-bob@home.com");
        goodEmails.Add("joe@his.home.place");
        goodEmails.Add("a@abc.org");
        goodEmails.Add("a@abc-xyz.org");
        goodEmails.Add("a@192.168.0.1");
        goodEmails.Add("a@10.1.100.1");

        foreach (String goodEmail in goodEmails)
        {
            var expected = new Result();
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
        badEmails.Add("joe"); // should fail
        badEmails.Add("joe@home"); // should fail
        //badEmails.Add("a@b.c"); // should fail because .c is only one character but must be 2-4 characters
        badEmails.Add("joe-bob[at]home.com"); // should fail because [at] is not valid
        badEmails.Add("joe.@bob.com"); // should fail because there is a dot at the end of the local-part
        badEmails.Add(".joe@bob.com"); // should fail because there is a dot at the beginning of the local-part
        badEmails.Add("john..doe@bob.com"); // should fail because there are two dots in the local-part
        badEmails.Add("john.doe@bob..com"); // should fail because there are two dots in the domain
        badEmails.Add("joe<>bob@bob.com"); // should fail because <> are not valid
        badEmails.Add("joe&bob@bob.com"); // should fail because & is not valid
        badEmails.Add("~joe@bob.com"); // should fail because ~ is not valid
        badEmails.Add("joe$@bob.com"); // should fail because $ is not valid
        badEmails.Add("joe+bob@bob.com"); // should fail because + is not valid
        badEmails.Add("o'reilly@there.com"); // should fail because ' is not valid
        badEmails.Add("joe@his.home.com."); // should fail because it can't end with a period
        badEmails.Add("john.doe@bob-.com"); // should fail because there is a dash at the start of a domain part
        badEmails.Add("john.doe@-bob.com"); // should fail because there is a dash at the end of a domain part
        badEmails.Add("a@10.1.100.1a");  // Should fail because of the extra character
        badEmails.Add("joe<>bob@bob.com\n"); // should fail because it end with \n
        badEmails.Add("joe<>bob@bob.com\r"); // should fail because it ends with \r

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
    public void ShouldCheckForValidPassphrase()
    {
        //TODO: check valid passphrase
        // Actual

        // Act

        // Assert
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
