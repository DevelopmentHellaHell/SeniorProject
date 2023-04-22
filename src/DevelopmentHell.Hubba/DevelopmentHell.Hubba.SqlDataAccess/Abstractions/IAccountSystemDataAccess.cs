using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface IAccountSystemDataAccess
    {
        Task<Result> SavePassword(string newHashPassword, string email);
        Task<Result> SaveEmailAlterations(string email, string newEmail);
        Task<Result<List<Dictionary<string, object>>>> GetPasswordData(int userId);
        Task<Result> UpdateUserName(int userId, string firstName, string lastName);
        Task<Result<List<Dictionary<string, object>>>> GetAccountSettings(int userId);
    }
}
