
using DevelopmentHell.Hubba.Models;
using Microsoft.AspNetCore.Http;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface ICollaboratorFileDataAccess
    {
        Task<Result<int>> InsertFileWithOutputId(string fileUrl, IFormFile file);
        Task<Result> UpdateFileUrl(int fileId, string fileUrl);
        Task<Result<List<string>>> SelectFileUrls(List<int> fileIds);
        Task<Result<List<int>>> SelectFileIdsFromOwner(int accountId);
        Task<Result> DeleteFilesFromUrl(string[] removedFileUrls);
    }
}
