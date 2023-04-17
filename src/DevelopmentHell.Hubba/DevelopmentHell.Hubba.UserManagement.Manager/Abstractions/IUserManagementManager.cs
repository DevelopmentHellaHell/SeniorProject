using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.UserManagement.Manager.Abstractions
{
    public interface IUserManagementManager
    {
        Task<Result> ElevatedCreateAccount(string email, string passphrase, string accountType, string? firstName, string? lastName, string? userName);
        Task<Result> ElevatedDeleteAccount(string email);
        Task<Result> ElevatedUpdateAccount(string email, Dictionary<string, object> data);
        Task<Result> ElevatedEnableAccount(string email);
        Task<Result> ElevatedDisableAccount(string email);
    }
}
