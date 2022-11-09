using DevelopmentHell.Hubba.Logging.Implementation;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using Microsoft.Data.SqlClient;
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
		public async Task ConnectionSuccessful()
		{
			// Arrange
			var expectedDatabaseName = "DevelopmentHell.Hubba.Logs";
			var connectionString = String.Format(@"Server={0};Database={1};Integrated Security=True;Encrypt=False", ConfigurationManager.AppSettings["LoggingServer"], expectedDatabaseName);
			var sut = new Logger(new LoggingDataAccess(connectionString), Models.Category.VIEW);
			var connectionAvailible = false;

			// Act
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				try
				{
					await conn.OpenAsync().ConfigureAwait(false);
					connectionAvailible = true;
				}
				catch (SqlException e)
				{
					connectionAvailible = false;
				}
			}

			// Assert
			Assert.IsTrue(connectionAvailible);
		}

		// Success Case 1
		[TestMethod]
		public async Task SuccessfulLogSystemSuccess()
		{
			// Arrange
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

		// Success Case 2
		[TestMethod]
        public async Task SuccessfulLogSystemFailure()
        {
			// Arrange
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

		// Success Case 3
		[TestMethod]
        public async Task SuccessfulLogUserSuccess()
        {
			// Arrange
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

		// Success Case 4
        [TestMethod]
        public async Task SuccessfulLogUserFailure()
        {
			// Arrange
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

		// Failure Case 1
		[TestMethod]
		public async Task LogWithin5Seconds()
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
			stopwatch.Start();
			await sut.Log(logLevel, userName, message);
			stopwatch.Stop();
			
			// Assert
			Assert.IsTrue(stopwatch.ElapsedMilliseconds <= 5000);

		}

		// Failure Case 2
		[TestMethod]
		public void LogWriteSuccessWithoutBlocking()
		{
			// Arrange
			var expectedDatabaseName = "DevelopmentHell.Hubba.Logs";
			var connectionString = String.Format(@"Server={0};Database={1};Integrated Security=True;Encrypt=False", ConfigurationManager.AppSettings["LoggingServer"], expectedDatabaseName);
			var dataAccess = new LoggingDataAccess(connectionString);

			var category = Models.Category.VIEW;
			var logLevel = Models.LogLevel.INFO;
			var userName = "System";
			var message = "test6";
			var sut = new Logger(dataAccess, category);

			// Code modified from https://stackoverflow.com/questions/27409247/how-to-test-blocking-function
			var ev = new AutoResetEvent(false);

			// Act
			ThreadPool.QueueUserWorkItem(async (e) =>
			{
				var actual = await sut.Log(logLevel, userName, message);
				(e as AutoResetEvent)!.Set();
			}, ev);

			// Assert
			Assert.IsTrue(ev.WaitOne(1000));
		}

		// Failure Case 3
		[TestMethod]
		public async Task LogWriteSuccessWithin5Seconds()
		{
			// Arrange
			var expectedDatabaseName = "DevelopmentHell.Hubba.Logs";
			var connectionString = String.Format(@"Server={0};Database={1};Integrated Security=True;Encrypt=False", ConfigurationManager.AppSettings["LoggingServer"], expectedDatabaseName);
			var dataAccess = new LoggingDataAccess(connectionString);

			var category = Models.Category.VIEW;
			var logLevel = Models.LogLevel.INFO;
			var userName = "System";
			var message = "test7";
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

			var dbCheck = await dataAccess.SelectLogs(new List<string>() { "id", "timestamp" }, new Dictionary<string, object>()
			{
				{ "message", message }
			}).ConfigureAwait(false);

			var payload = (dbCheck.Payload as List<List<object>>)!;
			Assert.IsTrue(payload.Count >= 1);

			bool foundWithinTime = false;
			foreach (List<object> row in payload)
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
		}

        // Failure Case 4
		[TestMethod]
		public async Task LogWriteSuccessAccurateWithin5Seconds()
		{
            // Arrange
            var expectedDatabaseName = "DevelopmentHell.Hubba.Logs";
            var connectionString = String.Format(@"Server={0};Database={1};Integrated Security=True;Encrypt=False", ConfigurationManager.AppSettings["LoggingServer"], expectedDatabaseName);
            var dataAccess = new LoggingDataAccess(connectionString);

			var category = Models.Category.VIEW;
			var logLevel = Models.LogLevel.INFO;
			var userName = "System";
			var message = "test8";
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
			}).ConfigureAwait(false);

            var payload = (dbCheck.Payload as List<List<object>>)!;
			Assert.IsTrue(payload.Count >= 1);

			bool foundWithinTime = false;
			foreach (List<object> row in payload)
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
        }

		// Failure Case 5
		[TestMethod]
		public async Task LogWriteIsNotModifiable()
		{
			// Arrange
			var expectedDatabaseName = "DevelopmentHell.Hubba.Logs";
			var connectionString = String.Format(@"Server={0};Database={1};Integrated Security=True;Encrypt=False", ConfigurationManager.AppSettings["LoggingServer"], expectedDatabaseName);
			var dataAccess = new LoggingDataAccess(connectionString);

			var category = Models.Category.VIEW;
			var logLevel = Models.LogLevel.INFO;
			var userName = "System";
			var message = "test9";
			var sut = new Logger(dataAccess, category);

			// Act
			var startTime = DateTime.UtcNow;
			await Task.Delay(100);
			var actual = await sut.Log(logLevel, userName, message);

			// Assert
			var dbCheck = await dataAccess.SelectLogs(new List<string>() { "id", "timestamp" }, new Dictionary<string, object> {
				{ "category", category },
				{ "logLevel", logLevel },
				{ "userName", userName },
				{ "message", message },
			}).ConfigureAwait(false);

			var payload = (dbCheck.Payload as List<List<object>>)!;
			Assert.IsTrue(payload.Count >= 1);

			Result updateResult = new Result(IsSuccessful: false);
			foreach (List<object> row in payload)
			{
				var now = Convert.ToDateTime((DateTime?)row[1]);
				var timeDiff = now.Subtract(startTime).TotalSeconds;
				if (0 <= timeDiff)
				{
					updateResult = await new UpdateDataAccess(connectionString).Update("logs",
						new Tuple<string, object>("id", row[0]),
						new Dictionary<string, object>()
						{
							{ "message", "newTestMessage" },
						});
					break;
				}
			}

			Assert.IsTrue(!updateResult!.IsSuccessful);
		}
	}
}