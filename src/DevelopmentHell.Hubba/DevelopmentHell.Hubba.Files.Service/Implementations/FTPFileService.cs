using System.Net;
using DevelopmentHell.Hubba.Files.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using System.Net.Http.Headers;
using Azure;
using Azure.Core;

#pragma warning disable SYSLIB0014 // Type or member is obsolete
namespace DevelopmentHell.Hubba.Files.Service.Implementations
{
    public class FTPFileService : IFileService
    {
        private static FtpWebRequest? _request;
        private readonly string _ftpServer;
        private readonly string _ftpUsername;
        private readonly string _ftpPassword;
        private readonly HttpClient _httpClient;
        private readonly ILoggerService _loggerService;
        public FTPFileService(string ftpServer, string ftpUsername, string ftpPassword, ILoggerService loggerService)
        {
            _ftpServer = "ftp://"+ftpServer;
            _ftpUsername = ftpUsername;
            _ftpPassword = ftpPassword;
            _httpClient = new(
                new HttpClientHandler
                {
                    Credentials = new NetworkCredential(_ftpUsername, _ftpPassword)
                }
                );
            _loggerService = loggerService;
        }

        private Result Bye()
        {
            ServicePointManager.FindServicePoint(new Uri(_ftpServer)).ConnectionLeaseTimeout = 0;
            _request = (FtpWebRequest)WebRequest.Create(_ftpServer);
            _request.Method = "QUIT";
            using (FtpWebResponse response = (FtpWebResponse)_request.GetResponse())
            {
                Console.WriteLine($"Status: {response.StatusDescription}");
                if (response.StatusCode != FtpStatusCode.ConnectionClosed)
                {
                    return Result.Failure("Problem in closing connection");
                }
            }
            return Result.Success();
        }

        public async Task<Result> CreateDir(string dirPath)
        {
            // Split the directory path into individual directory names
            var dirs = dirPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            // Loop through the directories, creating each one if it doesn't exist
            var currentPath = "";
            foreach (var dir in dirs)
            {
                var requestUri = new Uri($"{_ftpServer}/{currentPath}");
                ServicePointManager.FindServicePoint(requestUri).ConnectionLeaseTimeout = 0;
                currentPath += $"/{dir}";
                _request = (FtpWebRequest)WebRequest.Create(requestUri);
                _request.Method = WebRequestMethods.Ftp.MakeDirectory;
                _request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
                //_request.KeepAlive = false;
                try
                {
                    using (var response = (FtpWebResponse)await _request.GetResponseAsync().ConfigureAwait(false))
                    {
                        response.Dispose();
                        response.Close();
                        if (response.StatusCode != FtpStatusCode.PathnameCreated && response.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable)
                        {
                            _loggerService.Log(LogLevel.WARNING, Category.DATA, "CreateDir: Directory could not be created", "FTPFileService");
                            return Result.Failure($"Directory inaccessible or does not exist", (int)response.StatusCode);
                        }
                    }
                }
                catch (WebException ex)
                {
                }
            }
            _request.Abort();
            return Result.Success();
        }

        private async Task<Result<string>> DeleteAllFilesInDir(string directoryPath)
        {
            var requestUri = new Uri($"{_ftpServer}/{directoryPath}");
            ServicePointManager.FindServicePoint(requestUri).ConnectionLeaseTimeout = 0;

            var request = (FtpWebRequest)WebRequest.Create(requestUri);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);

            try
            {
                using var response = (FtpWebResponse)await request.GetResponseAsync();
                using var stream = response.GetResponseStream();
                using var reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {
                    var fileName = reader.ReadLine();

                    // Skip directories
                    if (fileName.EndsWith("/") || fileName.EndsWith("\\"))
                    {
                        continue;
                    }

                    // Delete file
                    var fileRequestUri = new Uri($"{_ftpServer}/{directoryPath}/{fileName}");
                    var fileRequest = (FtpWebRequest)WebRequest.Create(fileRequestUri);
                    fileRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                    fileRequest.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);

                    using var fileResponse = (FtpWebResponse)await fileRequest.GetResponseAsync();
                    if (fileResponse.StatusCode != FtpStatusCode.FileActionOK)
                    {
                        _loggerService.Log(LogLevel.WARNING, Category.DATA, $"DeleteAllFilesInDirectory: Error deleting file {fileName}", "FTPFileService");
                        return new(Result.Failure($"Error deleting file {fileName}", (int)fileResponse.StatusCode));
                    }
                }

                return Result<string>.Success(requestUri.OriginalString);
            }
            catch (WebException ex)
            {
                _loggerService.Log(LogLevel.WARNING, Category.DATA, $"DeleteAllFilesInDirectory: {ex.Message}", "FTPFileService");
                return new(Result.Failure($"Error deleting files in directory", (int)ex.Status));
            }
        }


        private async Task<Result> DeleteDirRecursively(string directoryPath)
        {
            var requestUri = new Uri($"{_ftpServer}/{directoryPath}");
            ServicePointManager.FindServicePoint(requestUri).ConnectionLeaseTimeout = 0;

            //try
            //{
                // Delete all files in directory
                var deleteFilesResult = await DeleteAllFilesInDir(directoryPath);
                if (!deleteFilesResult.IsSuccessful)
                {
                    _loggerService.Log(LogLevel.WARNING, Category.DATA, $"DeleteDirectoryRecursively: {deleteFilesResult.ErrorMessage}", "FTPFileService");
                    return deleteFilesResult;
                }

                // Delete subdirectories recursively
                var request = (FtpWebRequest)WebRequest.Create(requestUri);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);

                using var response = (FtpWebResponse)await request.GetResponseAsync();
                using var stream = response.GetResponseStream();
                using var reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {
                    var fileDetails = reader.ReadLine();
                    var fileDetailsArray = fileDetails.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var fileType = fileDetailsArray[0][0];
                    var fileName = fileDetailsArray.Last();

                    // Skip parent and current directory
                    if (fileName == "." || fileName == "..")
                    {
                        continue;
                    }

                    if (fileType == 'd')
                    {
                        var deleteSubdirectoryResult = await DeleteDirRecursively($"{directoryPath}/{fileName}");
                        if (!deleteSubdirectoryResult.IsSuccessful)
                        {
                            _loggerService.Log(LogLevel.WARNING, Category.DATA, $"DeleteDirectoryRecursively: {deleteSubdirectoryResult.ErrorMessage}", "FTPFileService");
                            return deleteSubdirectoryResult;
                        }
                    }
                    else if (fileType == '-')
                    {
                        var fileRequestUri = new Uri($"{_ftpServer}/{directoryPath}/{fileName}");
                        var fileRequest = (FtpWebRequest)WebRequest.Create(fileRequestUri);
                        fileRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                        fileRequest.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);

                        using var fileResponse = (FtpWebResponse)await fileRequest.GetResponseAsync();
                        if (fileResponse.StatusCode != FtpStatusCode.FileActionOK)
                        {
                            _loggerService.Log(LogLevel.WARNING, Category.DATA, $"DeleteDirectoryRecursively: Error deleting file {fileName}", "FTPFileService");
                            return new(Result.Failure($"Error deleting file {fileName}", (int)fileResponse.StatusCode));
                        }
                    }
                }

                // Delete directory itself
                var deleteDirectoryResult = await DeleteDirOnly(directoryPath);
                if (!deleteDirectoryResult.IsSuccessful)
                {
                    _loggerService.Log(LogLevel.WARNING, Category.DATA, $"DeleteDirectoryRecursively: {deleteDirectoryResult.ErrorMessage}", "FTPFileService");
                    return deleteDirectoryResult;
                }

                return Result<string>.Success(requestUri.OriginalString);
            //}
            //catch (WebException ex)
            //{
            //    _loggerService.Log(LogLevel.WARNING, Category.DATA, $"DeleteDirectoryRecursively: {ex.Message}", "FTPFileService");
            //    return new(Result.Failure($"Error deleting directory and its contents", (int)ex.Status));
            //}
        }

        public async Task<Result> DeleteDirOnly(string dirPath)
        {
            var requestUri = new Uri($"{_ftpServer}/{dirPath}");
            ServicePointManager.FindServicePoint(requestUri).ConnectionLeaseTimeout = 0;
            _request = (FtpWebRequest)WebRequest.Create(requestUri);
            _request.Method = WebRequestMethods.Ftp.RemoveDirectory;
            _request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
            //_request.KeepAlive = false;

            try
            {
                using (var response = (FtpWebResponse)await _request.GetResponseAsync())
                {
                    response.Dispose();
                    response.Close();
                    return Result<string>.Success(requestUri.OriginalString);
                }
            }
            catch (WebException ex)
            {
                var response = ex.Response as FtpWebResponse;
                if (response!.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    _loggerService.Log(LogLevel.WARNING, Category.DATA, "DeleteDir: Directory inaccessible or does not exist", "FTPFileService");
                    return Result.Failure($"Directory inaccessible or does not exist", (int)response.StatusCode);
                }
                else
                {
                    _loggerService.Log(LogLevel.ERROR, Category.DATA, $"DeleteDir: Error deleting directory - {ex.Message}", "FTPFileService");
                    return Result.Failure($"Error deleting directory - {ex.Message}", (int)response.StatusCode);
                }
            }
        }

        public async Task<Result> DeleteDir(string dirPath)
        {
            return await DeleteDirRecursively(dirPath).ConfigureAwait(false);
        }

        public async Task<Result> DeleteFile(string filePath)
        {
            var requestUri = new Uri($"{_ftpServer}/{filePath}");
            ServicePointManager.FindServicePoint(requestUri).ConnectionLeaseTimeout = 0;

            _request = (FtpWebRequest)WebRequest.Create(requestUri);
            _request.Method = WebRequestMethods.Ftp.DeleteFile;
            _request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
            //_request.KeepAlive = false;

            try
            {
                // Send the DELETE request to the FTP server
                using var response = (FtpWebResponse)await _request.GetResponseAsync();
                response.Dispose();
                response.Close();

                if (response.StatusCode == FtpStatusCode.FileActionOK)
                {
                    return Result<string>.Success(requestUri.OriginalString);
                }
                else
                {
                    _loggerService.Log(LogLevel.WARNING, Category.DATA, $"DeleteFile: File inaccessible or does not exist", "FTPFileService");
                    return new(Result.Failure($"File inaccessible or does not exist", (int)response.StatusCode));
                }
            }
            catch (WebException ex)
            {
                _loggerService.Log(LogLevel.WARNING, Category.DATA, $"DeleteFile: {ex.Message}", "FTPFileService");
                return new(Result.Failure($"File inaccessible or does not exist", (int)ex.Status));
            }
        }

        public async Task<Result<string>> GetFileReference(string filePath)
        {
            var requestUri = new Uri($"{_ftpServer}/{filePath}");
            ServicePointManager.FindServicePoint(requestUri).ConnectionLeaseTimeout = 0;

            _request = (FtpWebRequest)WebRequest.Create(requestUri);
            _request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
            _request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
            //_request.KeepAlive = false;

            try
            {
                using var response = (FtpWebResponse)await _request.GetResponseAsync();
                response.Dispose();
                response.Close();
                return Result<string>.Success(requestUri.OriginalString);
            }
            catch (WebException ex)
            {
                var response = (FtpWebResponse)ex.Response!;
                _loggerService.Log(LogLevel.WARNING, Category.DATA, $"GetFileReference: Directory inaccessible or does not exist", "FTPFileService");
                return new(Result.Failure($"File inaccessible or does not exist", (int)response.StatusCode));
            }
        }


        public async Task<Result<List<string>>> GetFilesInDir(string dirPath)
        {
            var requestUri = new Uri($"{_ftpServer}/{dirPath}");
            ServicePointManager.FindServicePoint(requestUri).ConnectionLeaseTimeout = 0;
            _request = (FtpWebRequest)WebRequest.Create(requestUri);
            _request.Method = WebRequestMethods.Ftp.ListDirectory;
            _request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
            //_request.KeepAlive = false;

            try
            {
                // Send the LIST request to the FTP server and read the response
                using var response = (FtpWebResponse)await _request.GetResponseAsync();
                response.Dispose();
                response.Close();
                using var responseStream = response.GetResponseStream();
                using var reader = new StreamReader(responseStream);
                var responseContent = await reader.ReadToEndAsync();

                // Parse the response and return a list of file names
                var files = responseContent.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                           .Select(file => file.Trim())
                                           .ToList();
                for (var i = 0; i < files.Count; i++)
                {
                    files[i] = $"{_ftpServer}/{dirPath}/{files[i]}";
                }
                return Result<List<string>>.Success(files);
            }
            catch (WebException ex)
            {
                _loggerService.Log(LogLevel.WARNING, Category.DATA, $"GetFilesInDir: Directory inaccessible or does not exist", "FTPFileService");
                return new(Result.Failure($"Directory inaccessible or does not exist", (int)((FtpWebResponse)ex.Response).StatusCode));
            }
        }


        public async Task<Result> UploadDir(string dirPath, List<Tuple<string, byte[]>> fileNameData)
        {
            var requestUri = new Uri($"{_ftpServer}/{dirPath}");
            ServicePointManager.FindServicePoint(requestUri).ConnectionLeaseTimeout = 0;

            _request = (FtpWebRequest)WebRequest.Create(requestUri);
            _request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
            _request.Method = WebRequestMethods.Ftp.UploadFile;

            //_request.KeepAlive = false;

            using var stream = await _request.GetRequestStreamAsync();
            using var content = new MultipartFormDataContent();

            foreach (var (fileName, fileData) in fileNameData)
            {
                stream.Write(fileData, 0, fileData.Length);
            }

            using var response = (FtpWebResponse)await _request.GetResponseAsync();
            await _request.EndGetRequestStream();
            response.Dispose();
            response.Close();
            if (response.StatusCode == FtpStatusCode.ClosingData)
            {
                return Result.Success();
            }
            else
            {
                _loggerService.Log(LogLevel.WARNING, Category.DATA, $"UploadDir: Directory inaccessible or does not exist", "FTPFileService");
                return Result.Failure($"{response.StatusDescription}", (int)response.StatusCode);
            }
        }


        public async Task<Result> UploadFile(string filePath, string fileName, byte[] fileData)
        {
            var requestUri = new Uri($"{_ftpServer}/{filePath}/{fileName}");
            ServicePointManager.FindServicePoint(requestUri).ConnectionLeaseTimeout = 0;

            _request = (FtpWebRequest)WebRequest.Create(requestUri);
            _request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
            _request.Method = WebRequestMethods.Ftp.UploadFile;
            //_request.KeepAlive = false;

            using (var requestStream = await _request.GetRequestStreamAsync())
            {
                await requestStream.WriteAsync(fileData, 0, fileData.Length);
            }

            using (var response = (FtpWebResponse)_request.GetResponse())
            {
                response.Dispose();
                response.Close();
                if (response.StatusCode == FtpStatusCode.ClosingData)
                {
                    return Result.Success();
                }
                else
                {
                    _loggerService.Log(LogLevel.WARNING, Category.DATA, $"UploadFile: Directory inaccessible or does not exist", "FTPFileService");
                    return Result.Failure($"Failed to upload file. Response code: {response.StatusCode}", (int)response.StatusCode);
                }
            }
        }

    }
}
