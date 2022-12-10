using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
	public interface IUserAccountDataAccess
	{
		Task<Result<int>> GetUserAccountIdByCredentials(string email, HashData password);
		Task<Result<int>> GetUserAccountIdByEmail(string email);
		Task<Result> CreateUserAccount(string email, HashData password);
	}
}
