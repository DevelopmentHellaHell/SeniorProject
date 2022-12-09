using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.UserService.Implementation
{
	public class UserService : IUserService
	{
		Result<UserProfile> IUserService.CreateUser(string email, string password)
		{
			throw new NotImplementedException();
		}

		Result IUserService.DeleteUser()
		{
			throw new NotImplementedException();
		}

		Result IUserService.DisableUser()
		{
			throw new NotImplementedException();
		}

		Result IUserService.UpdateUser(UserProfile userProfile)
		{
			throw new NotImplementedException();
		}
	}
}