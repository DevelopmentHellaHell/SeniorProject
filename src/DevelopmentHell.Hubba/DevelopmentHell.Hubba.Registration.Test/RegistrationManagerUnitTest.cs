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
        var expectedValidEmail1 = "someemail@gmail.com";
        var expectedValidEmail2 = "someemail_other@gmail.com.co";
        var expected = new Result().IsValid;
        expected = true;

        // Act
        var user1 = new RegistrationManager(expectedValidEmail1, "");
        var user2 = new RegistrationManager(expectedValidEmail2, "");
        var actual1 = new Result();
        var actual2 = new Result();
        actual1 = user1.ValidateEmail();
        actual2 = user2.ValidateEmail();

        // Assert
        Assert.IsNotNull(user1.ValidateEmail());
        Assert.IsNotNull(user2.ValidateEmail());
        Assert.IsTrue(actual1.IsValid == expected);
        Assert.IsTrue(actual2.IsValid == expected);
    }

    [TestMethod]
    public void ShouldCheckForInvalidEmail()
    {
        // Arrange
        var expectedInvalidEmail1 = "someemail";
        var expectedInvalidEmail2 = "someemail@g";
        var expectedInvalidEmail3 = "someemail@g.co@";
        var expected = new Result().IsValid;
        expected = false;

        // Act
        var user1 = new RegistrationManager(expectedInvalidEmail1, "");
        var user2 = new RegistrationManager(expectedInvalidEmail2, "");
        var user3 = new RegistrationManager(expectedInvalidEmail3, "");
        var actual1 = new Result();
        var actual2 = new Result();
        var actual3 = new Result();
        actual1 = user1.ValidateEmail();
        actual2 = user2.ValidateEmail();

        // Assert
        Assert.IsNotNull(user1.ValidateEmail());
        Assert.IsNotNull(user2.ValidateEmail());
        Assert.IsNotNull(user3.ValidateEmail());
        Assert.IsTrue(actual1.IsValid == expected);
        Assert.IsTrue(actual2.IsValid == expected);
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
