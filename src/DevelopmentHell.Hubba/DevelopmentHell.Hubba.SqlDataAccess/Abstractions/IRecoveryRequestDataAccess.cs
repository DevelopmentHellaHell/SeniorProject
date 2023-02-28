using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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