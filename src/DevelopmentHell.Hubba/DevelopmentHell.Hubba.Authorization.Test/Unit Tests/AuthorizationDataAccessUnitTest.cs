using DevelopmentHell.Hubba.SqlDataAccess;
using System.Configuration;

namespace DevelopmentHell.Hubba.Authorization.Test
{
	[TestClass]
	public class AuthorizationDataAccessUnitTest
	{
		private string _UsersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
		private string _UserRolesTable = ConfigurationManager.AppSettings["UserRolesTable"]!;

		private string _LogsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
		private string _LogsTable = ConfigurationManager.AppSettings["LogsTable"]!;

		[TestMethod]
		public void ShouldCreateNewInstanceWithParameterCtor()
		{
			// Arrange
			var expected = typeof(AuthorizationDataAccess);

			// Act
			var actual = new AuthorizationDataAccess(_UsersConnectionString, _UserRolesTable);

			// Assert
			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.GetType() == expected);

		}

		public void ShouldSetRoleForUser()
		{
			// Arrange

			// Act

			// Assert
			
		}
	}
}