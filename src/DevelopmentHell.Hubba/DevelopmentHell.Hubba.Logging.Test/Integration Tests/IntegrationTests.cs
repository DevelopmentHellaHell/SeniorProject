using DevelopmentHell.Hubba.Logging.Implementation;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
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
			var connectionString = String.Format(@"Server=.;Database={0};Integrated Security=True;Encrypt=False", expectedDatabaseName);
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
            var connectionString = String.Format(@"Server=.;Database={0};Integrated Security=True;Encrypt=False", expectedDatabaseName);
			var dataAccess = new LoggingDataAccess(connectionString);
            var sut = new Logger(dataAccess, Models.Category.VIEW);

            // Act
            var actual = await sut.Log(Models.LogLevel.INFO, "test", "test");

            // Assert
            Assert.IsTrue(actual.IsSuccessful);

			var dbChecker = new SelectDataAccess(connectionString);
			var dbCheck = await dataAccess.SelectLogs(new List<string>() { "*" }, new Dictionary<string, object> { { "message", "test" } });

			Assert.IsTrue(dbCheck.IsSuccessful);
        }
	}
}