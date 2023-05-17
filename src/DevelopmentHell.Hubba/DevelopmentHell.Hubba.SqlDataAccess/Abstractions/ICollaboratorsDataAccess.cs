using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface ICollaboratorsDataAccess
    {
        Task<Result<List<Dictionary<string, object>>>> Curate(int offset = 0);
        Task<Result<List<Dictionary<string, object>>>> Search(string query, int offset = 0, double FTTWeight = 0.5, double VCWeight = 0.5);
        Task<Result<int>> CreateCollaborator(CollaboratorProfile collab);
        Task<Result<int>> GetOwnerId(int collabId);
        Task<Result<int>> GetCollaboratorId(int ownerId);
        Task<Result<CollaboratorProfile>> GetCollaborator(int collabId);
        Task<Result<bool>> GetPublished(int collabId);
        Task<Result> Update(int collabId, CollaboratorProfile collab);
        Task<Result> UpdatePfpFileId(int collabId, int? pfpFileId = null);
        Task<Result> Delete(int collabId);
        Task<Result> SetPublished(int collabId, bool published);
        Task<Result<bool>> HasCollaborator(int accountId);
        Task<Result<int?>> SelectCollaboratorId(int accountId);
        Task<Result<bool>> Exists(int collabId);
    }
}
