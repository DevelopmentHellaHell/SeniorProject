using DevelopmentHell.Hubba.Authorization.Service.Implementation;
using DevelopmentHell.Hubba.Models;
using System.Security.Claims;

namespace DevelopmentHell.Hubba.Authorization.Manager
{
    public class AuthorizationManager
    {
        private AuthorizationService _authorizationService;
        AuthorizationManager(AuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public async Task<Result<bool>> CheckAccess (string email, string claimRequested)
        {
            return await _authorizationService.CheckAccess(email, claimRequested).ConfigureAwait(false) ;
        }
    }
}