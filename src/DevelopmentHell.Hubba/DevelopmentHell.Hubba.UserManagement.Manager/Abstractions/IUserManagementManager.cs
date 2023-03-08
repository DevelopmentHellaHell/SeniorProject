using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.UserManagement.Manager.Abstractions
{
    public interface IUserManagementManager
    {
        Task<Result> ElevatedUpdateAccount(string email, string passphrase, string firstName, string lastName, string role);
        Task<Result> ElevatedUpdateAccount(string email, Dictionary<string, object> data);
        Task<Result> ElevatedDeleteAccountNotifyListingsBookings(string email);
        Task<Result> ElevatedEnableAccount(string email);
        Task<Result> ElevatedDisableAccount(string email);
    }
}
