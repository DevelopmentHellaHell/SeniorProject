﻿using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Files.Service.Abstractions
{
    public interface IFileService
    {
        public Task<Result> UploadFile(string filePath, string fileName, byte[] fileData);
        public Task<Result> UploadDir(string dirPath, List<Tuple<string,byte[]>> fileNameData);
        public Task<Result> DeleteFile(string filePath);
        public Task<Result> DeleteDir(string dirPath);
        public Task<Result<string>> GetFileReference(string filePath);
        public Task<Result<List<string>>> GetFilesInDir(string dirPath);
    }
}
