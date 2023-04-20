using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Notification.Service.Abstractions;
using DevelopmentHell.Hubba.Scheduling.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using System.Diagnostics.Tracing;
using System.Security.Claims;

namespace DevelopmentHell.Hubba.Scheduling.Manager
{
    public class SchedulingManager : ISchedulingManager
    {
        //private readonly IValidationService _validationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILoggerService _loggerService;
        private readonly INotificationService _notificationService;

        private readonly IBookingService _bookingService;
        private readonly IAvailabilityService _availabilityService;

        public SchedulingManager(IBookingService bookingService, IAvailabilityService availabilityService, IAuthorizationService authorizationService, INotificationService notificationService, ILoggerService loggerService)
        {
            _bookingService = bookingService;
            _availabilityService = availabilityService;
            _authorizationService = authorizationService;
            _notificationService = notificationService;
            _loggerService = loggerService;
        }
        public Task<Result<bool>> CancelBooking(int userId, int bookingId)
        {
            //TODO implement
            throw new NotImplementedException();
        }

        public Task<Result<List<Tuple<DateTime,DateTime>>>> FindListingAvailabiityByMonth(int listingId, int month, int year)
        {
            //TODO implement
            throw new NotImplementedException();
        }
        /// <summary>
        /// Verified User request to reserve a booking
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="listingId"></param>
        /// <param name="fullPrice"></param>
        /// <param name="bookingStatus"></param>
        /// <param name="chosenTimeframes"></param>
        /// <returns>a BookingId:int in result.Payload</returns>
        public async Task<Result<int>> ReserveBooking(int userId, int listingId, float fullPrice, List<BookedTimeFrame> chosenTimeframes, BookingStatus? bookingStatus = BookingStatus.CONFIRMED)
        {
            Result<int> result = new() { IsSuccessful = false };
            
            /* Validate input */

            /* Authorize user */
            var authzResult = _authorizationService.Authorize(new string[] { "AdminUser", "VerifiedUser" });
            if (!authzResult.IsSuccessful)
            {
                result.ErrorMessage = authzResult.ErrorMessage;
                return result;
            }
            
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                result.ErrorMessage = "Error, invalid access token format.";
                return result;
            }
            // extracted user Id from JWT token
            var accountId = int.Parse(stringAccountId);

            /* Owner can't book their own listing */
            // get OwnerID by ListingId
            var getOwnerId = await _availabilityService.GetOwnerId(listingId).ConfigureAwait(false);
            int ownerId = getOwnerId.Payload;
            if(userId == ownerId)
            {
                result.ErrorMessage = "Owner can't book their own listing";
                return result;
            }

            /* Check timeframes have not already booked */
            // check each chosen time frame against the BookedTimeFrames table
            foreach (var timeframe in chosenTimeframes)
            {
                var isBooked = await _availabilityService.AreTimeFramesBooked(
                    listingId, 
                    (int)timeframe.AvailabilityId,
                    new List<BookedTimeFrame>() { timeframe }
                    ).ConfigureAwait(false);
                if (!isBooked.IsSuccessful) //timeframe already booked
                {
                    result.ErrorMessage = isBooked.ErrorMessage;
                    return result;
                }
            }

            /* Add a new booking */
            Booking booking = new()
            {
                UserId = userId,
                ListingId = listingId,
                FullPrice = fullPrice,
                BookingStatusId = BookingStatus.CONFIRMED,
                TimeFrames = chosenTimeframes
            };

            var createBooking = await _bookingService.AddNewBooking(booking).ConfigureAwait(false);

            if(!createBooking.IsSuccessful)
            {
                result.ErrorMessage = "Scheduling Error. Booking failed to complete. Refresh page or try again later";
                // log system error
                _loggerService.Log(LogLevel.ERROR, Category.BUSINESS, createBooking.ErrorMessage);
                return result;
            }
            // Process payload
            booking.BookingId = createBooking.Payload;

            /* Notify user and listing owner */
            var userNotificationMessage = string.Format("Booking #{0} confirmed", booking.BookingId);
            var notifyUser = await _notificationService.AddNotification(userId, "", NotificationType.SCHEDULING).ConfigureAwait(false);
            
            var ownerNotificationMessage = string.Format("Booking #{0} confirmed for your listing {0}", booking.BookingId, booking.ListingId);            
            var notifyOwner = await _notificationService.AddNotification(userId, "", NotificationType.SCHEDULING).ConfigureAwait(false);

            /* Log reservation */
            string logString = string.Format("Booking #{0} confirmed", booking.BookingId);
            _loggerService.Log(LogLevel.INFO, Category.BUSINESS, logString);

            result.IsSuccessful = true;
            result.Payload = booking.BookingId;
            return result;
        }
        
    }
}