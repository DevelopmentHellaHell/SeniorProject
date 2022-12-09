using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.UserService.Implementation
{
    public interface IUserService
    {
        Result<UserProfile> CreateUser(string email, string password);
        Result DeleteUser();
        Result UpdateUser(UserProfile userProfile);
        Result DisableUser();
    }
}