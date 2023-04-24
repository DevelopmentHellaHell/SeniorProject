using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.AccountSystem.Abstractions
{
    public interface IAccountSystemService
    {
        Task<Result> UpdateEmailInformation(int userId, string newEmail);
        Task<Result<PasswordInformation>> GetPasswordData(int userId);
        Task<Result> UpdatePassword(string newHashPassword, string email);
        Task<Result> UpdateUserName(int userId, string firstName, string lastName);
        Task<Result<AccountSystemSettings>> GetAccountSettings(int userId);
        Task<Result<int>> CheckNewEmail(string newEmail);
    }
}
