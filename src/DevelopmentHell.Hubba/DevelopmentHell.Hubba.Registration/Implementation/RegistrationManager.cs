using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DevelopmentHell.Hubba.Registration
{
    public class Result
    {
        public bool IsValid { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;
        public object Payload { get; set; } = null;
    }

    public class RegistrationManager
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(8)]
        public string Passphrase { get; set; } = string.Empty;

        public RegistrationManager() { }

        public RegistrationManager(string email, string passphrase)
        {
                Email = email;
                Passphrase = passphrase;
        }

        public Result ValidateEmail()
        {
            //TODO: email contains only 1 '@', in format: first@second.third
            // first: letters, numbers, periods
            // second: subdomain, letters
            // third: domain, letters
            var result = new Result();
            if (Email == string.Empty)
            { 
                result.ErrorMessage = "Please provide a valid email";
                return result;
            }
            string regex = @"^[^@\s] + @ [^@\s] + \.[^@\s] + $";

            try
            {
                // Normalize the domain
                Email = Regex.Replace(Email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                result.ErrorMessage = "Regex Timeout Exception";
                return result;
            }
            catch (ArgumentException e)
            {
                result.ErrorMessage = "Argument Exception";
                return result;
            }

            try
            {
                if (Regex.IsMatch(Email, regex,
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
                {
                    result.IsValid = true;
                }
                result.ErrorMessage = "Email provided is not valid. Retry with another one.";
                return result;
            }
            catch (RegexMatchTimeoutException)
            {
                result.ErrorMessage = "Can't validate email";
                return result;
            };
        }

        public Result ValidatePassphrase()
        {
            //TODO: passphrase and confirm passphrase must match
            //TODO: passphrase satisfies length and format
            var result = new Result();


            return result;
        }
        //method call registration service
    }
}
// References:
// Email regex: https://www.rhyous.com/2010/06/15/csharp-email-regular-expression/