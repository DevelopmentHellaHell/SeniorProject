using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Authorization.Service.Implementation
{
    public class AuthorizationService : IAuthorizationService
    {
        private AuthorizationDataAccess _authorizationDataAccess;
        private UserAccountDataAccess _userAccountDataAccess;
        private GenericIdentity _identity;
        private IPrincipal _principal;

        public AuthorizationService(GenericIdentity identity, IPrincipal principal, AuthorizationDataAccess authorizationDataAccess, UserAccountDataAccess userAccountDataAccess)
        {
            _identity = identity;
            _principal = principal;
            _authorizationDataAccess = authorizationDataAccess;
            _userAccountDataAccess = userAccountDataAccess;
        }

        public async Task<Result<bool>> CheckAccess (string email, string claimRequested)
        {
            Result<bool> output = new();

            Result<List<Role>> roleResult = await _authorizationDataAccess.GetRoles((await _userAccountDataAccess.GetId(email).ConfigureAwait(false)).Payload).ConfigureAwait(false);
            if (!roleResult.IsSuccessful || roleResult.Payload is null)
            {
                output.IsSuccessful = false;
                output.ErrorMessage = roleResult.ErrorMessage ?? "Unable to select roles from given email";
                return output;
            }

            Result<int> idResult = await _userAccountDataAccess.GetId(email).ConfigureAwait(false);
            if (!idResult.IsSuccessful)
            {
                output.IsSuccessful = false;
                output.ErrorMessage = idResult.ErrorMessage ?? "Unable to grab Id from given email";
                return output;
            }

            Result<bool> accessResult = await _authorizationDataAccess.hasAccess(idResult.Payload,claimRequested);
            if (!accessResult.IsSuccessful)
            {
                output.IsSuccessful = false;
                output.ErrorMessage = accessResult.ErrorMessage ?? "Unable to check access for Id associated with given email";
                return output;
            }

            output.IsSuccessful = true;
            output.Payload = accessResult.Payload;
            return output;
        }

        // By default, unauthenticated users will only be given access to resources or functionalities
        // that does not require knowledge of user's identity (i.e., anonymous user)
        public bool IsAnonymous()
        {
            return _identity.IsAuthenticated == false;
        }

        // The operation and timestamp of each unauthorized access will be recorded by the system
        public void RecordUnauthorizedAccess(string operation, DateTime timestamp)
        {
            // code to record unauthorized access goes here
        }

        // The system must prevent unauthorized users from viewing, modifying or deleting any protected data
        // (scalar or aggregate data)
        public bool CanAccessData(string dataType)
        {
            if (_principal.IsInRole("D")) ;
            // code to check if user can access data goes here
            return true;
        }

        public bool CanModifyData(string dataType)
        {
            // code to check if user can modify data goes here
            return true;
        }

        public bool CanDeleteData(string dataType)
        {
            // code to check if user can delete data goes here
            return true;
        }

        // The system must prevent unauthorized users from executing any protected functionality
        public bool CanExecuteFunctionality(string functionality)
        {
            // code to check if user can execute functionality goes here
            return true;
        }

        // The system must prevent unauthorized users from viewing or interacting with any protected views
        public bool CanAccessView(string view)
        {
            // code to check if user can access view goes here
            return true;
        }

        public bool CanInteractWithView(string view)
        {
            // code to check if user can interact with view goes here
            return true;
        }
    }
}
