
using DevelopmentHell.Hubba.Models;
using Microsoft.AspNetCore.Http;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface ICollaboratorFileDataAccess
    {
        Task<Result<int>> InsertFileWithOutput(string fileUrl, IFormFile file);
    }
}
