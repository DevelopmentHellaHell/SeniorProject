using DevelopmentHell.Hubba.AccountDeletion.Manager;
using DevelopmentHell.Hubba.AccountDeletion.Service.Abstractions;
using DevelopmentHell.Hubba.AccountDeletion.Service.Implementation;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Implementation;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Implementation;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using Microsoft.Identity.Client;
using System.Configuration;
using System.Security.Principal;

namespace DevelopmentHell.Hubba.Client
{
	public class Program
	{
		public async static Task Main(string[] args)
		{
			
			var app = new ViewDemoConsole();
            //await app.Run();
            // TODO: REMOVE THIS
            string UserAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
            string LogsTable = ConfigurationManager.AppSettings["LogsTable"]!;
            string UsersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
            string LogsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;

            var dao = new UserAccountDataAccess(UsersConnectionString, UserAccountsTable);
            var loggerService = new LoggerService(new LoggerDataAccess(LogsConnectionString, LogsTable));
            var accountDeletionService = new AccountDeletionService(dao, loggerService);
            var authorizationService = new AuthorizationService();

            var accountDeletionManager = new AccountDeletionManager(accountDeletionService, authorizationService, loggerService);
            //Result result = await accountDeletionService.DeleteAccountNotifyListingsBookings(16);


            var identity = new GenericIdentity("1");
            var principal = new GenericPrincipal(identity, new string[] { "VerifiedUser" });
            Thread.CurrentPrincipal = principal;

            Result result = await accountDeletionManager.DeleteAccount(1, principal);
            Console.WriteLine(result.ErrorMessage);
            
        }
	}
}