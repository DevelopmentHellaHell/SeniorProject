using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public interface IUserLoginDataAccess
    {
        Task<Result> AddLogin(int accountId, string ipAddress);
        Task<Result<string[]>> GetIPAddress(int accountId);
        Task<Result> Delete(int accountId);
    }
}