using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    public interface IUserLoginDataAccess
    {
        Task<Result> AddLogin(int accountId, string ipAddress);
        Task<Result<string[]>> GetIPAddress(int accountId);
        Task<Result> Delete(int accountId);
    }
}