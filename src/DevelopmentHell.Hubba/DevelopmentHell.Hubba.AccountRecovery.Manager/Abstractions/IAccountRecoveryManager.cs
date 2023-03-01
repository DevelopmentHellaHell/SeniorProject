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
        Task<Result<string>> Verification(string email, bool enabledSend = true);
        Task<Result<bool>> AuthenticateOTP(string otp, string ipAddress);
        Task<Result<string>> AccountAccess(string ipAddress);
    }
}
