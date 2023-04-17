using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.AccountRecovery.Manager.Abstractions
{
    public interface IAccountRecoveryManager
    {
        Task<Result<Tuple<string, string>>> EmailVerification(string email);
        Task<Result<bool>> AuthenticateOTP(string otp, string ipAddress);
        Task<Result<Tuple<string, string>>> AccountAccess(string ipAddress);
        Result Logout();
    }
}
