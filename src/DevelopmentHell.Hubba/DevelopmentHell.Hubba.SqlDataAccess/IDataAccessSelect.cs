using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    internal interface IDataAccessSelect : IDataAccess
    {
        Result Select(string databaseName, string tableName, List<Object> values);

    }
}
