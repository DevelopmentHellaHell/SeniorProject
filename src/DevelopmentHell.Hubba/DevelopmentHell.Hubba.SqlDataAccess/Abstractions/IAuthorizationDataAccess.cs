using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface IAuthorizationDataAccess
    {
        Task<Result<List<Role>>> GetRoles(int AccountId);
        Task<Result<bool>> hasAccess(int accountId, string claim);
		//Task<Result<UserAccount>> GetRoledUser(int AccountId);
		Task<Result> GiveRole(int AccountId, Role role);
        Task<Result> RevokeRole(int AccountId, Role role);
        Task<Result> SetRoles(int AccountId, Role[] roles);
        Task<Result> RevokeRoleAll(int AccountId);
    }
}
