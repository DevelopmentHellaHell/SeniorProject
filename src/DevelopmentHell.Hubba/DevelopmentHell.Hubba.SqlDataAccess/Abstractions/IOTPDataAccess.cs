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
        Task<Result> NewOTP(int accountId, byte[] encryptedOTP, DateTime expiration);
        Task<Result<byte[]>> GetOTP(int accountId);
        Task<Result> Delete(int accountId);
    }
}
