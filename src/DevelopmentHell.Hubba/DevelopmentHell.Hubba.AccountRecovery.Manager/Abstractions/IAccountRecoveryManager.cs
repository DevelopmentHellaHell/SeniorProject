using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.AccountRecovery.Manager.Abstractions
{
    public interface IAccountRecoveryManager
    {
        Task<Result<int>> Verification(string email, IPrincipal? principal = null, bool enabledSend = true);
        Task<Result<bool>> AuthenticateOTP(int accountId, string otp, string ipAddress, IPrincipal? principal = null);
        Task<Result<GenericPrincipal>> AccountAccess(int accountId, string ipAddress);
    }
}
