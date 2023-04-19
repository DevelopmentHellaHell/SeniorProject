
using DevelopmentHell.Hubba.Models;
using Microsoft.AspNetCore.Http;

namespace DevelopmentHell.Hubba.Collaborator.Manager.Abstractions
{
    public interface ICollaboratorManager
    {
        Task<Result<CollaboratorProfile>> GetCollaborator(int collabId);
        Task<Result> CreateCollaborator(CollaboratorProfile collab, IFormFile[] collabFiles, IFormFile pfpFile);
        Task<Result> EditCollaborator(CollaboratorProfile collab, IFormFile[] collabFiles, IFormFile pfpFile, string[] removedFiles);
        Task<Result> DeleteCollaborator(int collabId);
        Task<Result> DeleteCollaboratorWithAccountId(int accountId);
        Task<Result> ChangeVisibility(int collabId, bool isPublic);
    }
}