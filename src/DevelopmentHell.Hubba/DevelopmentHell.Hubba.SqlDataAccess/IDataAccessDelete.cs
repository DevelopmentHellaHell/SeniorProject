using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    internal interface IDataAccessDelete : IDataAccess
    {
        Result Insert(string databaseName, string tableName, List<Object> values);

    }
}
