using DevelopmentHell.Hubba.Logging.Service.Implementation;
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

		private static string connectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
		private LoggerDataAccess dataAccess = new LoggerDataAccess(connectionString, ConfigurationManager.AppSettings["LogsTable"]!);

		[TestMethod]
		public async Task ConnectionSuccessful()
		{
			// Arrange
			var sut = new LoggerService(new LoggerDataAccess(connectionString, ConfigurationManager.AppSettings["LogsTable"]!));
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
		public void SuccessfulLogSystemSuccess()
		{
			// Arrange
            foreach (var logLevel in successLevels)
            {
                var category = Models.Category.VIEW;
                var userName = "System";
                var message = "test1";
                var sut = new LoggerService(dataAccess);

                var stopwatch = new Stopwatch();

                // Act
                stopwatch.Start();
                var actual = sut.Log(logLevel, category, userName, message);
                stopwatch.Stop();

                // Assert
                Assert.IsTrue(actual.IsSuccessful);
                Assert.IsTrue(stopwatch.ElapsedMilliseconds <= 5000);
            }
        }

		// Success Case 2
		[TestMethod]
        public void SuccessfulLogSystemFailure()
        {
			// Arrange
            foreach (var logLevel in failureLevels)
            {
                var category = Models.Category.VIEW;
                var userName = "System";
                var message = "test2";
                var sut = new LoggerService(dataAccess);

                var stopwatch = new Stopwatch();

                // Act
                stopwatch.Start();
                var actual = sut.Log(logLevel, category, userName, message);
                stopwatch.Stop();

                // Assert
                Assert.IsTrue(actual.IsSuccessful);
                Assert.IsTrue(stopwatch.ElapsedMilliseconds <= 5000);
            }
        }

		// Success Case 3
		[TestMethod]
        public void SuccessfulLogUserSuccess()
        {
			// Arrange
            foreach (var logLevel in successLevels)
            {
                var category = Models.Category.VIEW;
                var userName = "User1";
                var message = "test3";
                var sut = new LoggerService(dataAccess);

                var stopwatch = new Stopwatch();

                // Act
                stopwatch.Start();
                var actual = sut.Log(logLevel, category, userName, message);
                stopwatch.Stop();

                // Assert
                Assert.IsTrue(actual.IsSuccessful);
                Assert.IsTrue(stopwatch.ElapsedMilliseconds <= 5000);
            }
        }

		// Success Case 4
        [TestMethod]
        public void SuccessfulLogUserFailure()
        {
			// Arrange
            foreach (var logLevel in failureLevels)
            {
                var category = Models.Category.VIEW;
                var userName = "User1";
                var message = "test4";
                var sut = new LoggerService(dataAccess);

                var stopwatch = new Stopwatch();

                // Act
                stopwatch.Start();
                var actual = sut.Log(logLevel, category, userName, message);
                stopwatch.Stop();

                // Assert
                Assert.IsTrue(actual.IsSuccessful);
                Assert.IsTrue(stopwatch.ElapsedMilliseconds <= 5000);
            }
        }

		// Failure Case 1
		[TestMethod]
		public void LogWithin5Seconds()
		{
			// Arrange
			var category = Models.Category.VIEW;
			var logLevel = Models.LogLevel.INFO;
			var userName = "System";
			var message = "test5";
			var sut = new LoggerService(dataAccess);

			var stopwatch = new Stopwatch();

			// Act
			stopwatch.Start();
			var result = sut.Log(logLevel, category, userName, message);
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
			var sut = new LoggerService(dataAccess);

			// Code modified from https://stackoverflow.com/questions/27409247/how-to-test-blocking-function
			var ev = new AutoResetEvent(false);
			Result actual = new Result();
			// Act
			ThreadPool.QueueUserWorkItem((e) =>
			{
				actual = sut.Log(logLevel, category, userName, message);

				(e as AutoResetEvent)!.Set();
			}, ev);

			// Assert
			Assert.IsTrue(ev.WaitOne(1000));
			Assert.IsTrue(actual.IsSuccessful);
		}

		//// Failure Case 3
		//[TestMethod]
		//public async Task LogWriteSuccessWithin5Seconds()
		//{
		//	// Arrange
		//	var category = Models.Category.VIEW;
		//	var logLevel = Models.LogLevel.INFO;
		//	var userName = "System";
		//	var message = "test7";
		//	var sut = new LoggerService(dataAccess);

		//	var stopwatch = new Stopwatch();

		//	// Act
		//	var startTime = DateTime.UtcNow;
		//	await Task.Delay(100);
		//	stopwatch.Start();
		//	var actual = sut.Log(logLevel, category, userName, message);
		//	stopwatch.Stop();

		//	// Assert
		//	Assert.IsTrue(actual.IsSuccessful);
		//	Assert.IsTrue(stopwatch.ElapsedMilliseconds <= 5000);

		//	var dbCheck = await dataAccess.SelectLogs(new List<string>() { "Id", "Timestamp" }, new() { new("message", "=", message) });

		//	Assert.IsTrue(dbCheck.IsSuccessful);

		//	List<Dictionary<string, object>> payload = dbCheck.Payload!;
		//	Assert.IsTrue(payload.Count == 1);

		//	bool foundWithinTime = false;
		//	foreach (var row in payload)
		//	{
		//		DateTime now = Convert.ToDateTime(row["Timestamp"]);
		//		var timeDiff = now.Subtract(startTime).TotalSeconds;
		//		if (0 <= timeDiff)
		//		{
		//			foundWithinTime = true;
		//			break;
		//		}
		//	}
		//	Assert.IsTrue(foundWithinTime);
		//}

		//// Failure Case 4
		//[TestMethod]
		//public async Task LogWriteSuccessAccurateWithin5Seconds()
		//{
		//	// Arrange
		//	var category = Models.Category.VIEW;
		//	var logLevel = Models.LogLevel.INFO;
		//	var userName = "System";
		//	var message = "test8";
		//	var sut = new LoggerService(dataAccess);

		//	var stopwatch = new Stopwatch();

		//	// Act
		//	var startTime = DateTime.UtcNow;
		//	await Task.Delay(100);
		//	stopwatch.Start();
		//	var actual = sut.Log(logLevel, category, userName, message);
		//	stopwatch.Stop();

		//	// Assert
		//	Assert.IsTrue(actual.IsSuccessful);
		//	Assert.IsTrue(stopwatch.ElapsedMilliseconds <= 5000);

		//	var dbCheck = await dataAccess.SelectLogs(new List<string>() { "Id", "Timestamp" }, new() {
		//		new("Category", "=", category),
		//		new("LogLevel", "=", logLevel),
		//		new("UserName", "=", userName),
		//		new("Message", "=", message)
		//	}).ConfigureAwait(false);

		//	Assert.IsTrue(dbCheck.IsSuccessful);

		//	List<Dictionary<string, object>> payload = dbCheck.Payload!;
		//	Assert.IsTrue(payload.Count == 1);

		//	bool foundWithinTime = false;
		//	foreach (var row in payload)
		//	{
		//		DateTime now = Convert.ToDateTime(row["Timestamp"]);
		//		var timeDiff = now.Subtract(startTime).TotalSeconds;
		//		if (0 <= timeDiff)
		//		{
		//			foundWithinTime = true;
		//			break;
		//		}
		//	}
		//	Assert.IsTrue(foundWithinTime);
		//}

		//// Failure Case 5
		//[TestMethod]
		//public async Task LogWriteIsNotModifiable()
		//{
		//	// Arrange
		//	var category = Models.Category.VIEW;
		//	var logLevel = Models.LogLevel.INFO;
		//	var userName = "System";
		//	var message = "test9";
		//	var sut = new LoggerService(dataAccess);

		//	// Act
		//	var startTime = DateTime.UtcNow;
		//	await Task.Delay(100);
		//	var actual = sut.Log(logLevel, category, userName, message);

		//	// Assert
		//	var dbCheck = await dataAccess.SelectLogs(new List<string>() { "Id", "Timestamp" }, new List<Comparator> {
		//		new Comparator("Category", "=", category),
		//		new Comparator("LogLevel", "=", logLevel),
		//		new Comparator("UserName", "=", userName),
		//		new Comparator("Message", "=", message)
		//	}).ConfigureAwait(false);

		//	Assert.IsTrue(!dbCheck.IsSuccessful);

		//	List<Dictionary<string, object>> payload = dbCheck.Payload!;
		//	Assert.IsTrue(payload.Count == 1);

		//	Result updateResult = new Result()
		//	{
		//		IsSuccessful = false,
		//	};

		//	foreach (var row in payload)
		//	{
		//		DateTime now = Convert.ToDateTime(row["Timestamp"]);
		//		var timeDiff = now.Subtract(startTime).TotalSeconds;
		//		if (0 <= timeDiff)
		//		{
		//			updateResult = await new UpdateDataAccess(connectionString).Update("logs",
		//				new() { new("Id", "=", row["Id"]) },
		//				new Dictionary<string, object>()
		//				{
		//					{ "Message", "newTestMessage" },
		//				});
		//		}

		//		Assert.IsTrue(!updateResult.IsSuccessful);
		//	}

		//}
	}
}