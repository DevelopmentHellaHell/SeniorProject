using DevelopmentHell.Hubba.Cryptography;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.Validaition;

namespace DevelopmentHell.Hubba.UserService.Implementations
{
	public class UserManagementService : IUserManagementService
	{

		private IUserAccountDataAccess _dao;
		public UserManagementService(string connectionString)
		{
			_dao = new UserAccountDataAccess(connectionString);
		}

		public Task<Result<int>> Login(string email, string password)
		{
			
		}

		public Task<Result> Register(string email, string password)
		{
			throw new NotImplementedException();
		}

		public Task<Result> Delete()
		{
			throw new NotImplementedException();
		}

		public Task<Result> Disable()
		{
			throw new NotImplementedException();
		}

		public Task<Result> Update()
		{
			throw new NotImplementedException();
		}
	}
}