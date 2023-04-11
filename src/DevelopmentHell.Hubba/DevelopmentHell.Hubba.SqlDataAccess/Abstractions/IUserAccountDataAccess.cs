using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
	public interface IUserAccountDataAccess
	{
		Task<Result> CreateUserAccount(string email, HashData password, string role);
		Task<Result<int>> GetId(string email);
		Task<Result<int>> GetId(string email, HashData password);
		Task<Result<UserAccount>> GetUser(int id);
		Task<Result<UserAccount>> GetAttempt(int id);
		Task<Result<UserAccount>> GetHashData(string email);
		Task<Result<bool>> GetDisabled(int id);
		Task<Result> Update(UserAccount userAccount);
		Task<Result> Delete(int id);
		Task<Result> Delete(string email);
		Task<Result<int>> CountAdmin();
		Task<Result> InsertUpdateAccount(string email, Dictionary<string, object> data);
		Task<Result> SetEnabledStatus(int id, bool enabled);

	}
}