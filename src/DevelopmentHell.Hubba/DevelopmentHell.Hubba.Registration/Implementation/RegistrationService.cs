using DevelopmentHell.Hubba.Registration.Implementation;
using DevelopmentHell.Hubba.SqlDataAccess;
using System.Configuration;
using System.Security.Cryptography;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;

namespace DevelopmentHell.Hubba.Registration
{
    public class RegistrationService
    {
        private static string _connectionString = String.Format(@"Server={0};Database=DevelopmentHell.Hubba.Accounts;Encrypt=false;User Id=DevelopmentHell.Hubba.SqlUser.Registration;Password=password", ConfigurationManager.AppSettings["AccountServer"]);
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
            if (PassphraseValidation.validate(account.PassphraseHash).IsSuccessful == false)
            {
                return PassphraseValidation.validate(account.PassphraseHash);
            }

            //unused email
            var selectAccount = await _registrationDAO.SelectAccount(new List<String> { "COUNT(Email)" }, new(){ new( "Email", "=", account.Email ) }).ConfigureAwait(false);

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
            selectAccount = await _registrationDAO.SelectAccount( new() { "COUNT(Username)" }, new(){ new("Username","=",username) }).ConfigureAwait(false);

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
            HashPassphrase(account.PassphraseHash, out string passphraseHash, out string passphraseSalt);

            account.PassphraseHash = passphraseHash;
            account.PassphraseSalt = passphraseSalt;

            //generate dictionary [String (column name), Object (value)
            Dictionary<String, Object> values = DictionaryConversion.ObjectToDictionary(account);

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