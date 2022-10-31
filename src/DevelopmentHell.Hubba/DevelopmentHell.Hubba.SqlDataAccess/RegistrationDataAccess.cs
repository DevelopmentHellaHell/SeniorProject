using Microsoft.Data.SqlClient;

namespace DevelopmentHell.Hubba.SqlDataAccess
{

    /*    public enum ResultStatus
        {
            Unknown = 0,
            Success = 1,
            Faulty = 2
        }*/
    public class Result
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public object Payload { get; set; }

    }
    public class RegistrationDataAccess
    {
            public RegistrationDataAccess()
            {

            }
            public RegistrationDataAccess(string tableName)
            {

            }
            public Result RegisterAccount(string username, string passphrase)
            {
                var result = new Result();

                return result;
            }
    }
}