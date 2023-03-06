
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Validation.Service.Implementations;

namespace DevelopmentHell.Hubba.Validation.Test
{
    [TestClass]
	public class UnitTests
	{
		/*
        * Success Case
        * Goal: Validate a syntactically correct email 
        * Process: Call the ValidateEmail method on a syntactically correct email
        */
		[TestMethod]
		public void Test01()
		{
			// Arrange
			string email = "syntactically@correct.com";
			Result expected = new();
			Result actual = new();
			expected.IsSuccessful = true;

			// Act
			actual = ValidationService.ValidateEmail(email);

			// Assert
			Assert.IsTrue(actual is not null);
			Assert.IsTrue(expected.IsSuccessful == actual.IsSuccessful);
		}

		/*
        * Failure Case
        * Goal: Invalidate a syntactically incorrect email with uppercase letters
        * Process: Call the ValidateEmail method on a syntactically incorrect email
        */
		[TestMethod]
		public void Test02()
		{
			// Arrange
			string email = "NOTsyntactically@correct.com";
			Result expected = new();
			Result actual = new();
			expected.IsSuccessful = false;
			expected.ErrorMessage = "Email provided is invalid. Retry or contact admin.";

			// Act
			actual = ValidationService.ValidateEmail(email);

			// Assert
			Assert.IsTrue(actual is not null);
			Assert.IsTrue(expected.IsSuccessful == actual.IsSuccessful);
			Assert.IsTrue(expected.ErrorMessage == actual.ErrorMessage);
		}

		/*
        * Failure Case
        * Goal: Invalidate a syntactically incorrect email with special characters
        * Process: Call the ValidateEmail method on a syntactically incorrect email
        */
		[TestMethod]
		public void Test03()
		{
			// Arrange
			string email = "$s!yntactically@correct.com";
			Result expected = new();
			Result actual = new();
			expected.IsSuccessful = false;
			expected.ErrorMessage = "Email provided is invalid. Retry or contact admin.";

			// Act
			actual = ValidationService.ValidateEmail(email);

			// Assert
			Assert.IsTrue(actual is not null);
			Assert.IsTrue(expected.IsSuccessful == actual.IsSuccessful);
			Assert.IsTrue(expected.ErrorMessage == actual.ErrorMessage);
		}

		/*
        * Success Case
        * Goal: Validate a syntactically correct password
        * Process: Call the ValidateEmail method on a syntactically correct password
        */
		[TestMethod]
		public void Test04()
		{
			// Arrange
			string password = "5up3r5t0ngP455w0rd";
			Result expected = new();
			Result actual = new();
			expected.IsSuccessful = true;

			// Act
			actual = ValidationService.ValidatePassword(password);

			// Assert
			Assert.IsTrue(actual is not null);
			Assert.IsTrue(expected.IsSuccessful == actual.IsSuccessful);
		}

		/*
        * Failure Case
        * Goal: Validate a syntactically incorrect password with illegal special characters
        * Process: Call the ValidateEmail method on a syntactically correct password
        */
		[TestMethod]
		public void Test05()
		{
			// Arrange
			string password = "$5up3r5t0ngP455w0rd";
			Result expected = new();
			Result actual = new();
			expected.IsSuccessful = false;
			expected.ErrorMessage = "Password provided is invalid. Retry or contact admin.";

			// Act
			actual = ValidationService.ValidatePassword(password);

			// Assert
			Assert.IsTrue(actual is not null);
			Assert.IsTrue(expected.IsSuccessful == actual.IsSuccessful);
			Assert.IsTrue(expected.ErrorMessage == actual.ErrorMessage);
		}

		/*
        * Failure Case
        * Goal: Validate a syntactically incorrect password with less than 8 characters
        * Process: Call the ValidateEmail method on a syntactically correct password
        */
		[TestMethod]
		public void Test06()
		{
			// Arrange
			string password = "1234567";
			Result expected = new();
			Result actual = new();
			expected.IsSuccessful = false;
			expected.ErrorMessage = "Password provided is invalid. Retry or contact admin.";

			// Act
			actual = ValidationService.ValidatePassword(password);

			// Assert
			Assert.IsTrue(actual is not null);
			Assert.IsTrue(expected.IsSuccessful == actual.IsSuccessful);
			Assert.IsTrue(expected.ErrorMessage == actual.ErrorMessage);
		}
	}
}