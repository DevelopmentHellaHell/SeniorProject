using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface ICollaboratorDataAccess
    {
        Task<Result<int>> CreateCollaborator(CollaboratorProfile collab);
        Task<Result<int>> GetOwnerId(int collabId);
        Task<Result<int>> GetCollaboratorId(int ownerId);
        Task<Result<CollaboratorProfile>> GetCollaborator(int collabId);
        Task<Result<bool>> GetPublished(int collabId);
        Task<Result> Update(CollaboratorProfile collab);
        Task<Result> UpdatePfpFileId(int collabId, int pfpFileId);
        Task<Result> Delete(int collabId);
        Task<Result> Remove(CollaboratorProfile collab);
        Task<Result> SetPublished(int collabId, bool published);
    }
}
