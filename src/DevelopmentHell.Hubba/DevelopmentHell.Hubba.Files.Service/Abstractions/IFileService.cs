﻿using DevelopmentHell.Hubba.Models;
using Microsoft.AspNetCore.Http;

namespace DevelopmentHell.Hubba.Files.Service.Abstractions
{
    public interface IFileService
    {
        public Task<Result> CreateDir(string dirPath);
        public Task<Result> UploadFile(string filePath, string fileName, byte[] fileData);
        public Task<Result> UploadIFormFile(string filePath, string fileName, IFormFile file);
        public Task<Result> UploadDir(string dirPath, List<Tuple<string, byte[]>> fileNameData);
        public Task<Result> DeleteFile(string filePath);
        public Task<Result> DeleteDir(string dirPath);
        public Task<Result<string>> GetFileReference(string filePath);
        public Task<Result<List<string>>> GetFilesInDir(string dirPath);
        public Task<Result> Disconnect();
        public Task<Result> RenameFile(string filePath, string fileName, string newFileName);
    }
}
