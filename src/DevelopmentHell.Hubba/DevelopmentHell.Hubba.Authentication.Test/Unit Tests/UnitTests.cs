using DevelopmentHell.Hubba.Authentication.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using System.Security.Principal;

namespace DevelopmentHell.Hubba.Authentication.Test
{
	[TestClass]
	public class UnitTests
	{
		/*
         * Success Case
         * Goal: Instantiate a AuthenticationService object using the constructor
         * Process: Create an AuthenticationService with dummy connectionstrings and table names
         */
		[TestMethod]
		public void AuthNService_DefaultConstructor_Instantiate_ReturnAnObj()
		{
			// Arrange
			string dummyConnectionString = "";
			string dummyTable = "";

			// Act
			var disconnectedAuthService = new AuthenticationService(
				new UserAccountDataAccess(dummyConnectionString, dummyTable),
				new LoggerService(new LoggerDataAccess(dummyConnectionString, dummyTable))
				);


			// Assert
			Assert.IsTrue(disconnectedAuthService is not null);
			Assert.IsTrue(disconnectedAuthService.GetType() == typeof(AuthenticationService));
		}

		/* 
		 * Success Case
		 * Goal: Assign the GenericPrincipal principal to the current thread
		 * Process: Create a dummy AuthService, invoke the createSession method
		 */
		[TestMethod]
		public void AuthNService_CreateSession_AccountID_ReturnVerifiedUserPrincipal()
		{
			// Arrange
			int actualId = 1;
			string expectedRole = "VerifiedUser";
			string dummyConnectionString = "";
			string dummyTable = "";
			var disconnectedAuthService = new AuthenticationService(
				new UserAccountDataAccess(dummyConnectionString, dummyTable),
				new LoggerService(new LoggerDataAccess(dummyConnectionString, dummyTable))
				);

			// Act
			Result<GenericPrincipal> actual = disconnectedAuthService.CreateSession(actualId);

			// Assert
			Assert.IsTrue(actual is not null);
			Assert.IsTrue(actual.Payload is not null);
			Assert.IsTrue(actual.Payload.Identity is not null);
			Assert.IsTrue(actual.Payload.Identity.Name is not null);
			Assert.IsTrue(actual.Payload.Identity.Name.Equals(1.ToString()));
			Assert.IsTrue(actual.Payload.IsInRole(expectedRole));
			Assert.IsTrue(Thread.CurrentPrincipal is not null);
			Assert.IsTrue(Thread.CurrentPrincipal.IsInRole(expectedRole));
		}

        /* 
		 * Success Case
		 * Goal: Update the GenericPrincipal principal to the current thread from VerifiedUser to DefaultUser
		 * Process: Create a dummy AuthService, invoke the EndSession method
		 */
        [TestMethod]
        public void AuthNService_EndSession_AccountID_ReturnDefaultUserPrincipal()
        {
            // Arrange
            int actualId = 1;
            string expectedRole = "DefaultUser";
            string dummyConnectionString = "";
            string dummyTable = "";
            var disconnectedAuthService = new AuthenticationService(
                new UserAccountDataAccess(dummyConnectionString, dummyTable),
                new LoggerService(new LoggerDataAccess(dummyConnectionString, dummyTable))
                );

            // Act
            Result<GenericPrincipal> actual = disconnectedAuthService.EndSession(actualId);

            // Assert
            Assert.IsTrue(actual is not null);
            Assert.IsTrue(actual.Payload is not null);
            Assert.IsTrue(actual.Payload.Identity is not null);
            Assert.IsTrue(actual.Payload.Identity.Name is not null);
            Assert.IsTrue(actual.Payload.Identity.Name.Equals(1.ToString()));
            Assert.IsTrue(actual.Payload.IsInRole(expectedRole));
            Assert.IsTrue(Thread.CurrentPrincipal is not null);
            Assert.IsTrue(Thread.CurrentPrincipal.IsInRole(expectedRole));
        }
    }
}