using System.Net;
using DevelopmentHell.Hubba.Files.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using System.Net.Http.Headers;
using Azure;

#pragma warning disable SYSLIB0014 // Type or member is obsolete
namespace DevelopmentHell.Hubba.Files.Service.Implementations
{
    public class FTPFileService : IFileService
    {
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

        public async Task<Result> CreateDir(string dirPath)
        {
            // Split the directory path into individual directory names
            var dirs = dirPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            // Loop through the directories, creating each one if it doesn't exist
            var currentPath = "";
            foreach (var dir in dirs)
            {
                currentPath += $"/{dir}";
                var request = (FtpWebRequest)WebRequest.Create(new Uri($"{_ftpServer}/{currentPath}"));
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
                request.KeepAlive = false;
                try
                {
                    using (var response = (FtpWebResponse)await request.GetResponseAsync().ConfigureAwait(false))
                    {
                        response.Dispose();
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
            return Result.Success();
        }


        public async Task<Result> DeleteDir(string dirPath)
        {
            var requestUri = new Uri($"{_ftpServer}/{dirPath}");
            var request = (FtpWebRequest)WebRequest.Create(requestUri);
            request.Method = WebRequestMethods.Ftp.RemoveDirectory;
            request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
            request.KeepAlive = false;

            try
            {
                using (var response = (FtpWebResponse)await request.GetResponseAsync())
                {
                    response.Dispose();
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

        public async Task<Result> DeleteFile(string filePath)
        {
            var requestUri = new Uri($"{_ftpServer}/{filePath}");

            // Create a WebDAV DELETE request to delete the file
            var request = (FtpWebRequest)WebRequest.Create(requestUri);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.KeepAlive = false;

            try
            {
                // Send the DELETE request to the FTP server
                using var response = (FtpWebResponse)await request.GetResponseAsync();
                response.Dispose();

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

            var request = (FtpWebRequest)WebRequest.Create(requestUri);
            request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
            request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
            request.KeepAlive = false;

            try
            {
                using var response = (FtpWebResponse)await request.GetResponseAsync();
                response.Dispose();
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
            var request = (FtpWebRequest)WebRequest.Create(requestUri);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.KeepAlive = false;

            try
            {
                // Send the LIST request to the FTP server and read the response
                using var response = (FtpWebResponse)await request.GetResponseAsync();
                response.Dispose();
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
            var remoteUri = new Uri($"{_ftpServer}/{dirPath}");

            var request = (FtpWebRequest)WebRequest.Create(remoteUri);
            request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.KeepAlive = false;

            using var stream = request.GetRequestStream();
            using var content = new MultipartFormDataContent();

            foreach (var (fileName, fileData) in fileNameData)
            {
                stream.Write(fileData, 0, fileData.Length);
            }

            using var response = (FtpWebResponse)await request.GetResponseAsync();
            response.Dispose();
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

            var request = (FtpWebRequest)WebRequest.Create(requestUri);
            request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.KeepAlive = false;

            using (var requestStream = request.GetRequestStream())
            {
                await requestStream.WriteAsync(fileData, 0, fileData.Length);
            }

            using (var response = (FtpWebResponse)request.GetResponse())
            {
                response.Dispose();
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
