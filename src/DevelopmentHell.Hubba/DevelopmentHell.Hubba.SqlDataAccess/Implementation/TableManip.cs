using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess.Implementation
{
    internal class TableManip
    {
        public static string InnerJoinTables(List<Tuple<string, string, string, string>> tableColumns)
        {
            string output = String.Concat(Enumerable.Repeat("(", tableColumns.Count));
            foreach (Tuple<string, string, string, string> tableColumn in tableColumns)
            {
                output += String.Format("\n{0} INNER JOIN {1} ON {0}.{2} = {1}.{3})", tableColumn.Item1, tableColumn.Item2, tableColumn.Item3, tableColumn.Item4);
            }
            return output;
        }
        public static string InnerJoinTables(string table1, string table2, string column1, string column2)
        {
            return String.Format("\n({0} INNER JOIN {1} ON {0}.{2} = {1}.{3})", table1, table2, column1, column2);
        }
    }
}
