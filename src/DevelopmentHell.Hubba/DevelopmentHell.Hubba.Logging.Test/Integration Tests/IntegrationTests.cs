using DevelopmentHell.Hubba.Logging.Implementation;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Globalization;
using System.Reflection;

namespace DevelopmentHell.Hubba.Logging.Test
{
	[TestClass]
	public class IntegrationTests
	{
		[TestMethod]
		public async Task TestMethod1()
		{
			// Arrange
			var expected = typeof(Logger);
			var expectedDatabaseName = "DevelopmentHell.Hubba.Logs";
			var connectionString = String.Format(@"Server={0};Database={1};Integrated Security=True;Encrypt=False", ConfigurationManager.AppSettings["LoggingServer"], expectedDatabaseName);
			var sut = new Logger(new LoggingDataAccess(connectionString), Models.Category.VIEW);

			// Act
			var actual = await sut.Log(Models.LogLevel.INFO, "test", "test");

			// Assert
			Assert.IsTrue(actual.IsSuccessful);
		}

		[TestMethod]
		public async Task WriteSuccessResponseButNoUpdate()
		{
            // Arrange
            var expected = typeof(Logger);
            var expectedDatabaseName = "DevelopmentHell.Hubba.Logs";
            var connectionString = String.Format(@"Server={0};Database={1};Integrated Security=True;Encrypt=False", ConfigurationManager.AppSettings["LoggingServer"], expectedDatabaseName);
            var dataAccess = new LoggingDataAccess(connectionString);

			var category = Models.Category.VIEW;
			var logLevel = Models.LogLevel.INFO;
			var userName = "System";
			var message = "test";
			var sut = new Logger(dataAccess, category);
			
			var stopwatch = new Stopwatch();

			// Act
			var startTime = DateTime.UtcNow;
			stopwatch.Start();
			var actual = await sut.Log(logLevel, userName, message);
			stopwatch.Stop();

            // Assert
            Assert.IsTrue(actual.IsSuccessful);
			Assert.IsTrue(stopwatch.ElapsedMilliseconds <= 5000);

			var dbChecker = new SelectDataAccess(connectionString);
			var dbCheck = await dataAccess.SelectLogs(new List<string>() { "id", "timestamp" }, new Dictionary<string, object> {
				{ "category", category },
				{ "logLevel", logLevel },
				{ "userName", userName },
				{ "message", message },
			});

			bool foundWithinTime = false;
			foreach (List<object> row in (dbCheck.Payload as List<List<object>>)!)
			{
				var now = Convert.ToDateTime((DateTime?)row[1]);
				var timeDiff = now.Subtract(startTime).TotalSeconds;
                if (0 <= timeDiff && timeDiff < 5)
				{
					foundWithinTime = true;
					break;
				}
			}
			Assert.IsTrue(foundWithinTime);
			Assert.IsTrue((dbCheck.Payload as List<List<object>>)!.Count >= 1);

        }
	}
}