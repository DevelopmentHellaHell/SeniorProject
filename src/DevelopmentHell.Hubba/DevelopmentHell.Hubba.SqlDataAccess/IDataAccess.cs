using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.SqlDataAccess
{
    // I'm not sure if this interface is necessary, but it allows each data access member to have access to Result
    // I'm not sure where else I'd put Result otherwise since I don't want to duplicate it across all interfaces or only have it in 1
    internal interface IDataAccess
    {

    }
    public class Result
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public object? Payload { get; set; }

    }
}
