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
        private string _tableName = string.Empty;
        private string _databaseName = string.Empty;
        public RegistrationDataAccess()
        {
        }
        public RegistrationDataAccess(string databaseName, string tableName)
        {
            _tableName = tableName;
            _databaseName = databaseName;
        }
        /// <summary>
        /// Insert an email and passphrase into the method and returns a result
        /// </summary>
        /// <param name="KeyValueSqlPair">Values added to the database. In this case it should only be email and passphrase hash</param>
        /// <returns>Result of SQL insertion statement into the Registration database of a new Account. 
        /// IsSuccessful is whether the insertion succeeded,
        /// ErrorMessage contains any error codes if the insertion failed,
        /// Payload is empty
        /// </returns>
         public Result InsertNewAccount(Dictionary<string, string> KeyValueSqlPair)
        {
            Result result = Insert(KeyValueSqlPair);
            return result;
        }

        /// <summary>
        /// This method inserts the values into the specified table at the specified database, assuming ALL VALUES ARE STRINGS
        /// </summary>
        /// <param name="sqlPairDict">Column value pairs of information to be inserted into the database</param>
        /// <returns>Result of SQL insertion statement into the database. 
        /// IsSuccessful is whether the insertion succeeded,
        /// ErrorMessage contains any error codes if the insertion failed,
        /// Payload is empty</returns>
        public Result Insert(Dictionary<string, string> sqlPairDict)
        {
            var result = new Result();
            // TODO: CHANGE ENCRYPT TO TRUE FOR ACTUAL SERVER IMPLEMENTATION
            var connectionString = @"Server=localhost\SQLEXPRESS;Database=" + _databaseName +"; Integrated Security=True;Encrypt=False";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // constructing the sql insert statement
                string insertSql = "INSERT INTO " + _tableName + BuildInsertSqlString(sqlPairDict);
                var command = new SqlCommand(insertSql, connection);

                // adding parameters for the sql statement command
                foreach (var pair in sqlPairDict)
                {
                    command.Parameters.Add(new SqlParameter(pair.Key, pair.Value));
                }

                // executing command in try catch so SQL errors are sent to result.errormessage
                try
                {
                    var rows = command.ExecuteNonQuery();
                    if(rows == 1)
                    {
                        result.IsSuccessful = true;
                        return result;
                    }
                    result.IsSuccessful = false;
                    result.ErrorMessage = $"Rows affected was not 1. It was {rows}";
                    return result;
                }
                catch(Exception e)
                {
                    result.ErrorMessage = e.Message;
                    Console.WriteLine(e.Message);
                }
                
                result.IsSuccessful = false;
                return result;
            }

        }

        /// <summary>
        /// Insert a username into a corresponding email in the accounts table
        /// </summary>
        /// <param name="databaseName">Name of the database that is being accessed</param>
        /// <param name="tableName">Table of the database that values are inserted into</param>
        /// <param name="email">Values added to the database. In this case it should only be email and passphrase hash</param>
        /// <param name="username">Values added to the database. In this case it should only be email and passphrase hash</param>
        /// <returns>Result of SQL insertion statement into the Registration database of a new Account. 
        /// IsSuccessful is whether the insertion succeeded,
        /// ErrorMessage contains any error codes if the insertion failed,
        /// Payload is empty
        /// </returns>
        public Result UpdateAccountUsername(string email, string username)
        {
            var result = new Result();
            Dictionary<string, string> sqlPairDict = new()
            {
                { "username", username }
            };
            result = Update("email", email, sqlPairDict);
            return result;
        }

        /// <summary>
        /// Updates the database based on the name of the database, table name, the key value pair, and whatever needs to be
        /// set in the database.
        /// </summary>
        /// <param name="databaseName">Name of the database that is being accessed</param>
        /// <param name="tableName">Table of the database that values are inserted into</param>
        /// <param name="keyName">Name of the key being used to locate the row in the database</param>
        /// <param name="keyValue">Value of the key being used to locate the row in the database</param>
        /// <param name="columnNames">Names of columns which are going to be updated</param>
        /// <param name="values">Values corresponding to the columns which are going to be updated</param>
        /// <returns>Result of SQL update statement into the database. 
        /// IsSuccessful is whether the insertion succeeded,
        /// ErrorMessage contains any error codes if the insertion failed,
        /// Payload is empty</returns>
        public Result Update(string keyName, string keyValue, Dictionary<string, string> sqlPairDict)
        {

            var result = new Result();
            // TODO: CHANGE ENCRYPT TO TRUE FOR ACTUAL SERVER IMPLEMENTATION
            var connectionString = @"Server=localhost\SQLEXPRESS;Database=" + _databaseName + "; Integrated Security=True;Encrypt=False";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // constructing the sql insert statement
                string insertSql = "UPDATE " + _tableName + " SET " + BuildUpdateSqlString(sqlPairDict) + " WHERE " + keyName + " = @" + keyName;
                var command = new SqlCommand(insertSql, connection);
                command.Parameters.Add(new SqlParameter(keyName, keyValue));

                // adding parameters for the sql statement command
                foreach (var pair in sqlPairDict)
                {
                    command.Parameters.Add(new SqlParameter(pair.Key, pair.Value));
                }

                // executing command in try catch so SQL errors are sent to result.errormessage
                try
                {
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
        /// Helper method used to construct the string which is used for values in insert sql statements
        /// </summary>
        /// <param name="columnNames">The column names corresponding to the table and values for the insert sql statement</param>
        /// <returns>Constructed string of values for use in the insert sql statement</returns>
        private static string BuildInsertSqlString(Dictionary<string, string> sqlPairDict)
        {
            bool firstValue = true;
            StringBuilder sbColumn = new StringBuilder();
            sbColumn.Append(" (");
            StringBuilder sbValue = new StringBuilder();
            foreach (var pair in sqlPairDict)
            {
                if (firstValue == true)
                {
                    sbColumn.Append(pair.Key);
                    sbValue.Append("@" + pair.Key);
                    firstValue = false;
                }
                else
                {
                    sbColumn.Append(", " + pair.Key);
                    sbValue.Append(", @" + pair.Key);
                }
            }
            sbColumn.Append(") VALUES (" + sbValue.ToString() + ")");
            return sbColumn.ToString();
        }
        /// <summary>
        /// Helper method used to construct the string which is used for column names in update sql statements
        /// </summary>
        /// <param name="columnNames">The column names corresponding to the table and values for the update sql statement</param>
        /// <returns>Constructed string of column headers for use in the update sql statement</returns>
        private static string BuildUpdateSqlString(Dictionary<string, string> sqlPairDict)
        {
            bool firstValue = true;
            StringBuilder sb = new();
            foreach (var pair in sqlPairDict)
            {
                if (firstValue == true)
                {
                    sb.Append(pair.Key + " = @" + pair.Key);
                    firstValue = false;
                }
                else
                {
                    sb.Append(", " + pair.Key + " = @" + pair.Key);
                }
            }
            return sb.ToString();
        }

    }
}