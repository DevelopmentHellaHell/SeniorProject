using System.Net;
using DevelopmentHell.Hubba.Files.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using System.Net.Http.Headers;

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

        public async Task<Result> DeleteDir(string dirPath)
        {
            var requestUri = new Uri($"{_ftpServer}/{dirPath}");
            // *Method found online, lost source*
            // Create a WebDAV DELETE request to delete the directory and its contents
            using var request = new HttpRequestMessage(new HttpMethod("DELETE"), requestUri);
            request.Headers.Add("Depth", "infinity");

            // Send the DELETE request to the FTP server
            using var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return Result<string>.Success(requestUri.OriginalString);
            }
            else
            {
                _loggerService.Log(LogLevel.WARNING, Category.DATA, $"DeleteDir: Directory inaccessible or does not exist", "FTPFileService");
                return new(Result.Failure($"Directory inaccessible or does not exist", (int)response.StatusCode));
            }
        }

        public async Task<Result> DeleteFile(string filePath)
        {
            var requestUri = new Uri($"{_ftpServer}/{filePath}");

            using var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            using var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return Result<string>.Success(requestUri.OriginalString);
            }
            else
            {
                _loggerService.Log(LogLevel.WARNING, Category.DATA, $"DeleteFile: Directory inaccessible or does not exist", "FTPFileService");
                return new(Result.Failure($"File inaccessible or does not exist", (int)response.StatusCode));
            }
        }

        public async Task<Result<string>> GetFileReference(string filePath)
        {
            var requestUri = new Uri($"{_ftpServer}/{filePath}");

            using var request = new HttpRequestMessage(HttpMethod.Head, requestUri);
            using var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return Result<string>.Success(requestUri.OriginalString);
            }
            else
            {
                _loggerService.Log(LogLevel.WARNING, Category.DATA, $"GetFileReference: Directory inaccessible or does not exist", "FTPFileService");
                return new(Result.Failure($"File inaccessible or does not exist", (int)response.StatusCode));
            }
        }

        public async Task<Result<List<string>>> GetFilesInDir(string dirPath)
        {
            var requestUri = new Uri($"{_ftpServer}/{dirPath}");

            // Create an HTTP GET request to list the directory
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            // Send the GET request to the FTP server and read the response
            using var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                _loggerService.Log(LogLevel.WARNING, Category.DATA, $"GetFilesInDir: Directory inaccessible or does not exist", "FTPFileService");
                return new(Result.Failure($"Directory inaccessible or does not exist", (int)response.StatusCode));
            }
            var responseContent = await response.Content.ReadAsStringAsync();

            // Parse the response and return a list of file names
            var files = responseContent.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                       .Select(file => file.Trim())
                                       .ToList();
            for ( var i = 0; i < files.Count; i++)
            {
                files[i] = $"{_ftpServer}/{dirPath}/{files[i]}";
            }
            return Result<List<string>>.Success(files);
        }

        public async Task<Result> UploadDir(string dirPath, List<Tuple<string, byte[]>> fileNameData)
        {
            var remoteUri = new Uri($"{_ftpServer}/{dirPath}");

            // *Method found online, lost source*
            using var request = new HttpRequestMessage(HttpMethod.Post, remoteUri);
            using var content = new MultipartFormDataContent();
            
            foreach (var (fileName, fileData) in fileNameData)
            {
                var fileContent = new ByteArrayContent(fileData);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                content.Add(fileContent, fileName, fileName);
            }

            request.Content = content;
            using var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return Result.Success();
            }
            else
            {
                _loggerService.Log(LogLevel.WARNING, Category.DATA, $"UploadDir: Directory inaccessible or does not exist", "FTPFileService");
                return Result.Failure($"{response.Content}", (int)response.StatusCode);
            }
        }

        public async Task<Result> UploadFile(string filePath, string fileName, byte[] fileData)
        {
            var requestUri = new Uri($"{_ftpServer}/{filePath}/{fileName}");
            using var content = new ByteArrayContent(fileData);
            using var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
            {
                Content = content
            };

            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                return Result.Success();
            }
            else
            {
                _loggerService.Log(LogLevel.WARNING, Category.DATA, $"UploadFile: Directory inaccessible or does not exist", "FTPFileService");
                return Result.Failure($"{response.Content}", (int)response.StatusCode);
            }
        }
    }
}
