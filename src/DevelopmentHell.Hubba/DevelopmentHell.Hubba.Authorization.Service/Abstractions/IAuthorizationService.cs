using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Authorization.Service.Abstractions
{
    public interface IAuthorizationService
    {
        Task<Result<bool>> CheckAccess(int accountId, string claimRequested);
    }

    
}
