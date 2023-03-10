using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.UserManagement.Manager.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.UserManagement.Manager.Implementations
{
    public class UserManagementManager : IUserManagementManager
    {
        public Task<Result> ElevatedCreateAccount(string email, string passphrase, string firstName, string lastName, string accountType)
        {
            throw new NotImplementedException();
        }

        public Task<Result> ElevatedDeleteAccountNotifyListingsBookings(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Result> ElevatedDisableAccount(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Result> ElevatedEnableAccount(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Result> ElevatedUpdateAccount(string email, string passphrase, string firstName, string lastName, string role)
        {
            throw new NotImplementedException();
        }

        public Task<Result> ElevatedUpdateAccount(string email, Dictionary<string, object> data)
        {
            throw new NotImplementedException();
        }
    }
}
