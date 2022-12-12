using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Implementation;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Authorization.Manager
{
    public class AuthorizationManager
    {
        private IAuthorizationService _authorizationService;
		private ILoggerService _loggerService;
		AuthorizationManager(IAuthorizationService authorizationService, ILoggerService loggerService)
        {
            _authorizationService = authorizationService;
            _loggerService = loggerService;
        }

        public async Task<Result<bool>> CheckAccess(string email, string claimRequested)
        {
            return await _authorizationService.CheckAccess(email, claimRequested).ConfigureAwait(false) ;
        }
    }
}