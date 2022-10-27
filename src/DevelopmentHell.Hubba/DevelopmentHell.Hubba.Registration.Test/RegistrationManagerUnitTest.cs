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
        //var expected = new Result();
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
        

        // Act

        // Assert
    }

    [TestMethod]
    public void ShouldCheckForValidPassphrase()
    {
        // Actual

        // Act

        // Assert
    }


}
