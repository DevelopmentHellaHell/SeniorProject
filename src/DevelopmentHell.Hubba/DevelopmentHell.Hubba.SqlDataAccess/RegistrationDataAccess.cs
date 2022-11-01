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
        public string ErrorMessage { get; set; } = string.Empty;
        public object? Payload { get; set; }

    }
    public class RegistrationDataAccess
    {
            public RegistrationDataAccess()
            {
            }
            public RegistrationDataAccess(string tableName)
            {

            }
            public Result RegisterAccount(string email, string passphrase)
            {
                var result = new Result();
                // TODO CHANGE ENCRYPT TO TRUE
                var connectionString = @"Server=DESKTOP-NZXT\SQLEXPRESS;Database=DevelopmentHell.Hubba.Accounts;Integrated Security=True;Encrypt=False";


                using (var connection = new SqlConnection(connectionString))
                {
                connection.Open();

                var insertSql = "INSERT INTO accounts (email,passphrase) VALUES (%email,%passphrase)";
                var parameterEmail = new SqlParameter("email", email);
                var parameterpassphrase = new SqlParameter("passphrase", passphrase);

                var command = new SqlCommand(insertSql, connection);

                command.Parameters.Add(parameterEmail);
                command.Parameters.Add(parameterpassphrase);

                var rows = command.ExecuteNonQuery();

                var selectSql = "SELECT account_id FROM accounts WHERE (email) IS values(%email)";
                command = new SqlCommand(selectSql, connection);
                command.Parameters.Add(parameterEmail);
                var account_id = command.ExecuteNonQuery();

                // TODO ACCESS SQL ACCOUNT TO GET ACCOUNT_ID

                if(rows == 1)
                {
                    result.IsSuccessful = true;
                    result.Payload = account_id;
                    return result;
                }
                
                result.IsSuccessful = false;
                result.ErrorMessage = $"Rows affected was not 1. It was {rows}";
                return result;
                }

            }

        public Result AccessEmail(int account_id)
        {
            //TODO: RETURN EMAIL 
            var result = new Result();

            var connectionString = @"Server=localhost\SQLEXPRESS;Database=DevelopmentHell.Hubba.Accounts;Integrated Security=True;Encrypt=True";


            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var insertSql = "INSERT INTO DevelopmentHell.Hubba.Accounts (account_id,email,passphrase) values(%account_id,%email,%passphrase)";
                var parameter = new SqlParameter("account_id", account_id);
                var command = new SqlCommand(insertSql, connection);
                command.Parameters.Add(account_id);
                var rows = command.ExecuteNonQuery();

                if (rows == 1)
                {
                    result.IsSuccessful = true;
                    return result;
                }

                result.IsSuccessful = false;
                result.ErrorMessage = $"Rows affected was not 1. It was {rows}";
                return result;
            }

        }

        public Result AccessPassphrase(int account_id)
        {
            //TODO: RETURN passphrase 
            var result = new Result();

            var connectionString = @"Server=localhost\SQLEXPRESS;Database=DevelopmentHell.Hubba.Accounts;Integrated Security=True;Encrypt=True";


            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var insertSql = "INSERT INTO DevelopmentHell.Hubba.Accounts (account_id,email,passphrase) values(%account_id,%email,%passphrase)";
                var parameter = new SqlParameter("account_id", account_id);
                var command = new SqlCommand(insertSql, connection);
                command.Parameters.Add(account_id);
                var rows = command.ExecuteNonQuery();

                if (rows == 1)
                {
                    result.IsSuccessful = true;
                    return result;
                }

                result.IsSuccessful = false;
                result.ErrorMessage = $"Rows affected was not 1. It was {rows}";
                return result;
            }

        }
    }
}