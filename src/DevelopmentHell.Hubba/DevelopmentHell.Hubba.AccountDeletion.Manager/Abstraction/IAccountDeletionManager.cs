using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.AccountDeletion.Manager.Abstraction
{
    public interface IAccountDeletionManager
    {
        Task<Result<string>> DeleteAccount(int accountId);

    }
}
