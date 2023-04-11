using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Authentication.Manager.Abstractions
{
    public interface IAuthenticationManager
    {
        Task<Result<string>> Login(string email, string password, string ipAddress);
        Task<Result<Tuple<string, string>>> AuthenticateOTP(string otp, string ipAddress);
        Result<string> Logout();
    }
}
