
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.WebAPI.DTO.Collaborator;
using Microsoft.AspNetCore.Http;

namespace DevelopmentHell.Hubba.Collaborator.Manager.Abstractions
{
    public interface ICollaboratorManager
    {
        Task<Result<CollaboratorProfile>> GetCollaborator(int collabId);
        Task<Result> CreateCollaborator(CreateCollaboratorDTO collabDTO);
        Task<Result> EditCollaborator(EditCollaboratorDTO collabDTO);
        Task<Result> DeleteCollaborator(int collabId);
        Task<Result> RemoveCollaborator(int collabId);
        Task<Result> DeleteCollaboratorWithAccountId(int accountId);
        Task<Result> ChangeVisibility(int collabId, bool isPublic);
        Task<Result<bool>> HasCollaborator(int? accountId);
        Task<Result> Vote(int collabId, bool upvote);
        Task<Result<string[]>> GetFileUrls(int collabId);
    }
}