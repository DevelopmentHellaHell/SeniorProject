using System.Globalization;
using System.Text.RegularExpressions;

namespace DevelopmentHell.Hubba.Registration
{
    public class Result
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public object Payload { get; set; } = null;
    }

    public class RegistrationManager
    {
        public string Email { get; set; } = string.Empty;
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
            var result = new Result();
            if (Email == string.Empty)
            {
                result.IsValid = false;
                result.ErrorMessage = "Please provide a valid email";
                return result;
            }
            //result.IsValid = new EmailAddressAttribute().IsValid(Email);
            //return result;
            string regex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

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
                result.IsValid = false;
                result.ErrorMessage = "Can't validate email";
                return result;
            }
            catch (ArgumentException e)
            {
                result.IsValid = false;
                result.ErrorMessage = "Domain name is invalid";
                return result;
            }
            // check if Email and Regex match
            try
            {
                if (Regex.IsMatch(Email, regex,
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
                {
                    result.IsValid = true;
                }
                else
                {
                    result.IsValid = false;
                    result.ErrorMessage = "Email provided is not valid. Retry with a different one.";
                }
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
            //TODO: passphrase satisfies length and format
            //length: min 8 character
            //format: contains valid characters:
            //blank space
            //a - z A - Z 0 - 9
            //.,@!-

            var result = new Result();
            result.IsValid = true;

            return result;
        }
        //method call registration service
    }
}
// References:
// Email validation: https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format