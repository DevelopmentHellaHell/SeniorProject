using DevelopmentHell.Hubba.Registration.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Registration.Test.UnitTests
{

    [TestClass]
    public class PassphraseValidatioUnitTest
    {

        [TestMethod]
        public void ShouldCheckForInvalidPassphrase()
        {
            //TODO: check invalid passphrase
            // Arrange
            List<String> badPassphrases = new List<String>();
            badPassphrases.Add("joe"); // length < 8
            badPassphrases.Add("joe1234$56"); // invalid special character

            foreach (String badPassphrase in badPassphrases)
            {
                var expected = new Result(false, "Passphrase provided is invalid. Retry or contact admin.");

                //Act
                var actual = PassphraseValidation.validate(badPassphrase);

                //Assert
                Assert.IsNotNull(actual);
                Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
                Assert.IsTrue(actual.ErrorMessage == expected.ErrorMessage);
            }
        }

        [TestMethod]
        public void ShouldCheckForValidPassphrase()
        {
            //TODO: check valid passphrase
            // Arrange
            List<String> goodPassphrases = new List<String>();
            goodPassphrases.Add("        "); 
            goodPassphrases.Add("@joE1234- ! 56");

            foreach (String goodPassphrase in goodPassphrases)
            {
                var expected = new Result(true);

                //Act
                var actual = PassphraseValidation.validate(goodPassphrase);

                //Assert
                Assert.IsNotNull(actual);
                Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
                
            }
        }



    }
}
