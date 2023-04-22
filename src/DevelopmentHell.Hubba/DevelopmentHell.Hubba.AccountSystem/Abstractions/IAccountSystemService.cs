using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.AccountSystem.Abstractions
{
    public interface IAccountSystemService
    {
        Task<Result> UpdateEmailInformation(string email, string newEmail);
        Task<Result<List<Dictionary<string, object>>>> GetPasswordData(int userId);
        Task<Result> UpdatePassword(string newHashPassword, string email);
        Task<Result> UpdateUserName(int userId, string firstName, string lastName);
        Task<Result<List<Dictionary<string, object>>>> GetAccountSetting(int userId);
    }
}
