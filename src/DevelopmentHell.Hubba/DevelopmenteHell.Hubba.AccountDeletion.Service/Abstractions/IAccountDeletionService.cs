using DevelopmentHell.Hubba.Models;


namespace DevelopmentHell.Hubba.AccountDeletion.Service.Abstractions
{
    public interface IAccountDeletionService
    {
        Task<Result> DeleteAccount(int accountID);
        Task<Result<int>> CountAdmin();
    }
}