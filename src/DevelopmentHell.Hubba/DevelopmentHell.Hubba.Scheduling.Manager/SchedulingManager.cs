using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Notification.Service.Abstractions;
using DevelopmentHell.Hubba.Scheduling.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using System.Security.Claims;

namespace DevelopmentHell.Hubba.Scheduling.Manager
{
    public class SchedulingManager : ISchedulingManager
    {
        private readonly IValidationService _validationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILoggerService _loggerService;
        private readonly INotificationService _notificationService;

        private readonly IBookingService _bookingService;
        private readonly IAvailabilityService _availabilityService;

        public SchedulingManager(IBookingService bookingService, IAvailabilityService availabilityService)
        {
            _bookingService = bookingService;
            _availabilityService = availabilityService;
        }
        public Task<Result> CancelBooking(int userId, int bookingId)
        {
            //TODO implement
            throw new NotImplementedException();
        }

        public Task<Result> FindListingAvailabiityByMonth(int listingId, int month, int year)
        {
            //TODO implement
            throw new NotImplementedException();
        }

        public async Task<Result> ReserveBooking(int userId, int listingId, float fullPrice, BookingStatus bookingStatus, int availabilityId, List<Tuple<DateTime,DateTime>> timeframes)
        {
            Result result = new() { IsSuccessful = false };
            //TODO implement
            /* Validate input */

            /* Authorize user */
            var authzResult = _authorizationService.Authorize(new string[] { "AdminUser", "VerifiedUser" });
            if (!authzResult.IsSuccessful)
            {
                return authzResult;
            }
            
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "Error, invalid access token format.";
                return result;
            }
            // extracted user Id from JWT token
            var accountId = int.Parse(stringAccountId);

            /* Owner can't book their own listing */
            // get OwnerID by ListingId
            var getOwnerId = _availabilityService.GetOwnerId(listingId).ConfigureAwait(false);
            return result;
        }
        
    }
}