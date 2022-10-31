using System.ComponentModel.DataAnnotations;

namespace DevelopmentHell.Hubba.Registration
{
    public class Result
    {
        public bool IsValid { get; set; } = true;
        public string ErrorMessage { get; set; } = String.Empty;
        //public object Payload { get; set; }
    }

    public class RegistrationManager
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(8)]
        public string Passphrase { get; set; } = string.Empty;
        [Required, Compare("Passphrase")]
        public string ConfirmPassphrase { get; set; } = string.Empty;

        public RegistrationManager() { }

        public RegistrationManager(string email, string passphrase)
        {
            Email = email;
            Passphrase = passphrase;
        }

        public Result ValidateEmail()
        {
            //TODO: email contains only 1 '@',
            //at least '.', a-z, 0-9, no other special char
            var result = new Result();
            if (Email.Count(x => x == '@') != 1) //using LINQ
            {
                result.IsValid = false;
                result.ErrorMessage = "Email provided is invalid. Retry with a different one.";
            }
            
            return result;
        }

        public Result ValidatePassphrase()
        {
            //TODO: passphrase and confirm passphrase must match
            //TODO: passphrase satisfies length and format
            var result = new Result();
            return result;
        }
    }
}