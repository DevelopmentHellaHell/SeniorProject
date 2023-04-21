using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Files.Service.Implementations;
using DevelopmentHell.Hubba.Files.Service.Abstractions;
using System.Configuration;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using System.Text;

namespace DevelopmentHell.Hubba.FTPFileService.Test
{
    [TestClass]
    public class FTPFileServiceUnitTests
    {
        IFileService _fileService;
        string dirPath;
        List<Tuple<string, byte[]>> fileNameData;

        string _ftpServer;
        string _ftpUsername;
        string _ftpPassword;
        string _now;

        public FTPFileServiceUnitTests()
        {
            _now = DateTime.UtcNow.Millisecond.ToString();
            _ftpServer = ConfigurationManager.AppSettings["FTPServer"]!;
            _ftpUsername = ConfigurationManager.AppSettings["FTPUsername"]!;
            _ftpPassword = ConfigurationManager.AppSettings["FTPPassword"]!;
            fileNameData = new List<Tuple<string, byte[]>>()
            {
                new Tuple<string, byte[]>($"{_now}_file1.txt", Encoding.ASCII.GetBytes("Text For File 1")),
                new Tuple<string, byte[]>($"{_now}_file2.txt", Encoding.ASCII.GetBytes("Text For File 2"))
            };

            dirPath = "Testing/TestDir"; 
            _fileService = new Files.Service.Implementations.FTPFileService(
                _ftpServer,
                _ftpUsername,
                _ftpPassword,
                new LoggerService
                (
                    new LoggerDataAccess
                    (
                        ConfigurationManager.AppSettings["LogsConnectionString"]!,
                        ConfigurationManager.AppSettings["LogsTable"]!
                    )
                )
            );
        }
        [TestInitialize]
        public void Setup()
        {

        }
        [TestMethod]
        public async Task CreateDir_UploadFile_GetFileReference()
        {
            //var dirResult = await _fileService.CreateDir(dirPath);
            //Assert.IsTrue(dirResult.IsSuccessful);

            var result = await _fileService.UploadFile(dirPath, fileNameData[0].Item1, fileNameData[0].Item2).ConfigureAwait(false);
            Assert.IsTrue(result.IsSuccessful);

            var testResult = await _fileService.GetFileReference(dirPath + "/" + fileNameData[0].Item1).ConfigureAwait(false);
            Assert.IsTrue(testResult.IsSuccessful);
            Assert.IsTrue(testResult.Payload == $"ftp://{_ftpServer}/{dirPath}/{fileNameData[0].Item1}");
        }
        [TestMethod]
        public async Task UploadDir()
        {
            var dirResult = await _fileService.CreateDir(dirPath);
            Assert.IsTrue(dirResult.IsSuccessful);

            var result = await _fileService.UploadDir(dirPath, fileNameData);
            Assert.IsTrue(result.IsSuccessful);

            var testResult = await _fileService.GetFileReference(dirPath + "/" + fileNameData[0].Item1).ConfigureAwait(false);
            Assert.IsTrue(testResult.IsSuccessful);
            Assert.IsTrue(testResult.Payload == $"ftp://{_ftpServer}/{dirPath}/{fileNameData[0].Item1}");

            testResult = await _fileService.GetFileReference(dirPath + fileNameData[1].Item1).ConfigureAwait(false);
            Assert.IsTrue(testResult.IsSuccessful);
            Assert.IsTrue(testResult.Payload == $"ftp://{_ftpServer}/{dirPath}/{fileNameData[1].Item1}");
        }
        [TestMethod]
        public async Task DeleteFile()
        {
            var dirResult = await _fileService.CreateDir(dirPath);
            Assert.IsTrue(dirResult.IsSuccessful);

            var result = await _fileService.UploadFile(dirPath, fileNameData[0].Item1, fileNameData[0].Item2).ConfigureAwait(false);
            Assert.IsTrue(result.IsSuccessful);

            var deleteResult = await _fileService.DeleteFile(dirPath + fileNameData[0].Item1);
            Assert.IsTrue(deleteResult.IsSuccessful);

            var testResult = await _fileService.GetFileReference(dirPath + "/" + fileNameData[0].Item1).ConfigureAwait(false);
            Assert.IsTrue(!testResult.IsSuccessful);
        }
        [TestMethod]
        public async Task DeleteDir()
        {
            var dirResult = await _fileService.CreateDir(dirPath);
            Assert.IsTrue(dirResult.IsSuccessful);

            var result = await _fileService.UploadFile(dirPath, fileNameData[0].Item1, fileNameData[0].Item2).ConfigureAwait(false);
            Assert.IsTrue(result.IsSuccessful);

            var deleteResult = await _fileService.DeleteDir(dirPath);
            Assert.IsTrue(deleteResult.IsSuccessful);

            var testResult = await _fileService.GetFileReference(dirPath + "/" + fileNameData[0].Item1).ConfigureAwait(false);
            Assert.IsTrue(!testResult.IsSuccessful);
        }
        [TestMethod]
        public async Task GetFilesInDir()
        {
            var dirResult = await _fileService.CreateDir(dirPath);
            Assert.IsTrue(dirResult.IsSuccessful);

            var result = await _fileService.UploadDir(dirPath, fileNameData);
            Assert.IsTrue(result.IsSuccessful);


            var testResult = await _fileService.GetFilesInDir(dirPath).ConfigureAwait(false);
            Assert.IsTrue(testResult.IsSuccessful);
            Assert.IsTrue(testResult.Payload![0] == $"ftp://{_ftpServer}/{dirPath}/{fileNameData[0].Item1}");
            Assert.IsTrue(testResult.Payload![1] == $"ftp://{_ftpServer}/{dirPath}/{fileNameData[1].Item1}");
        }
    }
}