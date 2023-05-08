using System.Configuration;
using System.Diagnostics;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.Tests;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using Microsoft.Data.SqlClient;

namespace DevelopmentHell.Hubba.Logging.Test
{
    [TestClass]
    public class IntegrationTests
    {
        private readonly Models.LogLevel[] failureLevels = { Models.LogLevel.WARNING, Models.LogLevel.ERROR };
        private readonly Models.LogLevel[] successLevels = { Models.LogLevel.DEBUG, Models.LogLevel.INFO };

        private static string connectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private LoggerDataAccess dataAccess = new LoggerDataAccess(connectionString, ConfigurationManager.AppSettings["LogsTable"]!);

        private string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;

        private readonly ITestingService _testingService;

        public IntegrationTests()
        {
            _testingService = new TestingService(
                new TestsDataAccess()
            );
        }

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
                var actual = sut.Log(logLevel, category, message, userName);
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
                var actual = sut.Log(logLevel, category, message, userName);
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
                var actual = sut.Log(logLevel, category, message, userName);
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
                var actual = sut.Log(logLevel, category, message, userName);
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
            var result = sut.Log(logLevel, category, message, userName);
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
                actual = sut.Log(logLevel, category, message, userName);

                (e as AutoResetEvent)!.Set();
            }, ev);

            // Assert
            Assert.IsTrue(ev.WaitOne(1000));
            Assert.IsTrue(actual.IsSuccessful);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await _testingService.DeleteDatabaseRecords(Databases.LOGS).ConfigureAwait(false);
        }
    }
}