using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.OneTimePassword.Service.Abstractions
{
    public interface IOTPService
    {
        Task<Result<string>> NewOTP(int accountId);
        Task<Result> CheckOTP(int accountId, string otp);
        Result SendOTP(string email, string otp);
        Task<Result<string>> GetOTP(int accountId);
    }
}
