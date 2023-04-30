
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Globalization;
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

            Regex regex = new(@"^((https?)://)?((www.)?[a-z0-9]+(.[a-z]+)|(([0-9]{1,3}.){3}([0-9]{1,3})))(/[a-zA-Z0-9#.]+/?)*/?$");

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

        public Result ValidateImageFile(IFormFile file)
        {
            Result result = new Result();
            if (!Regex.IsMatch(file.FileName, @"^[a-zA-Z0-9_. ]*$"))
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "File names must consist of only letters, numbers, spaces, and underscores.";
                result.StatusCode = StatusCodes.Status412PreconditionFailed;
                return result;
            }
            if (file == null || file.Length == 0)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = $"File {file!.FileName} is empty";
                result.StatusCode = StatusCodes.Status412PreconditionFailed;
                return result;
            }
            if (file.ContentType.StartsWith("image/"))
            {
                string fileExtension = Path.GetExtension(file.FileName);
                if (fileExtension == ".jpeg" || fileExtension == ".jpg" || fileExtension == ".png")
                {
                    // Check if the image size is less than or equal to 25 MB
                    if (file.Length > 25 * 1024 * 1024)
                    {
                        result.IsSuccessful = false;
                        result.ErrorMessage = file.FileName + " is too large.";
                        result.StatusCode = StatusCodes.Status412PreconditionFailed;
                        return result;
                    }
                }
                else
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = file.FileName + " is an invalid image file type.";
                    result.StatusCode = StatusCodes.Status412PreconditionFailed;
                    return result;
                }
            }
            else
            {
                result.IsSuccessful = false;
                result.ErrorMessage = file.FileName + " is an invalid file type.";
                result.StatusCode = StatusCodes.Status412PreconditionFailed;
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
                if (obj is ListingViewDTO && prop.Name == "AverageRating") continue;
                if ((obj is ListingAvailabilityDTO && prop.Name == "OwnerId") || (obj is ListingAvailabilityDTO && prop.Name == "Action")) continue;
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

            if (Convert.ToDateTime(listingAvailability.StartTime).Day != Convert.ToDateTime(listingAvailability.EndTime).Day)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Start time and end time may not be on different days.";
                return result;
            }

            if (Convert.ToDateTime(listingAvailability.StartTime).Minute % 30 != 0)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Start time must be on the hour or half hour.";
                return result;
            }

            if (Convert.ToDateTime(listingAvailability.EndTime).Minute % 30 != 0)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "End time must be on the hour or half hour.";
                return result;
            }

            if (Convert.ToDateTime(listingAvailability.EndTime).CompareTo(Convert.ToDateTime(listingAvailability.StartTime)) <= 0)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "End time must be at least 30 mins past start time.";
                return result;
            }

            if (DateTime.Now.AddMinutes(30).CompareTo(Convert.ToDateTime(listingAvailability.StartTime)) > 0)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Start time must be more than 30 minutes of right now.";
                return result;
            }

            if (DateTime.Now.AddHours(1).CompareTo(Convert.ToDateTime(listingAvailability.EndTime)) > 0)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "End time must be more than 60 minutes of right now.";
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public Result ValidateFiles(List<Tuple<string, string>> files)
        {
            Result result = new Result();

            foreach (Tuple<string, string> file in files)
            {
                string extension = Path.GetExtension(file.Item1).ToLower();

                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                {
                    using (var stream = new MemoryStream(Convert.FromBase64String(file.Item2)))
                    using (var image = System.Drawing.Image.FromStream(stream))
                    {
                        if (image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg) ||
                            image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                        {
                            // Check if the image size is less than or equal to 25 MB
                            if (Convert.FromBase64String(file.Item2).Length > 25 * 1024 * 1024)
                            {
                                result.IsSuccessful = false;
                                result.ErrorMessage = file.Item1 + " is too large.";
                                return result;
                            }
                        }
                        else
                        {
                            result.IsSuccessful = false;
                            result.ErrorMessage = file.Item1 + " is an invalid image file type.";
                            return result;
                        }
                    }
                }
                else if (extension == ".mp4")
                {
                    // Check if the video size is less than or equal to 300 MB
                    if (Convert.FromBase64String(file.Item2).Length > 300 * 1024 * 1024)
                    {
                        result.IsSuccessful = false;
                        result.ErrorMessage = file.Item1 + " is too large.";
                        return result;
                    }
                }
                else
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = file.Item1 + " is an invalid file type.";
                    return result;
                }
            }

            result.IsSuccessful = true;
            return result;
        }

        public Result ValidateFile(IFormFile file)
        {
            Result result = new Result();


            if (!Regex.IsMatch(file.FileName, @"^[a-zA-Z0-9_.]*$"))
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "File names must consist of only letters, numbers, and underscores.";
                return result;
            }

            if (file == null || file.Length == 0)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = $"File {file.FileName} is empty";
                return result;
            }

            if (!file.ContentType.StartsWith("image/") && file.ContentType != "video/mp4")
            {
                result.IsSuccessful = false;
                result.ErrorMessage = file.FileName + " is not a valid file type.";
                return result;
            }

            if (file.ContentType.StartsWith("image/"))
            {
                using (var image = System.Drawing.Image.FromStream(file.OpenReadStream()))
                {
                    if (image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg) ||
                        image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                    {
                        // Check if the image size is less than or equal to 25 MB
                        if (file.Length > 25 * 1024 * 1024)
                        {
                            result.IsSuccessful = false;
                            result.ErrorMessage = file.FileName + " is too large.";
                            return result;
                        }
                    }
                    else
                    {
                        result.IsSuccessful = false;
                        result.ErrorMessage = file.FileName + " is an invalid image file type.";
                        return result;
                    }
                }
            }
            else if (file.ContentType == "video/mp4")
            {
                // Check if the video size is less than or equal to 300 MB
                if (file.Length > 300 * 1024 * 1024)
                {
                    result.IsSuccessful = false;
                    result.ErrorMessage = file.FileName + " is too large.";
                    return result;
                }
            }
            else
            {
                result.IsSuccessful = false;
                result.ErrorMessage = file.FileName + " is an invalid file type.";
                return result;
            }
            

            result.IsSuccessful = true;
            return result;
        }
    }
}