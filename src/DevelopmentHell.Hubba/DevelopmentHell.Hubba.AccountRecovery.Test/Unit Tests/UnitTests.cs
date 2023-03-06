using DevelopmentHell.Hubba.AccountRecovery.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.AccountRecovery.Test.Unit_Tests
{
    [TestClass]
    public class UnitTests
    {
        /*
         * Success Case
         * Goal: 
         * Process: 
         */
        [TestMethod]
        public void Test01()
        {
            // Arrange
            string dummyConnectionString = "";
            string dummyTable = "";

            // Act
            var disconnectedAccountRecoveryService = new AccountRecoveryService(
                new UserAccountDataAccess(dummyConnectionString, dummyTable),
                new LoggerService(new LoggerDataAccess(dummyConnectionString, dummyTable)),
                new UserLoginDataAccess(dummyConnectionString, dummyTable),
                new RecoveryRequestDataAccess(dummyConnectionString, dummyTable)
                );


            // Assert

        }
    }
}
