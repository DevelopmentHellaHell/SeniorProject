using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface ICollaboratorUserVoteDataAccess
    {
        Task<Result> Upvote(int collabId, int accountId);
        Task<Result> Downvote(int collabId, int accountId);
    }
}
