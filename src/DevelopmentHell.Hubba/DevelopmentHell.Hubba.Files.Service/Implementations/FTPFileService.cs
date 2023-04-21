using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using DevelopmentHell.Hubba.Files.Service.Abstractions;
using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Files.Service.Implementations
{
    public class FTPFileService : IFileService
    {
        private readonly string _ftpServer;
        private readonly string _ftpUsername;
        private readonly string _ftpPassword;
        public FTPFileService(string ftpServer, string ftpUsername, string ftpPassword)
        {
            _ftpServer = ftpServer;
            _ftpUsername = ftpUsername;
            _ftpPassword = ftpPassword;
        }
        public Task<Result> DeleteFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<Result<string>> GetFileReference(string fileName, string fileExtension)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<string>>> GetFilesInDir(string dir)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> UploadFile(string filePath, string fileName, byte[] fileData)
        {
            var httpClientHandler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(_ftpUsername, _ftpPassword)
            };
            var httpClient = new HttpClient(httpClientHandler);

            var requestUri = new Uri($"{_ftpServer}/{filePath}/{fileName}");

            using var content = new ByteArrayContent(fileData);
            using var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
            {
                Content = content
            };

            using var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
            }
        }
    }
}
