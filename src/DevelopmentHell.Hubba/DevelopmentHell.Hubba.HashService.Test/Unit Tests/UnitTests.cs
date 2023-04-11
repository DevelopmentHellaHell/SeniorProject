using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.HashService.Test
{
	[TestClass]
	public class UnitTests
	{
		/*
         * Success Case
         * Goal: Hash a string using SHA 256
         * Process: Call the HashString method with a known message and salt, then compare it with an expected hash 
         */
		[TestMethod]
		public void Test01()
		{

			// Arrange
			string message = "message";
			string salt = "salt";
			string actual = "";
			string expected = "1/cn6/TcXH6mG+O84C26BfEiijkFVSB689Lv+NVTbxa2OytsWFasSRfivxzp7qGvoOKhASlPtlpTfMbPsIr6bQ==";

			// Act
			Result<HashData> hashedData = Cryptography.Service.HashService.HashString(message, salt);
			if (hashedData.Payload is not null)
				if (hashedData.Payload.Hash is not null)
					actual = Convert.ToBase64String(hashedData.Payload.Hash);

			// Assert
			Assert.IsTrue(hashedData.Payload is not null);
			Assert.IsTrue(hashedData.Payload.Hash is not null);
			Assert.IsTrue(Convert.ToBase64String(hashedData.Payload.Hash).Equals(expected));
		}
	}
}