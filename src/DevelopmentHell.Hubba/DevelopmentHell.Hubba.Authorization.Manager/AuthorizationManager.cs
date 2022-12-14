//using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
//using DevelopmentHell.Hubba.Cryptography.Service;
//using DevelopmentHell.Hubba.Logging.Service.Abstractions;
//using DevelopmentHell.Hubba.Models;

//namespace DevelopmentHell.Hubba.Authorization.Manager
//{
//    public class AuthorizationManager
//    {
//        private IAuthorizationService _authorizationService;
//		private ILoggerService _loggerService;
//		AuthorizationManager(IAuthorizationService authorizationService, ILoggerService loggerService)
//        {
//            _authorizationService = authorizationService;
//            _loggerService = loggerService;
//        }

//        public async Task<Result<bool>> CheckAccess(AuthTicket ticket, string claimRequested)
//        {
//            AuthTicket convertedTicket = AuthTicketConversionService.FromBytes(ticket.Self!);
//            return await _authorizationService.CheckAccess(convertedTicket.AccountId, claimRequested).ConfigureAwait(false);
//        }
//    }
//}