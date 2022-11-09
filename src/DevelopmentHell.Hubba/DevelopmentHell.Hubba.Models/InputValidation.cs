using System.Globalization;
using System.Security.Principal;
using System.Text.RegularExpressions;
using DevelopmentHell.Hubba.Registration;
using static System.Net.Mime.MediaTypeNames;

namespace DevelopmentHell.Hubba.Models
{
    public class InputValidation
    {
        private Result _result = new Result();
        public InputValidation()
        {
            _result.IsSuccessful = false;
            _result.ErrorMessage = "";
        }

        public Result ValidateEmail(string email)
        {
            _result.IsSuccessful = false;
            if (email == string.Empty)
            {
                _result.ErrorMessage = "Please provide a valid email";
                return _result;
            }
            //result.IsSuccessful = new EmailAddressAttribute().IsSuccessful(Email);
            //return result;
            string regex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
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
            catch (RegexMatchTimeoutException)
            {
                _result.ErrorMessage = "Can't validate email";
                return _result;
            }
            catch (ArgumentException)
            {
                _result.ErrorMessage = "Domain name is invalid";
                return _result;
            }
            // check if Email and Regex match
            try
            {
                if (Regex.IsMatch(email, regex,
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
                {
                    _result.IsSuccessful = true;
                }
                else
                {
                    _result.ErrorMessage = "Email provided is not valid. Retry with a different one.";
                }
                return _result;
            }
            catch (RegexMatchTimeoutException)
            {
                _result.ErrorMessage = "Can't validate email";
                return _result;
            }
        }

        public Result ValidateBirthdate(DateTime birthdate)
        {
            DateTime today = DateTime.Now;
            if (today.Subtract(birthdate).Days < (365 * 14))
            {
                _result.IsSuccessful = false;
                _result.ErrorMessage = "Age requirement not reached.";
                return _result;
            }
            _result.IsSuccessful = true;

            return _result;
        }

        public Result ValidatePassphrase(string passphrase)
        {
            //passphrase satisfies length and format
            //length: min 8 character
            //format: valid characters:
            //blank space
            //a - z
            //A - Z
            //0 - 9
            //.,@!-

            Regex rx = new(@"[^A-Za-z0-9.,@! -]");
            if (rx.IsMatch(passphrase) || passphrase.Length <= 8)
            {
                _result.IsSuccessful = false;
                _result.ErrorMessage = "Passphrase provided is invalid. Retry or contact admin.";
                return _result;
            }
            _result.IsSuccessful = true;
            return _result;

        }
    }
}