using DevelopmentHell.Hubba.Registration.Implementation;
using DevelopmentHell.Hubba.SqlDataAccess;
using System.Configuration;
using System.Security.Cryptography;
using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Registration
{
    public class RegistrationService
    {
        private Account _account;
        private static string _connectionString = String.Format(@"Server=localhost\SQLEXPRESS;Database=DevelopmentHell.Hubba.Accounts;Integrated Security=True;Encrypt=False", ConfigurationManager.AppSettings["AccountServer"]);
        private RegistrationDataAccess _registrationDAO;

        public RegistrationService(Account account)
        {
            _account = account;
            _registrationDAO = new RegistrationDataAccess(_connectionString);
        }

        public async Task<Result> RegisterAccount()
        {
            
            var result = new Result();
            result.IsSuccessful = false;

            //age validation
            if (Implementation.BirthdateValidation.validate(_account.BirthDate).IsSuccessful == false)
            {
                result.ErrorMessage = "Email provided is invalid. Retry or contact admin.";
                return Implementation.BirthdateValidation.validate(_account.BirthDate);
            }
            
            //email validation
            if (EmailValidation.validate(_account.Email).IsSuccessful == false)
            {
                return EmailValidation.validate(_account.Email);
            }

            //passphrase validation
            if (PassphraseValidation.validate(_account.PassphraseHash).IsSuccessful == false)
            {
                return PassphraseValidation.validate(_account.PassphraseHash);
            }

            //unused email
            Dictionary<string, object> emailValue = new()
            {
                { "Email", _account.Email }
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
                    _account.Id = (int)payload[0][0] + 1;
                    _account.Username = username;
                    tempUsername = username + _account.Id;
                }
            }

            //get hash and salt for passphrase
            HashPassphrase(_account.PassphraseHash, out string passphraseHash, out string passphraseSalt);

            _account.PassphraseHash = passphraseHash;
            _account.PassphraseSalt = passphraseSalt;

            //generate dictionary [String (column name), Object (value)
            Dictionary<String, Object> values = DictionaryConversion.ObjectToDictionary(_account);

            //insert account
            var insertAccount = await _registrationDAO.InsertAccount(values).ConfigureAwait(false);
            
            if (insertAccount.IsSuccessful == false)
            {
                insertAccount.ErrorMessage = "Username creation error. Please try again or contact admin.";
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