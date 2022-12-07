using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface IOTPDataAccess
    {
        Task<Result> NewOTP(int accountId, byte[] encryptedOTP);
        Task<Result> Check(int accountId, string encryptedOTP);
        Task<Result> Delete(int accountId);
    }
}
