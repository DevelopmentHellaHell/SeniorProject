using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.AccountDeletion.Manager.Abstraction
{
    internal interface IAccountDeletionManager
    {
        Task<Result> DeleteAccount(int accountID, IPrincipal? principal = null);

    }
}
