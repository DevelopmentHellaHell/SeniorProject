using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    internal interface IDataAccessInsert : IDataAccess
    {
        Result Insert(Dictionary<string, string> sqlPairDict);
    }
}
