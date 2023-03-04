using System.Configuration;

namespace DevelopmentHell.Hubba.AccountDelection.Test
{
    [TestClass]
    public class UnitTests
    {
        private string _UsersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private string _UserAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private string _UserOTPsTable = ConfigurationManager.AppSettings["UserOTPsTable"]!;

        private string _LogsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private string _LogsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        
        
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange

            // Act

            // Assert
        }
    }
}