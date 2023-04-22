
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using System.Diagnostics;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Reflection;
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

        //passphrase satisfies length and format
        //length: min 8 character
        //format: valid characters:
        //blank space
        //a - z
        //A - Z
        //0 - 9
        //.,@!-
        public Result ValidatePhoneNumber(string phoneNumber)
        {
            Result result = new Result();
            result.IsSuccessful = false;
            Regex regex = new(@"^[0-9]+$");
            if (!regex.IsMatch(phoneNumber) || phoneNumber.Length < 10 || phoneNumber.Length > 11)
            {
                result.ErrorMessage = "Phone number provided is invalid. Retry or contact admin.";
                return result;
            }
            result.IsSuccessful = true;
            return result;
        }

        public Result ValidateModel(Object obj)
        {
            Result result = new Result();
            result.IsSuccessful = false;
            List<string> nullValueNames = new();

            foreach (PropertyInfo prop in obj.GetType().GetProperties())
            {
                var value = prop.GetValue(obj, null);
                if (prop.Name == "AverageRating") continue;
                if (value is null || value.ToString()!.Length < 1)
                {
                    nullValueNames.Add(prop.Name);
                }
            }
            if (nullValueNames.Count > 0)
            {
                result.ErrorMessage = "Missing value(s) for " + string.Join(", ", nullValueNames);
                result.ErrorMessage.Trim();
                return result;
            }
            result.IsSuccessful = true;
            return result;
        }


        public Result ValidateBodyText(string input)
        {
            Result result = new Result();
            result.IsSuccessful = false;

            Regex regex = new(@"[^a-zA-Z0-9\s'.,!()\]+]");

            if (regex.IsMatch(input))
            {
                result.ErrorMessage = "Invalid characters or special characters. Note only apostrophes, commas, periods, exclamation marks, and parentheses are the only special characters allowed.";
                return result;
            }

            result.IsSuccessful = true;
            return result;

        }

        //change
        public Result ValidateTitle(string title)
        {
            Result result = new Result();
            result.IsSuccessful = false;

            Regex regex = new(@"[^a-zA-Z0-9\s']+");

            if (regex.IsMatch(title))
            {
                result.ErrorMessage = "Invalid characters or special characters. Note apostrophes are the only special characters allowed.";
                return result;
            }

            result.IsSuccessful = true;
            return result;

        }

        public Result ValidateRating(int rating)
        {
            Result result = new Result();
            result.IsSuccessful = false;


            if (rating < 0 || rating > 5)
            {
                result.ErrorMessage = "Invalid rating.";
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public Result ValidateAvailability(ListingAvailabilityDTO listingAvailability)
        {
            Result result = new Result();

            if (listingAvailability.StartTime.Day != listingAvailability.EndTime.Day)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Start time and end time may not be on different days.";
                return result;
            }

            if (listingAvailability.StartTime.Minute % 30 != 0)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Start time must be on the hour or half hour.";
                return result;
            }

            if (listingAvailability.EndTime.Minute % 30 != 0)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "End time must be on the hour or half hour.";
                return result;
            }

            if (listingAvailability.EndTime.CompareTo(listingAvailability.StartTime) <= 0)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "End time must be at least 30 mins past start time.";
                return result;
            }

            if (DateTime.Now.AddMinutes(30).CompareTo(listingAvailability.StartTime) > 0)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Start time must be more than 30 minutes of right now.";
                return result;
            }

            if (DateTime.Now.AddHours(1).CompareTo(listingAvailability.EndTime) > 0)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "End time must be more than 60 minutes of right now.";
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }
    }
}