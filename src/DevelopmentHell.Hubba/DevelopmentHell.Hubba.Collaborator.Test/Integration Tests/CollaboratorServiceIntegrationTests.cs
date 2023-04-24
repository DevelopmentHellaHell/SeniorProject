using Development.Hubba.JWTHandler.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.Collaborator.Manager.Abstractions;
using DevelopmentHell.Hubba.Collaborator.Manager.Implementations;
using DevelopmentHell.Hubba.Collaborator.Service.Abstractions;
using DevelopmentHell.Hubba.Collaborator.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Files.Service.Abstractions;
using DevelopmentHell.Hubba.Files.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using System.Configuration;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Authentication.Service.Abstractions;

namespace DevelopmentHell.Hubba.Collaborator.Test.Integration_Tests
{

    [TestClass]
    public class CollaboratorServiceIntegrationTests
    {
        private string _usersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        private string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;

        private readonly IUserAccountDataAccess _userAccountDataAccess;
        private readonly IRegistrationService _registrationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICollaboratorManager _collaboratorManager;
        private readonly ICollaboratorService _collaboratorService;
        private readonly ICollaboratorsDataAccess _collaboratorsDataAccess;
        private readonly ICollaboratorFileDataAccess _collaboratorFileDataAccess;
        private readonly ITestingService _testingService;
        private readonly IValidationService _validationService;
        private readonly IFileService _fileService;

        public CollaboratorServiceIntegrationTests()
        {
            _userAccountDataAccess = new UserAccountDataAccess(_usersConnectionString, _userAccountsTable);
            _validationService = new ValidationService();
            ICryptographyService cryptographyService = new CryptographyService(ConfigurationManager.AppSettings["CryptographyKey"]!);
            IJWTHandlerService jwtHandlerService = new JWTHandlerService(
                _jwtKey
            );
            ILoggerService loggerService = new LoggerService(
                new LoggerDataAccess(_logsConnectionString, _logsTable)
            );
            _authorizationService = new AuthorizationService(
                _userAccountDataAccess,
                jwtHandlerService,
                loggerService
            );
            _testingService = new TestingService(
                _jwtKey,
                new TestsDataAccess()
            );
            _collaboratorsDataAccess = new CollaboratorsDataAccess(
                    ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
                    ConfigurationManager.AppSettings["CollaboratorsTable"]!);
            _collaboratorFileDataAccess = new CollaboratorFileDataAccess(
                    ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
                    ConfigurationManager.AppSettings["CollaboratorFilesTable"]!);
            _collaboratorService = new CollaboratorService(
                _collaboratorsDataAccess,
                _collaboratorFileDataAccess,
                new CollaboratorFileJunctionDataAccess(
                    ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
                    ConfigurationManager.AppSettings["CollaboratorFileJunctionTable"]!),
                new CollaboratorUserVoteDataAccess(
                    ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
                    ConfigurationManager.AppSettings["CollaboratorUserVotesTable"]!),
                new FTPFileService(
                            ConfigurationManager.AppSettings["FTPServer"]!,
                            ConfigurationManager.AppSettings["FTPUsername"]!,
                            ConfigurationManager.AppSettings["FTPPassword"]!,
                            loggerService
                        ),
                loggerService,
                _validationService
                );
            _registrationService = new RegistrationService(
                _userAccountDataAccess,
                cryptographyService,
                _validationService,
                loggerService
            );
            _fileService = new FTPFileService(
                ConfigurationManager.AppSettings["FTPServer"]!,
                ConfigurationManager.AppSettings["FTPUsername"]!,
                ConfigurationManager.AppSettings["FTPPassword"]!,
                loggerService);

        }
        [TestInitialize]
        public async Task Setup()
        {
            await _testingService.DeleteAllRecords().ConfigureAwait(false);
        }

        [TestMethod]
        public void ShouldInstansiateCtor()
        {
            Assert.IsNotNull(_collaboratorManager);
        }
    } 
}