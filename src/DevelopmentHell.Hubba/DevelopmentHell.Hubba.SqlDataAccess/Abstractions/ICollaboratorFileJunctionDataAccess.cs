
using DevelopmentHell.Hubba.Models;
using Microsoft.AspNetCore.Http;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface ICollaboratorFileJunctionDataAccess
    {
        Task<Result> InsertCollaboratorFile(int collabId, int fileId);
    }
}
