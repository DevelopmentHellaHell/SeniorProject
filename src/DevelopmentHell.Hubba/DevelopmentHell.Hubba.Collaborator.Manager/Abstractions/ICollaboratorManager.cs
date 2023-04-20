
using DevelopmentHell.Hubba.Models;
using Microsoft.AspNetCore.Http;

namespace DevelopmentHell.Hubba.Collaborator.Manager.Abstractions
{
    public interface ICollaboratorManager
    {
        Task<Result<CollaboratorProfile>> GetCollaborator(int collabId);
        Task<Result> CreateCollaborator(CollaboratorProfile collab, IFormFile[] collabFiles, IFormFile? pfpFile = null);
        Task<Result> EditCollaborator(CollaboratorProfile collab, IFormFile[]? collabFiles = null, string[]? removedFiles = null, IFormFile? pfpFile = null);
        Task<Result> DeleteCollaborator(int collabId);
        Task<Result> DeleteCollaboratorWithAccountId(int accountId);
        Task<Result> ChangeVisibility(int collabId, bool isPublic);
    }
}