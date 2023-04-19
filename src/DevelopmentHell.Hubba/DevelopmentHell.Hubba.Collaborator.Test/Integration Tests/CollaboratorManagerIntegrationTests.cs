
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using System.Configuration;


namespace DevelopmentHell.Hubba.Collaborator.Test.Integration_Tests
{
    [TestClass]
    public class CollaboratorManagerIntegrationTests
    {
        private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;
        private readonly ITestingService _testingService;
        public CollaboratorManagerIntegrationTests()
        {
            _testingService = new TestingService(
                _jwtKey,
                new TestsDataAccess()
            );
        }
        [TestInitialize]
        public async Task Setup()
        {
            await _testingService.DeleteAllRecords().ConfigureAwait(false);
        }
    }
}
