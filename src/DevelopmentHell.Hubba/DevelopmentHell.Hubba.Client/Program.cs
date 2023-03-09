using DevelopmentHell.Hubba.AccountDeletion.Manager;
using DevelopmentHell.Hubba.AccountDeletion.Service.Abstractions;
using DevelopmentHell.Hubba.AccountDeletion.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
﻿using DevelopmentHell.Hubba.AccountRecovery.Manager;
using DevelopmentHell.Hubba.AccountRecovery.Service.Implementations;
using DevelopmentHell.Hubba.Analytics.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
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
			await app.Run();
			//var dao = new UserLoginDataAccess(ConfigurationManager.AppSettings["UsersConnectionString"]!, ConfigurationManager.AppSettings["UserLoginsTable"]!);
			//var result = await dao.AddLogin(999, "192.168.255.100");
			//if (!result.IsSuccessful)
			//{
			//	Console.WriteLine(result.ErrorMessage);
			//	return;
			//}
			//Console.Write("Success ^._.^");
			//var manager = new AccountRecoveryManager(new AccountRecoveryService()

		}
	}
}