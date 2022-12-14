using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.UserService.Implementation
{
    public interface IUserManagementService
    {
        Task<Result> Register(string email, string password);
        Task<Result> Update();
        Task<Result> Recover();
        Task<Result> Disable();
		Task<Result> Delete();
	}
}