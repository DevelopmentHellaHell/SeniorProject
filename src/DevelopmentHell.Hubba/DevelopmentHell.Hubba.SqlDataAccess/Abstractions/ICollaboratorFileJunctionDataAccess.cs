
using DevelopmentHell.Hubba.Models;
using Microsoft.AspNetCore.Http;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface ICollaboratorFileJunctionDataAccess
    {
        Task<Result> InsertCollaboratorFile(int collabId, int fileId);
        Task<Result<List<int>>> SelectFileIdsFromCollabId(int collabId);
        Task<Result<List<string>>> SelectFileUrlsFromCollabId(int collabId);
        Task<Result<List<string>>> SelectFileUrlsFromOwnerId(int ownerId);
    }
}
