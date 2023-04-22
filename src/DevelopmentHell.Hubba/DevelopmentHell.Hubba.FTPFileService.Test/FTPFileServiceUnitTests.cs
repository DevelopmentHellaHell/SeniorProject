using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Files.Service.Implementations;
using DevelopmentHell.Hubba.Files.Service.Abstractions;
using System.Configuration;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.StaticFiles;

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

        private async Task AsyncSetup()
        {
            await _fileService.DeleteDir(dirPath);
        }

        [TestCleanup]
        public void Setup()
        {
            _ = AsyncSetup();
        }
        [TestMethod]
        public async Task CreateDir_UploadFile_GetFileReference()
        {
            await _fileService.DeleteDir(dirPath);

            var dirResult = await _fileService.CreateDir(dirPath);
            Assert.IsTrue(dirResult.IsSuccessful);

            Thread.Sleep(100);

            var result = await _fileService.UploadFile(dirPath, fileNameData[0].Item1, fileNameData[0].Item2).ConfigureAwait(false);
            Assert.IsTrue(result.IsSuccessful);

            Thread.Sleep(100);

            var testResult = await _fileService.GetFileReference(dirPath + "/" + fileNameData[0].Item1).ConfigureAwait(false);
            Assert.IsTrue(testResult.IsSuccessful);
            Assert.IsTrue(testResult.Payload == $"http://{_ftpServer}/{dirPath}/{fileNameData[0].Item1}");
        }
        [TestMethod]
        public async Task UploadDir()
        {
            await _fileService.DeleteDir(dirPath);

            var dirResult = await _fileService.CreateDir(dirPath);
            Assert.IsTrue(dirResult.IsSuccessful);

            Thread.Sleep(100);

            var result = await _fileService.UploadDir(dirPath, fileNameData);
            Assert.IsTrue(result.IsSuccessful);

            Thread.Sleep(100);

            var testResult = await _fileService.GetFileReference(dirPath + "/" + fileNameData[0].Item1).ConfigureAwait(false);
            Assert.IsTrue(testResult.IsSuccessful);
            Assert.IsTrue(testResult.Payload == $"http://{_ftpServer}/{dirPath}/{fileNameData[0].Item1}");

            Thread.Sleep(100);

            testResult = await _fileService.GetFileReference(dirPath + "/" + fileNameData[1].Item1).ConfigureAwait(false);
            Assert.IsTrue(testResult.IsSuccessful);
            Assert.IsTrue(testResult.Payload == $"http://{_ftpServer}/{dirPath}/{fileNameData[1].Item1}");
        }
        [TestMethod]
        public async Task DeleteFile()
        {
            await _fileService.DeleteDir(dirPath);

            var dirResult = await _fileService.CreateDir(dirPath);
            Assert.IsTrue(dirResult.IsSuccessful);

            Thread.Sleep(100);

            var result = await _fileService.UploadFile(dirPath, fileNameData[0].Item1, fileNameData[0].Item2).ConfigureAwait(false);
            Assert.IsTrue(result.IsSuccessful);

            Thread.Sleep(100);

            var deleteResult = await _fileService.DeleteFile(dirPath + "/" + fileNameData[0].Item1);
            Assert.IsTrue(deleteResult.IsSuccessful, deleteResult.ErrorMessage);

            Thread.Sleep(100);

            var testResult = await _fileService.GetFileReference(dirPath + "/" + fileNameData[0].Item1).ConfigureAwait(false);
            Assert.IsTrue(!testResult.IsSuccessful);
        }
        [TestMethod]
        public async Task DeleteDir()
        {
            await _fileService.DeleteDir(dirPath);

            var dirResult = await _fileService.CreateDir(dirPath);
            Assert.IsTrue(dirResult.IsSuccessful);

            Thread.Sleep(100);

            var deleteResult = await _fileService.DeleteDir(dirPath);
            Assert.IsTrue(deleteResult.IsSuccessful);

            Thread.Sleep(100);

            var testResult = await _fileService.GetFileReference(dirPath).ConfigureAwait(false);
            Assert.IsTrue(!testResult.IsSuccessful);
        }
        [TestMethod]
        public async Task GetFilesInDir()
        {
            await _fileService.DeleteDir(dirPath);

            var dirResult = await _fileService.CreateDir(dirPath);
            Assert.IsTrue(dirResult.IsSuccessful);

            Thread.Sleep(100);

            var result = await _fileService.UploadDir(dirPath, fileNameData);
            Assert.IsTrue(result.IsSuccessful);

            Thread.Sleep(100);


            var testResult = await _fileService.GetFilesInDir(dirPath).ConfigureAwait(false);
            Assert.IsTrue(testResult.IsSuccessful);

            HashSet<string> files = new HashSet<string>()
            {
                 $"http://{_ftpServer}/{dirPath}/{fileNameData[0].Item1}",
                 $"http://{_ftpServer}/{dirPath}/{fileNameData[1].Item1}"
            }
            ;
            Assert.IsTrue(files.Contains(testResult.Payload![0]));
            files.Remove(testResult.Payload![0]);
            Assert.IsTrue(files.Contains(testResult.Payload![1]));
        }
        [TestMethod]
        public async Task CreateDir_UploadFile_GetFileReference_UsingIFormFile()
        {
            await _fileService.DeleteDir(dirPath);
            IFormFile file = CreateFormFileFromFilePath("C:\\Users\\NZXT ASRock\\Documents\\Senior Project\\SeniorProject\\src\\DevelopmentHell.Hubba\\Images\\rayquaza0.png");
            var dirResult = await _fileService.CreateDir(dirPath);
            Assert.IsTrue(dirResult.IsSuccessful);


            Thread.Sleep(100);

            var result = await _fileService.UploadIFormFile(dirPath, "Sick_ass_shiny_rayquaza_wtf.png", file).ConfigureAwait(false);
            Assert.IsTrue(result.IsSuccessful);

            Thread.Sleep(100);

            var testResult = await _fileService.GetFileReference(dirPath + "/" + "Sick_ass_shiny_rayquaza_wtf.png").ConfigureAwait(false);
            Assert.IsTrue(testResult.IsSuccessful);
            Assert.IsTrue(testResult.Payload == $"ftp://{_ftpServer}/{dirPath}/Sick_ass_shiny_rayquaza_wtf.png");
        }






        // Making an IFormFile for testing, it streams the location of the file into a file object
        public IFormFile CreateFormFileFromFilePath(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var stream = new FileStream(filePath, FileMode.Open);
            var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = GetContentType(fileName)
            };

            return formFile;
        }
        private string GetContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}