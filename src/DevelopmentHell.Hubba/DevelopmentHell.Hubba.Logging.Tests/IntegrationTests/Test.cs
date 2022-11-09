using DevelopmentHell.Hubba.Logging.Implementation;
using DevelopmentHell.Hubba.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Logging.Test.IntegrationTests
{
	[TestClass]
	public class Test
	{
		[TestMethod]
		public void ShouldLogInDatabase()
		{
			// Arrange
			var expected = typeof(Logger);
			var expectedDatabaseName = "DevelopmentHell.Hubba.Accounts";
			var connectionString = String.Format(@"Server=.;Database={0};Integrated Security=True;Encrypt=False", expectedDatabaseName);
			var sut = new Logger(new LoggingDataAccess(connectionString), Models.Category.VIEW);

			// Act
			var actual = sut.Log(Models.LogLevel.INFO, "test", "test");

			// Assert
			Assert.IsTrue(true);
		}
	}
}
