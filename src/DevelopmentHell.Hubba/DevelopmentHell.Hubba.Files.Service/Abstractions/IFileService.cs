using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Files.Service.Abstractions
{
    public interface IFileService
    {
        public Task<Result> UploadFile(string fileName, byte[] fileData);
        public Task<Result> DeleteFile(string fileName);
        public Task<Result<string>> GetFileReference(string fileName, string fileExtension);
        public Task<Result<List<string>>> GetFilesInDir(string dir);
    }
}
