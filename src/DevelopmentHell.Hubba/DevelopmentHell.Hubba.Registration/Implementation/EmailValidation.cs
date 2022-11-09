using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Registration.Implementation
{
    public class EmailValidation
    {

        public static Result validate(string email)
        {
            Result result = new Result();
            result.IsSuccessful = false;
            string error = "Email provided is invalid. Retry or contact admin.";
            if (email == string.Empty)
            {
                result.ErrorMessage = error;
                return result;
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
    }
}
