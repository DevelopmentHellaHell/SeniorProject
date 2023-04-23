using DevelopmentHell.Hubba.Models;
using Microsoft.AspNetCore.Http;

namespace DevelopmentHell.Hubba.Collaborator.Service.Abstractions
{
    public interface ICollaboratorService
    {
        Task<Result> CreateCollaborator(CollaboratorProfile collab, IFormFile[] collabFiles, IFormFile? pfpFile = null);
        Task<Result> DeleteCollaborator(int collabId);
        Task<Result> RemoveCollaborator(int collabId);
        Task<Result> DeleteCollaboratorWithAccountId(int accountId);
        Task<Result> EditCollaborator(CollaboratorProfile collab, IFormFile[]? collabFiles = null,
            string[]? removedFiles = null, IFormFile? pfpFile = null);
        Task<Result<CollaboratorProfile>> GetCollaborator(int collabId);
        Task<Result> UpdatePublished(int collabId, bool isPublic);
        Task<Result<int>> GetOwnerId(int collabId);
        Task<Result<bool>> HasCollaborator(int accountId);
        Task<Result> Vote(int collabId, bool upvote);
        Task<Result<string[]>> GetFileUrls(int collabId);
        Task<Result<int>> CountFilesWithoutPfp(int accountId);
        Task<Result<string?>> GetPfpUrl(int ownerId);
    }
}