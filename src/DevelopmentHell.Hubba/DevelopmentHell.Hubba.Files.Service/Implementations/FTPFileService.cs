using DevelopmentHell.Hubba.Files.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using FluentFTP;
using FluentFTP.Exceptions;
using FluentFTP.Helpers;
using Microsoft.AspNetCore.Http;

#pragma warning disable SYSLIB0014 // Type or member is obsolete
namespace DevelopmentHell.Hubba.Files.Service.Implementations
{
    public class FTPFileService : IFileService
    {
        // removed for warning
        //private static FtpWebRequest? _request;
        private readonly string _ftpServer;
        private readonly string _httpServer;
        private readonly string _ftpUsername;
        private readonly string _ftpPassword;
        private readonly ILoggerService _loggerService;
        private readonly AsyncFtpClient _ftpClient;
        public FTPFileService(string ftpServer, string ftpUsername, string ftpPassword, ILoggerService loggerService)
        {
            _ftpServer = "ftp://" + ftpServer;
            _httpServer = "http://" + ftpServer;
            _ftpUsername = ftpUsername;
            _ftpPassword = ftpPassword;
            _loggerService = loggerService;

            _ftpClient = new(_ftpServer, _ftpUsername, _ftpPassword);
        }

        public async Task<Result> CreateDir(string dirPath)
        {
            if (await _ftpClient.DirectoryExists(dirPath))
            {
                return Result.Success();
            }
            if (await _ftpClient.CreateDirectory(dirPath))
            {
                return Result.Success();
            }
            return Result.Failure("Unable to create Directory");
        }

        public async Task<Result> DeleteDir(string dirPath)
        {
            try
            {
                await _ftpClient.DeleteDirectory(dirPath);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Unable to delete Directory: {ex.Message}");
            }
            return Result.Success();
        }

        public async Task<Result> DeleteFile(string filePath)
        {
            try
            {
                await _ftpClient.DeleteFile(filePath);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to delete file: {ex.Message}");
            }
            return Result.Success();
        }

        public async Task<Result> Disconnect()
        {
            try
            {
                await _ftpClient.Disconnect();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to disconnect: {ex.Message}");
            }
            return Result.Success();
        }

        public async Task<Result<string>> GetFileReference(string filePath)
        {
            if (await _ftpClient.FileExists(filePath))
            {
                return Result<string>.Success(_httpServer + "/" + filePath);
            }
            return new(Result.Failure("Failed to Check for file existence"));
        }

        public async Task<Result<List<string>>> GetFilesInDir(string dirPath)
        {
            try
            {
                var fileList = await _ftpClient.GetListing(dirPath);
                var outFiles = new List<string>();
                foreach (var file in fileList)
                {
                    outFiles.Add(_httpServer + file.FullName);
                }
                return Result<List<string>>.Success(outFiles);
            }
            catch (FtpException ex)
            {
                return new(Result.Failure($"Directory likely does not exist: {ex.Message}"));
            }
        }

        public async Task<Result> UploadDir(string dirPath, List<Tuple<string, byte[]>> fileNameData)
        {
            bool passed = true;
            foreach (var file in fileNameData)
            {
                var status = await _ftpClient.UploadBytes(file.Item2, dirPath + "/" + file.Item1);
                if (status.IsFailure())
                {
                    passed = false;
                }
            }
            if (!passed)
            {
                return new(Result.Failure($"Unable to upload One or more of the files provided"));
            }
            return Result.Success();
        }

        public async Task<Result> UploadFile(string filePath, string fileName, byte[] fileData)
        {
            var status = await _ftpClient.UploadBytes(fileData, filePath + "/" + fileName);
            if (status.IsFailure())
            {
                return new(Result.Failure($"Upload failed: {status.ToString()}"));
            }
            return Result.Success();
        }

        public async Task<Result> UploadIFormFile(string filePath, string fileName, IFormFile file)
        {
            var status = await _ftpClient.UploadStream(file.OpenReadStream(), filePath + "/" + fileName);
            if (status.IsFailure())
            {
                return new(Result.Failure($"Upload failed: {status.ToString()}"));
            }
            return Result.Success();
        }


        public Task<Result> RenameFile(string filePath, string fileName, string newFileName)
        {
            throw new NotImplementedException();
        }
    }
}
