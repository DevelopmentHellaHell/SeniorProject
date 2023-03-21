using DevelopmentHell.Hubba.Models;


namespace DevelopmentHell.Hubba.AccountRecovery.Service.Abstractions
{
	public interface IAccountRecoveryService
	{
		Task<Result<bool>> CompleteRecovery(int accountId, string ipAddress);
		Task<Result<int>> Verification(string email);
		bool CheckIpAddress(string[] successfulIpAddress, string ipAddress);
	}
}