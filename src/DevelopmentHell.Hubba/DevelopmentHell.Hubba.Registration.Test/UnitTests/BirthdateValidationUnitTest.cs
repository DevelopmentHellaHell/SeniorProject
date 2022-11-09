using DevelopmentHell.Hubba.Registration.Implementation;
using DevelopmentHell.Hubba.Models;


namespace DevelopmentHell.Hubba.Registration.Test.UnitTests;


[TestClass]
public class BirthdateValidationUnitTest
{
    [TestMethod]
    public void ValidBirthdates()
    {
        // Arrange
        var expected = new Result(false, "Age requirement not met.");
        List<DateTime> goodBirthdates = new List<DateTime>();
        goodBirthdates.Add(new DateTime(2000, 01, 01));
        goodBirthdates.Add(new DateTime(2008, 11, 01));
        goodBirthdates.Add(new DateTime(1995, 05, 16));

        foreach (DateTime goodBirthdate in goodBirthdates)
        {
            //Act
            var actual = BirthdateValidation.validate(goodBirthdate);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.GetType() == expected.GetType());
            Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
        }
        
        
    }

    [TestMethod]
    public void InvalidBirthdates() //make invalid date
    {
        // Arrange
        var expected = new Result(false, "Age requirement not met.");
        List<DateTime> badBirthdates = new List<DateTime>();
        badBirthdates.Add(DateTime.Today);
        badBirthdates.Add(new DateTime(2018, 11, 01));
        badBirthdates.Add(new DateTime(2018, 11, 15));

        foreach (DateTime badBirthdate in badBirthdates)
        {
            //Act
            var actual = BirthdateValidation.validate(badBirthdate);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.GetType() == expected.GetType());
            Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
            Assert.IsTrue(actual.ErrorMessage == expected.ErrorMessage);
        }
    }

}
//References: