using Microsoft.Data.SqlClient;
using System.Text;

namespace DevelopmentHell.Hubba.SqlDataAccess
{

    /*    public enum ResultStatus
        {
            Unknown = 0,
            Success = 1,
            Faulty = 2
        }*/

    public class RegistrationDataAccess : IDataAccessInsert, IDataAccessUpdate
    {
        public RegistrationDataAccess()
        {
        }
        public RegistrationDataAccess(string tableName)
        {

        }
        /// <summary>
        /// Insert an email and passphrase into the method and returns a result
        /// </summary>
        /// <param name="dataBaseName">Name of the database that is being accessed</param>
        /// <param name="tableName">Table of the database that values are inserted into</param>
        /// <param name="values">Values added to the database. In this case it should only be email and passphrase hash</param>
        /// <returns>Result of SQL insertion statement into the Registration database of a new Account. 
        /// IsSuccessful is whether the insertion succeeded,
        /// ErrorMessage contains any error codes if the insertion failed,
        /// Payload is empty
        /// </returns>
        /// 
        public Result InsertNewAccount(string databaseName, string tableName, List<Object> values)
        {
            var result = new Result();
            List<Object> columnNames = new List<object>();

            foreach (Object obj in values)
            {
                if (obj is null)
                {
                    result.ErrorMessage = "Error: Email or passphrase is NULL.";
                    result.IsSuccessful = false;
                    return result;
                }
            }

            columnNames.Add("email");
            columnNames.Add("passphrase");
            result = Insert(databaseName, tableName, columnNames, values);
            return result;
        }

        /// <summary>
        /// This method inserts the values into the specified table at the specified database, assuming ALL VALUES ARE STRINGS
        /// </summary>
        /// <param name="dataBaseName">Name of the database that is being accessed</param>
        /// <param name="tableName">Table of the database that values are inserted into</param>
        /// <param name="columnNames">Name of the columns that correspond to the values</param>
        /// <param name="values">List of the values being added, MUST BE STRINGS</param>
        /// <returns>Result of SQL insertion statement into the database. 
        /// IsSuccessful is whether the insertion succeeded,
        /// ErrorMessage contains any error codes if the insertion failed,
        /// Payload is empty</returns>
        public Result Insert(string databaseName, string tableName, List<Object> columnNames, List<Object> values)
        {

            var result = new Result();
            // TODO: CHANGE ENCRYPT TO TRUE FOR ACTUAL SERVER IMPLEMENTATION
            var connectionString = @"Server=localhost\SQLEXPRESS;Database=" + databaseName +"; Integrated Security=True;Encrypt=False";

            string columnName = string.Empty;
            string columnSqlStatement = String.Empty;
            string valueSqlStatement = String.Empty;
            string value = string.Empty;
            string parameter = string.Empty;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // constructing the sql insert statement
                string insertSql = "INSERT INTO " + tableName + " (" + BuildColumnSqlString(columnNames) + ") VALUES (" + BuildValueSqlString(columnNames) + ")";
                Console.WriteLine(insertSql);
                var command = new SqlCommand(insertSql, connection);

                // adding parameters for the sql statement command
                for (int i = 0; i < values.Count; ++i)
                {
                    command.Parameters.Add(new SqlParameter((string)(columnNames[i]), (string)(values[i])));
                }

                // executing command in try catch so SQL errors are sent to result.errormessage
                try
                {
                    var rows = command.ExecuteNonQuery();
                    if(rows == 1)
                    {
                        result.IsSuccessful = true;
                        //result.Payload = account_id;
                        return result;
                    }
                    result.IsSuccessful = false;
                    result.ErrorMessage = $"Rows affected was not 1. It was {rows}";
                    return result;
                }
                catch(Exception e)
                {
                    result.ErrorMessage = e.Message;
                }
                
                result.IsSuccessful = false;
                return result;
            }

        }

        public Result UpdateAccountUsername(string databaseName, string tableName, string email, string username)
        {
            var result = new Result();
            List<Object> columnNames = new List<object>();
            List<Object> values = new List<object>();


            columnNames.Add("username");
            values.Add(username);
            result = Update(databaseName, tableName, "email", email, columnNames, values);
            return result;
        }

        //TODO FILL THIS OUT LATER
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="tableName"></param>
        /// <param name="keyName"></param>
        /// <param name="keyValue"></param>
        /// <param name="columnNames"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public Result Update(string databaseName, string tableName, string keyName, string keyValue, List<Object> columnNames, List<Object> values)
        {

            var result = new Result();
            // TODO: CHANGE ENCRYPT TO TRUE FOR ACTUAL SERVER IMPLEMENTATION
            var connectionString = @"Server=localhost\SQLEXPRESS;Database=" + databaseName + "; Integrated Security=True;Encrypt=False";

            string columnName = string.Empty;
            string columnSqlStatement = String.Empty;
            string valueSqlStatement = String.Empty;
            string value = string.Empty;
            string parameter = string.Empty;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // constructing the sql insert statement
                string insertSql = "UPDATE " + tableName + " SET " + BuildUpdateSqlString(columnNames) + " WHERE " + keyName +" = @" + keyName ;
                Console.WriteLine(insertSql);
                var command = new SqlCommand(insertSql, connection);
                command.Parameters.Add(new SqlParameter(keyName, keyValue));

                // adding parameters for the sql statement command
                for (int i = 0; i < values.Count; ++i)
                {
                    command.Parameters.Add(new SqlParameter((string)(columnNames[i]), (string)(values[i])));
                }

                // executing command in try catch so SQL errors are sent to result.errormessage
                try
                {
                    var rows = command.ExecuteNonQuery();
                    if (rows == 1)
                    {
                        result.IsSuccessful = true;
                        //result.Payload = account_id;
                        return result;
                    }
                    result.IsSuccessful = false;
                    result.ErrorMessage = $"Rows affected was not 1. It was {rows}";
                    return result;
                }
                catch (Exception e)
                {
                    result.ErrorMessage = e.Message;
                }

                result.IsSuccessful = false;
                return result;
            }
        }

        public Result AccessEmail(int account_id)
        {
            //TODO: RETURN EMAIL 
            var result = new Result();

            return result;
            

        }

        public Result AccessPassphrase(int account_id)
        {
            //TODO: RETURN passphrase 
            var result = new Result();
            return result;
            
        }

        /// <summary>
        /// Helper method used to construct the string which is used for column names in sql statements
        /// </summary>
        /// <param name="columnNames">The column names corresponding to the table and values for the sql statement</param>
        /// <returns>Constructed string of column headers for use in the sql statement</returns>
        private string BuildColumnSqlString(List<object> columnNames)
        {
            bool firstValue = true;
            StringBuilder sb = new StringBuilder();
            foreach (Object obj in columnNames)
            {
                string columnName = string.Empty;
                if (obj is not null)
                {
                    columnName = (string)obj;
                }
                if (firstValue == true)
                {
                    sb.Append(columnName);
                    firstValue = false;
                }
                else
                {
                    sb.Append(", " + columnName);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Helper method used to construct the string which is used for values in sql statements
        /// </summary>
        /// <param name="columnNames">The column names corresponding to the table and values for the sql statement</param>
        /// <returns>Constructed string of values for use in the sql statement</returns>
        private string BuildValueSqlString(List<object> columnNames)
        {
            bool firstValue = true;
            string columnName = string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (Object obj in columnNames)
            {
                if (obj is not null)
                {
                    columnName = (string)obj;
                }
                if (firstValue == true)
                {
                    sb.Append("@" + columnName);
                    firstValue = false;
                }
                else
                {
                    sb.Append(", @" + columnName);
                }
            }
            return sb.ToString();
        }
        private string BuildUpdateSqlString(List<object> columnNames)
        {
            bool firstValue = true;
            string columnName = string.Empty;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < columnNames.Count; i++)
            {
                if (columnNames[i] is not null)
                {
                    columnName = (string)columnNames[i];
                }
                if (firstValue == true)
                {
                    sb.Append(columnName + " = @" + columnName);
                    firstValue = false;
                }
                else
                {
                    sb.Append(", " +columnName + " = @" + columnName);
                }
            }
            return sb.ToString();
        }

    }
}