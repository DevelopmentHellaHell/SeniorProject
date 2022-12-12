using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Authorization.Service.Abstractions
{
    public interface IAuthorizationService
    {
        Task<Result<bool>> CheckAccess(string email, string claimRequested);
    }

    
}
