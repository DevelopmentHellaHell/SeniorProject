using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DevelopmentHell.Hubba.Validation.Service.Implementations
{
    public class ValidationService : IValidationService
    {
        public ValidationService()
        {

        }

        public Result ValidateEmail(string email)
        {
            Result result = new Result();
            result.IsSuccessful = false;
            string error = "Email provided is invalid. Retry or contact admin.";
            Regex rx = new(@"[^a-z0-9.@-]");
            if (rx.IsMatch(email) || email.Length < 8 || email.Length > 127)
            {
                result.ErrorMessage = error;
                return result;
            }

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
                result.ErrorMessage = error;
                return result;
            }
            catch (ArgumentException)
            {
                result.ErrorMessage = error;
                return result;
            }
            // check if Email and Regex match
            try
            {
                if (Regex.IsMatch(email, regex,
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
                {
                    result.IsSuccessful = true;
                }
                else
                {
                    result.ErrorMessage = error;
                }
                return result;
            }
            catch (RegexMatchTimeoutException)
            {
                result.ErrorMessage = error;
                return result;
            }
        }

        //passphrase satisfies length and format
        //length: min 8 character
        //format: valid characters:
        //blank space
        //a - z
        //A - Z
        //0 - 9
        //.,@!-
        public Result ValidatePassword(string password)
        {
            Result result = new Result();
            result.IsSuccessful = false;
            Regex rx = new(@"[^A-Za-z0-9.,@! -]");
            if (rx.IsMatch(password) || password.Length < 8 || password.Length > 127)
            {
                result.ErrorMessage = "Password provided is invalid. Retry or contact admin.";
                return result;
            }
            result.IsSuccessful = true;
            return result;
        }

    }
}