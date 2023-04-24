using DevelopmentHell.Hubba.Collaborator.Service.Abstractions;
using DevelopmentHell.Hubba.Collaborator.Service.Implementations;
using DevelopmentHell.Hubba.Files.Service.Abstractions;
using DevelopmentHell.Hubba.Files.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementations;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Collaborator.Test.Unit_Tests
{
    [TestClass]
    public class CollaboratorDataAccessUnitTests
    {
        private InsertDataAccess _insertDataAccess;
        private SelectDataAccess _selectDataAccess;
        private DeleteDataAccess _deleteDataAccess;
        private UpdateDataAccess _updateDataAccess;
        private readonly ITestingService _testingService;
        private readonly IFileService _fileService;

        private readonly string _fileName = "FileName";
        private readonly string _fileType = "FileType";
        private readonly string _fileSize = "FileSize";
        private readonly string _fileUrl = "FileUrl";
        private readonly string _ownerId = "OwnerId";
        private readonly string _lastModifiedUser = "LastModifiedUser";
        private readonly string _createDate = "CreateDate";
        private readonly string _fileId = "FileId";
        private readonly string _pfpFile = "PfpFile";
        private readonly string _updateDate = "UpdateDate";
        private readonly string _usersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"]!;
        private readonly string _userAccountsTable = ConfigurationManager.AppSettings["UserAccountsTable"]!;
        private readonly string _logsConnectionString = ConfigurationManager.AppSettings["LogsConnectionString"]!;
        private readonly string _logsTable = ConfigurationManager.AppSettings["LogsTable"]!;
        private readonly string _jwtKey = ConfigurationManager.AppSettings["JwtKey"]!;
        private ICollaboratorsDataAccess _collaboratorsDataAccess;
        private ICollaboratorFileDataAccess _collaboratorFileDataAccess;
        private ICollaboratorFileJunctionDataAccess _collaboratorFileJunctionDataAccess;
        private ICollaboratorUserVoteDataAccess _collaboratorUserVoteDataAccess;
        private ICollaboratorService _collaboratorService;

        public CollaboratorDataAccessUnitTests()
        {

            ValidationService validationService = new ValidationService();
            ILoggerService loggerService = new LoggerService(
                    new LoggerDataAccess(_logsConnectionString, _logsTable)
                );
            _fileService = new FTPFileService(
                ConfigurationManager.AppSettings["FTPServer"]!,
                ConfigurationManager.AppSettings["FTPUsername"]!,
                ConfigurationManager.AppSettings["FTPPassword"]!,
                loggerService);
            _collaboratorsDataAccess = new CollaboratorsDataAccess(
                ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
                ConfigurationManager.AppSettings["CollaboratorsTable"]!
            );
            _collaboratorFileDataAccess = new CollaboratorFileDataAccess(
                ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
                ConfigurationManager.AppSettings["CollaboratorFilesTable"]!
            );
            _collaboratorFileJunctionDataAccess = new CollaboratorFileJunctionDataAccess(
                ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
                ConfigurationManager.AppSettings["CollaboratorFileJunctionTable"]!
            );
            _collaboratorUserVoteDataAccess = new CollaboratorUserVoteDataAccess(
                ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
                ConfigurationManager.AppSettings["CollaboratorUserVotesTable"]!
            );
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
                    validationService
                );
        }

        [TestInitialize]
        public async Task Setup()
        {
            await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.COLLABORATOR_PROFILES).ConfigureAwait(false);
            await _fileService.DeleteDir("/Collaborators").ConfigureAwait(false);
        }






        [TestCleanup]
        public async Task Cleanup()
        {
            await _testingService.DeleteDatabaseRecords(Models.Tests.Databases.COLLABORATOR_PROFILES).ConfigureAwait(false);
            await _fileService.DeleteDir("/Collaborators").ConfigureAwait(false);
        }




    }
}
