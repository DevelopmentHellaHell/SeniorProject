using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using System.Security.Principal;

namespace DevelopmentHell.Hubba.Authorization.Test
{
	[TestClass]
	public class UnitTests
	{
		/* Success Case
		 * Goal: Authorize User if User have Verified role
		 * Process: Take existing principal and authorize user
		 */
		[TestMethod]
		public void Test01()
		{
			// Arrange
			var authorizationService = new AuthorizationService();
			var identity = new GenericIdentity("1");
			var principal = new GenericPrincipal(identity, new string[] { "VerifiedUser" });
			var expected = new Result()
			{
				IsSuccessful = true,
			};

			// Actual
			var actual = authorizationService.authorize(principal, new string[] { "VerifiedUser" });

			// Assert
			Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
		}

		/* Failure Case
		 * Goal: Do not authorize User if User does not have VerifiedUser role
		 * Process: Take existing principal and authorize user
		 */
		[TestMethod]
		public void Test02()
		{
			// Arrange
			var authorizationService = new AuthorizationService();
			var identity = new GenericIdentity("1");
			var principal = new GenericPrincipal(identity, null);
			var expected = new Result()
			{
				IsSuccessful = false,
			};

			// Actual
			var actual = authorizationService.authorize(principal, new string[] { "VerifiedUser" });

			// Assert
			Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
		}

		/* Success Case
		 * Goal: Authorize User if User is a Anonymous User and no required roles
		 * Process: Take existing principal and authorize user
		 */
		[TestMethod]
		public void Test03()
		{
			// Arrange
			var authorizationService = new AuthorizationService();
			var expected = new Result()
			{
				IsSuccessful = true,
			};

			// Actual
			var actual = authorizationService.authorize(null);

			// Assert
			Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
		}

		/* Failure Case
		 * Goal: Do not authorize User if Anonymous User does not have VerifiedUser role 
		 * Process: Take existing principal and authorize user
		 */
		[TestMethod]
		public void Test04()
		{
			// Arrange
			var authorizationService = new AuthorizationService();
			var expected = new Result()
			{
				IsSuccessful = false,
			};

			// Actual
			var actual = authorizationService.authorize(null, new string[] { "VerifiedUser" });

			// Assert
			Assert.IsTrue(actual.IsSuccessful == expected.IsSuccessful);
		}
	}
}