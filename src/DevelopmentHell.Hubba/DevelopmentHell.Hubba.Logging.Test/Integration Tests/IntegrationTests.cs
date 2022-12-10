using DevelopmentHell.Hubba.Logging.Implementation;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

namespace DevelopmentHell.Hubba.Logging.Test
{
	[TestClass]
	public class IntegrationTests
	{
        private readonly Models.LogLevel[] failureLevels = { Models.LogLevel.WARNING, Models.LogLevel.ERROR };
        private readonly Models.LogLevel[] successLevels = { Models.LogLevel.DEBUG, Models.LogLevel.INFO };

		private static string expectedDatabaseName = "DevelopmentHell.Hubba.Logs";
		private static string connectionString = String.Format(@"Server={0};Database={1};Encrypt=false;User Id=DevelopmentHell.Hubba.SqlUser.Logging;Password=password", ConfigurationManager.AppSettings["LoggingServer"], expectedDatabaseName);
		private LoggerDataAccess dataAccess = new LoggerDataAccess(connectionString);

		[TestMethod]
		public async Task ConnectionSuccessful()
		{
			// Arrange
			var sut = new Logger(new LoggerDataAccess(connectionString));
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
					Console.WriteLine(e.ToString());
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
            foreach (var logLevel in successLevels)
            {
                var category = Models.Category.VIEW;
                var userName = "System";
                var message = "test1";
                var sut = new Logger(dataAccess);

                var stopwatch = new Stopwatch();

                // Act
                stopwatch.Start();
                var actual = await sut.Log(logLevel, category, userName, message);
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
            foreach (var logLevel in failureLevels)
            {
                var category = Models.Category.VIEW;
                var userName = "System";
                var message = "test2";
                var sut = new Logger(dataAccess);

                var stopwatch = new Stopwatch();

                // Act
                stopwatch.Start();
                var actual = await sut.Log(logLevel, category, userName, message);
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
            foreach (var logLevel in successLevels)
            {
                var category = Models.Category.VIEW;
                var userName = "User1";
                var message = "test3";
                var sut = new Logger(dataAccess);

                var stopwatch = new Stopwatch();

                // Act
                stopwatch.Start();
                var actual = await sut.Log(logLevel, category, userName, message);
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
            foreach (var logLevel in failureLevels)
            {
                var category = Models.Category.VIEW;
                var userName = "User1";
                var message = "test4";
                var sut = new Logger(dataAccess);

                var stopwatch = new Stopwatch();

                // Act
                stopwatch.Start();
                var actual = await sut.Log(logLevel, category, userName, message);
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
			var category = Models.Category.VIEW;
			var logLevel = Models.LogLevel.INFO;
			var userName = "System";
			var message = "test5";
			var sut = new Logger(dataAccess);

			var stopwatch = new Stopwatch();

			// Act
			stopwatch.Start();
			var result = await sut.Log(logLevel, category, userName, message);
			stopwatch.Stop();

			// Assert
			Assert.IsTrue(stopwatch.ElapsedMilliseconds <= 5000);
			Assert.IsTrue(result.IsSuccessful);
		}

		// Failure Case 2
		[TestMethod]
		public void LogWriteSuccessWithoutBlocking()
		{
			// Arrange
			var category = Models.Category.VIEW;
			var logLevel = Models.LogLevel.INFO;
			var userName = "System";
			var message = "test6";
			var sut = new Logger(dataAccess);

			// Code modified from https://stackoverflow.com/questions/27409247/how-to-test-blocking-function
			var ev = new AutoResetEvent(false);
			Result actual = new Result();
			// Act
			ThreadPool.QueueUserWorkItem(async (e) =>
			{
				actual = await sut.Log(logLevel, category, userName, message);

				(e as AutoResetEvent)!.Set();
			}, ev);

			// Assert
			Assert.IsTrue(ev.WaitOne(1000));
			Assert.IsTrue(actual.IsSuccessful);
		}

		// Failure Case 3
		[TestMethod]
		public async Task LogWriteSuccessWithin5Seconds()
		{
			// Arrange
			var category = Models.Category.VIEW;
			var logLevel = Models.LogLevel.INFO;
			var userName = "System";
			var message = "test7";
			var sut = new Logger(dataAccess);

			var stopwatch = new Stopwatch();

			// Act
			var startTime = DateTime.UtcNow;
			await Task.Delay(100);
			stopwatch.Start();
			var actual = await sut.Log(logLevel, category, userName, message);
			stopwatch.Stop();

			// Assert
			Assert.IsTrue(actual.IsSuccessful);
			Assert.IsTrue(stopwatch.ElapsedMilliseconds <= 5000);

			var dbCheck = await dataAccess.SelectLogs(new List<string>() { "id", "timestamp" }, new () {new("message","=",message)});

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
			var category = Models.Category.VIEW;
			var logLevel = Models.LogLevel.INFO;
			var userName = "System";
			var message = "test8";
			var sut = new Logger(dataAccess);
			
			var stopwatch = new Stopwatch();

			// Act
			var startTime = DateTime.UtcNow;
            await Task.Delay(100);
            stopwatch.Start();
            var actual = await sut.Log(logLevel, category, userName, message);
            stopwatch.Stop();

            // Assert
            Assert.IsTrue(actual.IsSuccessful);
			Assert.IsTrue(stopwatch.ElapsedMilliseconds <= 5000);

            var dbCheck = await dataAccess.SelectLogs(new List<string>() { "id", "timestamp" }, new() {
				new("category", "=", category),
				new("logLevel", "=", logLevel),
				new("userName", "=", userName),
				new("message", "=", message)
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
			var category = Models.Category.VIEW;
			var logLevel = Models.LogLevel.INFO;
			var userName = "System";
			var message = "test9";
			var sut = new Logger(dataAccess);

			// Act
			var startTime = DateTime.UtcNow;
			await Task.Delay(100);
			var actual = await sut.Log(logLevel, category, userName, message);

			// Assert
			var dbCheck = await dataAccess.SelectLogs(new List<string>() { "id", "timestamp" }, new List<Comparator> {
                new Comparator("category", "=", category),
                new Comparator("logLevel", "=", logLevel),
                new Comparator("userName", "=", userName),
                new Comparator("message", "=", message)
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
						new() { new("id", "=", row[0]) },
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