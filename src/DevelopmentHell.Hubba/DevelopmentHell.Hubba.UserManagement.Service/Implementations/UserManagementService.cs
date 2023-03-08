using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.UserManagement.Service.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.UserManagement.Service.Implementations
{
    public class UserManagementService : IUserManagementService
    {
        public Task<Result> DisableAccount(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> EnableAccount(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> SetNames(string email, string firstName, string lastName)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateAccount(string email, Dictionary<string, object> data)
        {
            throw new NotImplementedException();
        }
    }
}
