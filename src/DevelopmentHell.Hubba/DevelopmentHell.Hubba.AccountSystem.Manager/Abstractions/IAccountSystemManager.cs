using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.AccountSystem.Manager.Abstractions
{
    public interface IAccountSystemManager
    {
        Task<Result> VerifyAccount();
        Task<Result> OTPVerification(string otpEntry);
        Task<Result> UpdateEmailInformation(string newEmail, string newPassword);
        Task<Result> UpdatePassword(string oldPassword, string newPassword, string newPasswordDupe);
        Task<Result> UpdateUserName(string firstName, string lastName);
        Task<Result<List<Dictionary<string, object>>>> GetAccountSettings();
    }
}
