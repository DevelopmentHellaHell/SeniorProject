using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
	public interface IOTPDataAccess
	{
		Task<Result> NewOTP(int accountId, byte[] encryptedOTP, DateTime expiration);
		Task<Result<byte[]>> GetOTP(int accountId);
		Task<Result> Delete(int accountId);
	}
}
