using DevelopmentHell.Hubba.Logging.Implementation;
using DevelopmentHell.Hubba.SqlDataAccess;
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
	}
}