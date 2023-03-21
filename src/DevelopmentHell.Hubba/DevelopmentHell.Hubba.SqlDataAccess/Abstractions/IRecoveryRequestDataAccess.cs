using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
	public interface IRecoveryRequestDataAccess
	{
		Task<Result> AddManualRecovery(int accountId);
		Task<Result> GetAccounts();
		Task<Result> Delete(int accountId);
		Task<Result<int>> GetId(int accountId);

	}
}