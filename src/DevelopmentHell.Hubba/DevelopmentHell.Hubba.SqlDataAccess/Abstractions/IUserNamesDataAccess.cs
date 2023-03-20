using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
    public interface IUserNamesDataAccess
    {
        Task<Result<Dictionary<string,object>>> GetData(int id);
        Task<Result> Insert(int id, string? firstName, string? lastName, string? userName);
        Task<Result> Update(int id, Dictionary<string,object> data);
    }
}
