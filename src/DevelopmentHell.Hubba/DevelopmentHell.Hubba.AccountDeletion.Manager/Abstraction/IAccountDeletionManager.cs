using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.AccountDeletion.Manager.Abstraction
{
    public interface IAccountDeletionManager
    {
        Task<Result<string>> DeleteAccount(int accountId);

    }
}
