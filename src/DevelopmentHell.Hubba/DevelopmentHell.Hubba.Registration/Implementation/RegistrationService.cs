using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Registration.Implementation;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Reflection;
using System.Security.Cryptography;

namespace DevelopmentHell.Hubba.Registration
{
    public class RegistrationService
    {
        private static string _connectionString = String.Format(@"Server=localhost\SQLEXPRESS;Database=DevelopmentHell.Hubba.Accounts;Integrated Security=True;Encrypt=False", ConfigurationManager.AppSettings["AccountServer"]);
        private RegistrationDataAccess _registrationDAO;

        public RegistrationService()
        {
            _registrationDAO = new RegistrationDataAccess(_connectionString);
        }

        public async Task<Result> RegisterAccount(Account account)
        {
            
            var result = new Result();
            result.IsSuccessful = false;

            //age validation
            if (Implementation.BirthdateValidation.validate(account.BirthDate).IsSuccessful == false)
            {
                result.ErrorMessage = "Email provided is invalid. Retry or contact admin.";
                return Implementation.BirthdateValidation.validate(account.BirthDate);
            }
            
            //email validation
            if (EmailValidation.validate(account.Email).IsSuccessful == false)
            {
                return EmailValidation.validate(account.Email);
            }

            //passphrase validation
            if (PassphraseValidation.validate(account.Hash).IsSuccessful == false)
            {
                return PassphraseValidation.validate(account.Hash);
            }

            //unused email
            Dictionary<string, object> emailValue = new()
            {
                { "Email", account.Email }
            };
            var selectAccount = await _registrationDAO.SelectAccount(new List<String> { "COUNT(Email)" }, emailValue).ConfigureAwait(false);

            if (selectAccount is not null)
            {
                if (selectAccount.Payload is not null)
                {
                    List<List<object>> payload = (List<List<object>>)selectAccount.Payload;
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
                { "Username", username }
            };
            selectAccount = await _registrationDAO.SelectAccount( new List<string> { "COUNT(Username)" }, usernameValue).ConfigureAwait(false);

            //assign id to account based on username check
            string tempUsername = "";
			if (selectAccount is not null)
            {
                if (selectAccount.Payload is not null)
                {
                    List<List<object>> payload = (List<List<object>>)selectAccount.Payload;
                    account.Id = (int)payload[0][0] + 1;
                    account.Username = username;
                    tempUsername = username + account.Id;
                }
            }

            //get hash and salt for passphrase
            HashPassphrase(account.Hash, out string passphraseHash, out string passphraseSalt);

            account.Hash = passphraseHash;
            account.Salt = passphraseSalt;

            //generate dictionary [String (column name), Object (value)
            Dictionary<String, Object> values = DictonaryConversion.ObjectToDictionary(account);

            //insert account
            var insertAccount = await _registrationDAO.InsertAccount(values).ConfigureAwait(false);
            
            if (insertAccount.IsSuccessful == false)
            {
                return insertAccount;
            }


            return new Result(true, "Account created successfully", tempUsername);
            
        }
        public void HashPassphrase(string passphrase, out string passphraseHash, out string passphraseSalt)
        {
            var result = new Result();
            using (var hmac = new HMACSHA512())
            {
                passphraseSalt = Convert.ToBase64String(hmac.Key);
                passphraseHash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passphrase)));
            }
        }

    }
}