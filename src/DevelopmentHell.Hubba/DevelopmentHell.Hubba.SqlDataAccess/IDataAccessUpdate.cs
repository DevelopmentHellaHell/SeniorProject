using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    internal interface IDataAccessUpdate : IDataAccess
    {
        Result Update(string databaseName, string tableName, List<Object> values);

    }
}
