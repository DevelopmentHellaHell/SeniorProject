using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface IAccountSystemDataAccess
    {
        Task<Result> SavePassword(string newHashPassword, string email);
        Task<Result> SaveEmailAlterations(string email, string newSalt, string newEmail, string newHashPassword);
    }
}
