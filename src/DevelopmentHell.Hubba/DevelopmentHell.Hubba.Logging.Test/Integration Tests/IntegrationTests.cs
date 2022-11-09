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
        private readonly Models.LogLevel[] failureLevels = { Models.LogLevel.WARNING, Models.LogLevel.ERROR };
        private readonly Models.LogLevel[] successLevels = { Models.LogLevel.DEBUG, Models.LogLevel.INFO };

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
		public async Task SuccessfulLogSystemSuccess()
		{
            var expectedDatabaseName = "DevelopmentHell.Hubba.Logs";
            var connectionString = String.Format(@"Server={0};Database={1};Integrated Security=True;Encrypt=False", ConfigurationManager.AppSettings["LoggingServer"], expectedDatabaseName);
            var dataAccess = new LoggingDataAccess(connectionString);

            foreach (var logLevel in successLevels)
            {
                var category = Models.Category.VIEW;
                var userName = "System";
                var message = "test1";
                var sut = new Logger(dataAccess, category);

                var stopwatch = new Stopwatch();

                // Act
                stopwatch.Start();
                var actual = await sut.Log(logLevel, userName, message);
                stopwatch.Stop();

                // Assert
                Assert.IsTrue(actual.IsSuccessful);
                Assert.IsTrue(stopwatch.ElapsedMilliseconds <= 5000);
            }
        }

        [TestMethod]
        public async Task SuccessfulLogSystemFailure()
        {
            var expectedDatabaseName = "DevelopmentHell.Hubba.Logs";
            var connectionString = String.Format(@"Server={0};Database={1};Integrated Security=True;Encrypt=False", ConfigurationManager.AppSettings["LoggingServer"], expectedDatabaseName);
            var dataAccess = new LoggingDataAccess(connectionString);

            foreach (var logLevel in failureLevels)
            {
                var category = Models.Category.VIEW;
                var userName = "System";
                var message = "test2";
                var sut = new Logger(dataAccess, category);

                var stopwatch = new Stopwatch();

                // Act
                stopwatch.Start();
                var actual = await sut.Log(logLevel, userName, message);
                stopwatch.Stop();

                // Assert
                Assert.IsTrue(actual.IsSuccessful);
                Assert.IsTrue(stopwatch.ElapsedMilliseconds <= 5000);
            }
        }

        [TestMethod]
        public async Task SuccessfulLogUserSuccess()
        {
            var expectedDatabaseName = "DevelopmentHell.Hubba.Logs";
            var connectionString = String.Format(@"Server={0};Database={1};Integrated Security=True;Encrypt=False", ConfigurationManager.AppSettings["LoggingServer"], expectedDatabaseName);
            var dataAccess = new LoggingDataAccess(connectionString);

            foreach (var logLevel in successLevels)
            {
                var category = Models.Category.VIEW;
                var userName = "User1";
                var message = "test3";
                var sut = new Logger(dataAccess, category);

                var stopwatch = new Stopwatch();

                // Act
                stopwatch.Start();
                var actual = await sut.Log(logLevel, userName, message);
                stopwatch.Stop();

                // Assert
                Assert.IsTrue(actual.IsSuccessful);
                Assert.IsTrue(stopwatch.ElapsedMilliseconds <= 5000);
            }
        }

        [TestMethod]
        public async Task SuccessfulLogUserFailure()
        {
            var expectedDatabaseName = "DevelopmentHell.Hubba.Logs";
            var connectionString = String.Format(@"Server={0};Database={1};Integrated Security=True;Encrypt=False", ConfigurationManager.AppSettings["LoggingServer"], expectedDatabaseName);
            var dataAccess = new LoggingDataAccess(connectionString);

            foreach (var logLevel in failureLevels)
            {
                var category = Models.Category.VIEW;
                var userName = "User1";
                var message = "test4";
                var sut = new Logger(dataAccess, category);

                var stopwatch = new Stopwatch();

                // Act
                stopwatch.Start();
                var actual = await sut.Log(logLevel, userName, message);
                stopwatch.Stop();

                // Assert
                Assert.IsTrue(actual.IsSuccessful);
                Assert.IsTrue(stopwatch.ElapsedMilliseconds <= 5000);
            }
        }

        [TestMethod]
		public async Task WriteSuccessResponseButNoUpdate()
		{
            // Arrange
            var expectedDatabaseName = "DevelopmentHell.Hubba.Logs";
            var connectionString = String.Format(@"Server={0};Database={1};Integrated Security=True;Encrypt=False", ConfigurationManager.AppSettings["LoggingServer"], expectedDatabaseName);
            var dataAccess = new LoggingDataAccess(connectionString);

			var category = Models.Category.VIEW;
			var logLevel = Models.LogLevel.INFO;
			var userName = "System";
			var message = "test5";
			var sut = new Logger(dataAccess, category);
			
			var stopwatch = new Stopwatch();

			// Act
			var startTime = DateTime.UtcNow;
            await Task.Delay(100);
            stopwatch.Start();
            var actual = await sut.Log(logLevel, userName, message);
            stopwatch.Stop();

            // Assert
            Assert.IsTrue(actual.IsSuccessful);
			Assert.IsTrue(stopwatch.ElapsedMilliseconds <= 5000);

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
                if (0 <= timeDiff)
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