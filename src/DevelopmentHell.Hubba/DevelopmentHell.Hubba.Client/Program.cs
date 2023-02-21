using DevelopmentHell.Hubba.Analytics.Service.Implementation;
using DevelopmentHell.Hubba.Logging.Service.Implementation;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using System.Configuration;

namespace DevelopmentHell.Hubba.Client
{
	public class Program
	{
		public async static Task Main(string[] args)
		{
			// on the service layer, set current query timestamp and store data in database so next query will query that if > 60s
			var _loggerService = new LoggerService(new LoggerDataAccess(ConfigurationManager.AppSettings["LogsConnectionString"]!, ConfigurationManager.AppSettings["LogsTable"]!));
			_loggerService.Log(LogLevel.INFO, Category.BUSINESS, $"New booking: TEST.", "123");
			var test = new AnalyticsDataAccess(ConfigurationManager.AppSettings["LogsConnectionString"]!, ConfigurationManager.AppSettings["LogsTable"]!);

			var analyticsService = new AnalyticsService(test, _loggerService);
			var result = await analyticsService.GetData(DateTime.Now.AddMonths(-3)).ConfigureAwait(false);
			if (result.IsSuccessful && result.Payload is not null)
			{
				foreach (var i in result.Payload)
				{
					Console.WriteLine(i.Key);
					foreach (var j in i.Value)
					{
						Console.WriteLine($"\t{j.Key}\t{j.Value}");
					}
				}
			}
			
			var app = new ViewDemoConsole();
			await app.Run();
		}
	}
}