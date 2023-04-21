using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using System.Globalization;
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

        public Result ValidateCollaboratorAllowEmptyFiles(CollaboratorProfile collab)
        {
            Result result = new Result();
            result.IsSuccessful = false;

            // checking for null
            if (collab == null)
            {
                result.ErrorMessage = "Collaborator profile is null.";
                return result;
            }

            // checking Name requirements
            if (string.IsNullOrEmpty(collab.Name))
            {
                result.ErrorMessage = "Name does not meet system requirements.";
                return result;
            }
            if (collab.Name.Length > 70)
            {
                result.ErrorMessage = "Name length does not meet system requirements.";
                return result;
            }

            // checking contact information requirements
            if (string.IsNullOrEmpty(collab.ContactInfo))
            {
                result.ErrorMessage = "Contact Info does not meet system requirements.";
                return result;
            }
            if (collab.ContactInfo.Length > 1000)
            {
                result.ErrorMessage = "Contact information length does not meet system requirements.";
                return result;
            }

            // checking Tag requirements
            // tags are optional
            if (collab.Tags != null && collab.Tags.Length > 2000)
            {
                result.ErrorMessage = "Cumulative Tag length does not meet system requirements.";
                return result;
            }
            if (collab.Tags != null)
            {
                string[] TagSplit = collab.Tags.Split(',');
                if (TagSplit.Length > 20)
                {
                    result.ErrorMessage = "Tag count does not meet system requirements.";
                    return result;
                }
                foreach (string Tag in TagSplit)
                {
                    if (Tag.Length > 100)
                    {
                        result.ErrorMessage = "Individual Tag length does not meet system requirements.";
                        return result;
                    }
                }
            }

            // checking Description requirements
            if (string.IsNullOrEmpty(collab.Description))
            {
                result.ErrorMessage = "Description does not meet system requirements.";
                return result;
            }
            if (collab.Description.Length > 10000)
            {
                result.ErrorMessage = "Description length does not meet system requirements.";
                return result;
            }

            // checking Availability requirements
            // availability is optional
            if (collab.Availability != null && collab.Availability.Length > 10000)
            {
                result.ErrorMessage = "Availability length does not meet system requirements.";
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public Result ValidateCollaborator(CollaboratorProfile collab)
        {
            Result result = new Result();
            result.IsSuccessful = false;

            // Regex made to match urls
            // may or may not have https, www, subdomain, directory path
            // must have domain

            Result generalValidation = ValidateCollaboratorAllowEmptyFiles(collab);
            if (!generalValidation.IsSuccessful)
            {
                result.ErrorMessage = generalValidation.ErrorMessage;
                return result;
            }

            Regex regex = new(@"^((https?)://)?(www.)?[a-z0-9]+(.[a-z]+)(/[a-zA-Z0-9#]+/?)*/?$");

            // checking profile picture requirements
            // pfp is optional
            if (collab.PfpUrl != null && !regex.IsMatch(collab.PfpUrl))
            {
                result.ErrorMessage = "Profile picture url provided does not meet expected format.";
                return result;
            }

            // checking uploaded file requirements
            if (collab.CollabUrls == null)
            {
                result.ErrorMessage = "Uploaded file urls do not meet system requirements.";
                return result;
            }

            if (collab.CollabUrls.Count > 10 || collab.CollabUrls.Count == 0)
            {
                result.ErrorMessage = "Collaborator must contain within 1-10 uploaded files.";
                return result;
            }

            foreach (string collabUrl in collab.CollabUrls)
            {
                if (string.IsNullOrEmpty(collabUrl))
                {
                    result.ErrorMessage = "An uploaded file url does not meet system requirements.";
                    return result;
                }
                if (!regex.IsMatch(collabUrl))
                {
                    result.ErrorMessage = "An uploaded file url does not meet expected format.";
                    return result;
                }
            }

            result.IsSuccessful = true;
            return result;
        }

    }
}