using DevelopmentHell.Hubba.AccountDeletion.Manager;
using DevelopmentHell.Hubba.AccountDeletion.Manager.Implementations;
using DevelopmentHell.Hubba.AccountDeletion.Service.Abstractions;
using DevelopmentHell.Hubba.AccountDeletion.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using Microsoft.Identity.Client;
using System.Configuration;
using System.Security.Principal;
using HubbaConfig = System.Configuration;



namespace DevelopmentHell.Hubba.AccountDeletion.Test
{
    [TestClass]
    public class IntegrationTests
    {
        private string _UsersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private string _UserAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private string _LogsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private string _LogsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        
        
        [TestMethod]
        public async Task DeleteVerifiedUserAccount()
        {
            // Arrange
            var dao = new UserAccountDataAccess(_UsersConnectionString, _UserAccountsTable);
            var loggerService = new LoggerService(new LoggerDataAccess(_LogsConnectionString, _LogsTable));
            var accountDeletionService = new AccountDeletionService(dao, loggerService);
            var authorizationService = 
                new AuthorizationService(
		            HubbaConfig.ConfigurationManager.AppSettings,
                    dao,
		            loggerService
	            );
            var accountDeletionManager = new AccountDeletionManager(accountDeletionService, authorizationService, loggerService);
            var identity = new GenericIdentity("2");
            var principal = new GenericPrincipal(identity, new string[] { "VerifiedUser" });
            Thread.CurrentPrincipal = principal;
            Result expected = new Result()
            {
                IsSuccessful = true
            };

            // Act
            Result actual = await accountDeletionManager.DeleteAccount(2);

            // Assert
            Assert.IsTrue(expected.IsSuccessful == actual.IsSuccessful);
        }
    }
}