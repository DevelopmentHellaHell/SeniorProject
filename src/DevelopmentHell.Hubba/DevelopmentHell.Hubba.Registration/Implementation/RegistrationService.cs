using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Implementation;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using System.Collections.Generic;
using System.Reflection;

namespace DevelopmentHell.Hubba.Registration
{
    public class RegistrationService
    {
        public Result CheckUnusedEmail(string email)
        {
            //TODO: call Data Access to look up this email in database
            var result = new Result();

            return result;
        }
        private Account _account;
        private string _connectionString;

        public RegistrationService(Account account, string connectionString)
        {
            _account = account;
            _connectionString = connectionString;
        }

        public Result registerAccount()
        {
            
            var result = new Result();
            result.IsSuccessful = false;

            var inputValidation = new InputValidation();
            //email validation
            if (inputValidation.ValidateEmail(_account.Email).IsSuccessful == false)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Email provided is invalid. Retry or contact admin.";
                return result;
            }

            //passphrase validation
            if (inputValidation.ValidatePassphrase(_account.Passphrase).IsSuccessful == false)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Passphrase provided is invalid. Retry or contact admin.";
                return result;
            }

            //unused email
            SelectDataAccess selectDAO = new SelectDataAccess(_connectionString);
            Dictionary<string, object> emailValue = new()
            {
                { "Email", _account.Email }
            };
            Result unusedEmailCheck = selectDAO.Select("Accounts", new List<String> { "COUNT(Email" }, emailValue);

            if (unusedEmailCheck is not null)
            {
                if (unusedEmailCheck.Payload is not null)
                {
                    List<List<object>> payload = (List<List<object>>)unusedEmailCheck.Payload;
                    if ((int)payload[0][0] > 0)
                    {
                        result.IsSuccessful = false;
                        result.ErrorMessage = "Email provided already exists. Retry or contact admin.";
                        return result;
                    }
                }
            }

            //username generation
            var usernameGeneration = new UsernameGeneration();
            string username = usernameGeneration.generateUsername();


            //username check
            Dictionary<string, object> usernameValue = new()
            {
                { "username", username }
            };
            Result usernameCheck = selectDAO.Select("Accounts", new List<string> { "COUNT(username)" }, usernameValue);

            //assign id to account based on username check
            if (usernameCheck is not null)
            {
                if (usernameCheck.Payload is not null)
                {
                    List<List<object>> payload = (List<List<object>>)usernameCheck.Payload;
                    _account.ID = (int)payload[0][0] + 1;
                }
            }

            //generate dictionary [String (column name), Object (value)
            var accountDictionary = new AccountDictionary(_account);
            Dictionary<String, Object> values = accountDictionary.CreateDictionary();

            //insert account
            InsertDataAccess insertDAO = new InsertDataAccess(_connectionString);
            insertDAO.Insert("Accounts", values);
            
            
            
            return result;
            
        }
        public Result HashPassphrase(string passphrase)
        {
            //TODO: use Crytography library
            var result = new Result();
            result.IsSuccessful = false;
            return result;
        }
    }
}