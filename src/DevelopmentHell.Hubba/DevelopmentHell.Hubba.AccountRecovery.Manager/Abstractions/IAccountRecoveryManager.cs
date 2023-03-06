using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.AccountRecovery.Manager.Abstractions
{
    public interface IAccountRecoveryManager
    {
        Task<Result<string>> EmailVerification(string email);
        Task<Result<bool>> AuthenticateOTP(string otp, string ipAddress);
        Task<Result<string>> AccountAccess(string ipAddress);
        Result Logout();
    }
}
