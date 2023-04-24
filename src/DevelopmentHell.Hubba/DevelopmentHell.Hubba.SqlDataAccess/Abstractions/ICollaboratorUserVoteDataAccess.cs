using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface ICollaboratorUserVoteDataAccess
    {
        Task<Result> Upvote(int collabId, int accountId);
        Task<Result> Downvote(int collabId, int accountId);
    }
}
