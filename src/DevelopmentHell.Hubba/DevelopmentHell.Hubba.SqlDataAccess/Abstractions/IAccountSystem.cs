using DevelopmentHell.Hubba.Models

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface IAccountSystem
    {
        Task<Result<List<Dictionary<string, string>>>> GetPasswordData(string email);
    }
}
