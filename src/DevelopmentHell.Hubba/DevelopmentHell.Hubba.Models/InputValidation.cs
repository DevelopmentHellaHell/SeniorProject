using System.Globalization;
using System.Security.Principal;
using System.Text.RegularExpressions;

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

        public Result ValidatePassphrase(string passphrase)
        {
            //passphrase satisfies length and format
            //length: min 8 character
            //format: contains valid characters:
            //blank space
            //a - z
            //A - Z
            //0 - 9
            //.,@!-

            // check off each contraint, once validationPoints == 6, passphrase is valid
            var validationPoints = 0;
            _result.ErrorMessage = "Invalid passphrase.";
            _result.IsSuccessful = false;

            //1) length check
            if (passphrase.Length < 7)
            {
                return _result;
            }
            validationPoints++;

            //2) blank space
            if (!passphrase.Contains(' '))
            {
                return _result;
            }
            validationPoints++;

            //3) a-z
            foreach (var c in passphrase)
            {
                if (c >= 'a' && c <= 'z')
                {
                    validationPoints++;
                    break;
                }
            }
            if (validationPoints != 3)
            {
                return _result;
            }

            //4) A-Z
            foreach (var c in passphrase)
            {
                if (c >= 'a' && c <= 'z')
                {
                    validationPoints++;
                    break;
                }
            }
            if (validationPoints != 4)
            {
                return _result;
            }

            //5) 0-9
            foreach (var c in passphrase)
            {
                if (c >= 'a' && c <= 'z')
                {
                    validationPoints++;
                    break;
                }
            }
            if (validationPoints != 5)
            {
                return _result;
            }
            //6) .,@!-
            char[] special = { ' ', '.', ',', '@', '!', '-' };
            if (passphrase.IndexOfAny(special) == -1)
            {
                return _result;
            }
            validationPoints++;

            if (validationPoints == 6)
            {
                _result.IsSuccessful = true;
                _result.ErrorMessage = "";
            }
            return _result;
        }
    }
}