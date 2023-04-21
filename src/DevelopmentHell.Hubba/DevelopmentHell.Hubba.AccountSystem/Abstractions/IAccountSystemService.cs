using DevelopmentHell.Hubba.Models

namespace DevelopmentHell.Hubba.AccountSystem.Abstractions
{
    public interface IAccountSystemService
    {
        Task<Result> UpdateEmailInformation(string email, string newEmail, string newSalt, string newPasswordHash);
    }
}
