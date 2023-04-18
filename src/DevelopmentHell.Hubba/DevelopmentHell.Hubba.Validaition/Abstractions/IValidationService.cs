using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using Microsoft.Extensions.Options;

namespace DevelopmentHell.Hubba.Validation.Service.Abstractions
{
    public interface IValidationService
    {
        Result ValidateEmail(string email);
        Result ValidatePassword(string password);
        Result ValidatePhoneNumber(string phoneNumber);
        Result ValidateModel(Object obj);
        Result ValidateBodyText(string input);
        Result ValidateTitle(string title);

        Result ValidateRating(int rating);

        Result ValidateAvailability(ListingAvailabilityDTO listingAvailability);
    }
}

