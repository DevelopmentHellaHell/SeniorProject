using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.Notification.Service.Abstractions;
using DevelopmentHell.Hubba.Scheduling.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration.UserSecrets;
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
        private async Task<Result> AuthorizeUser (int userId)
        {
            var authzResult = _authorizationService.Authorize(new string[] { "AdminUser", "VerifiedUser" });
            if (!authzResult.IsSuccessful)
            {
                return new(Result.Failure(authzResult.ErrorMessage, StatusCodes.Status401Unauthorized));
            }

            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
            if (stringAccountId is null)
            {
                return new(Result.Failure("Error, invalid access token format."));
            }
            // extracted user Id from JWT token
            if (int.TryParse(stringAccountId, out int accountId))
            {

                if (userId != accountId)
                {
                    return new(Result.Failure("Unsupported operation. User can't book on other's behalf", StatusCodes.Status400BadRequest));
                }
            }
            return new(Result.Success());
        }
        private async Task<Result> NotifyUsers(int bookingId, int userId, int ownerId, BookingStatus status)
        {
            // Notify user and listing owner
            var userNotificationMessage = string.Format("Booking #{0} {1}", bookingId, status.ToString());
            var notifyUser = await _notificationService.AddNotification(userId, userNotificationMessage, NotificationType.SCHEDULING).ConfigureAwait(false);
            if (!notifyUser.IsSuccessful)
            {
                // failed to notify user, rollback delete inserted booking
                var deleteIncompleteBooking = await _bookingService.DeleteIncompleteBooking(bookingId).ConfigureAwait(false);
                if (!deleteIncompleteBooking.IsSuccessful)
                {
                    return new(Result.Failure("System error. Please contact admin.", deleteIncompleteBooking.StatusCode));
                }
                _loggerService.Log(LogLevel.ERROR, Category.BUSINESS, notifyUser.ErrorMessage);
                return new(Result.Failure("Scheduling error. Please try again later or contact admin.", notifyUser.StatusCode));
            }

            var ownerNotificationMessage = string.Format("Booking #{0} {1} for one of your listings", bookingId, status.ToString());
            var notifyOwner = await _notificationService.AddNotification(ownerId, ownerNotificationMessage, NotificationType.SCHEDULING).ConfigureAwait(false);
            if (!notifyOwner.IsSuccessful)
            {
                // failed to notify owner, rollback delete inserted booking
                var deleteIncompleteBooking = await _bookingService.DeleteIncompleteBooking(bookingId).ConfigureAwait(false);
                if (!deleteIncompleteBooking.IsSuccessful)
                {
                    return new(Result.Failure("System error. Booking incomplete. Please contact admin."));
                }
                _loggerService.Log(LogLevel.ERROR, Category.BUSINESS, notifyUser.ErrorMessage);
                return new(Result.Failure("Notification error. Booking not complete. Please try again later or contact admin.", notifyOwner.StatusCode));
            }

            return new(Result.Success());
        }
        public async Task<Result<bool>> CancelBooking(int userId, int bookingId)
        {
            //TODO implement
            // Authorize user 
            var authzUser = await AuthorizeUser(userId).ConfigureAwait(false);
            if (!authzUser.IsSuccessful)
            {
                return new(Result.Failure(authzUser.ErrorMessage, authzUser.StatusCode));
            }
            // User must have a booking with BookingId
            var getBooking = await _bookingService.GetBookingByBookingId(bookingId).ConfigureAwait(false);
            if (!getBooking.IsSuccessful || getBooking.Payload == null) 
            {
                return new(Result.Failure(getBooking.ErrorMessage, getBooking.StatusCode));
            }
            if(userId != getBooking.Payload.UserId)
            {
                return new(Result.Failure("Booking not found. Unable to make the cancellation.", StatusCodes.Status400BadRequest));
            }
            // get Listing ownerId
            var getListingDetails = await _availabilityService.GetListingDetails(getBooking.Payload.ListingId).ConfigureAwait(false);
            if (!getListingDetails.IsSuccessful)
            {
                return new(Result.Failure(getListingDetails.ErrorMessage, getListingDetails.StatusCode));
            }
            int ownerId = (int) getListingDetails.Payload.OwnerId;

            // Cancel booking
            var cancelBooking = await _bookingService.CancelBooking(bookingId).ConfigureAwait (false);
            if(!cancelBooking.IsSuccessful)
            {
                return new(Result.Failure(cancelBooking.ErrorMessage, cancelBooking.StatusCode));
            }

            // Notify User and Listing's owner
            var notifyUsers = await NotifyUsers(bookingId, userId, ownerId, BookingStatus.CANCELLED).ConfigureAwait(false);
            if (!notifyUsers.IsSuccessful)
            {
                return new(Result.Failure(notifyUsers.ErrorMessage, notifyUsers.StatusCode));
            }
            return new(Result.Success());
        }

        public async Task<Result<List<Tuple<DateTime,DateTime>>>> FindListingAvailabiityByMonth(int listingId, int month, int year)
        {
            var getOpenTimeSlots = await _availabilityService.GetOpenTimeSlotsByMonth(listingId, month, year).ConfigureAwait(false);
            // TODO: what if someone else booked during this searching?
            if (!getOpenTimeSlots.IsSuccessful || getOpenTimeSlots.Payload == null)
            {
                return new(Result.Failure(getOpenTimeSlots.ErrorMessage, getOpenTimeSlots.StatusCode));
            }
            return Result<List<Tuple<DateTime,DateTime>>>.Success(getOpenTimeSlots.Payload);
        }

        public async Task<Result<BookingViewDTO>> ReserveBooking(int userId, int listingId, float fullPrice, List<BookedTimeFrame> chosenTimeframes, BookingStatus bookingStatus = BookingStatus.CONFIRMED)
        {
            // Authorize user 
            var authzUser = await AuthorizeUser(userId).ConfigureAwait(false);
            if(!authzUser.IsSuccessful)
            {
                return new (Result.Failure(authzUser.ErrorMessage, authzUser.StatusCode));
            }
            // Owner can't book their own listing
            // get OwnerID by ListingId
            var getListingDetails = await _availabilityService.GetListingDetails(listingId).ConfigureAwait(false);
            if (!getListingDetails.IsSuccessful || getListingDetails.Payload == null)
            {
                return new(Result.Failure("No listing found.", StatusCodes.Status400BadRequest));
            }
            int ownerId = (int) getListingDetails.Payload.OwnerId;
            if(userId == ownerId)
            {
                return new (Result.Failure("Owner can't book their own listing", StatusCodes.Status400BadRequest));
            }

            // Check timeframes have not already booked
            // check each chosen time frame against the BookedTimeFrames table
            foreach (var timeframe in chosenTimeframes)
            {
                var isOpentoBook = await _availabilityService.ValidateChosenTimeFrames(
                    listingId, 
                    (int)timeframe.AvailabilityId,
                    timeframe
                    ).ConfigureAwait(false);
                if (!isOpentoBook.IsSuccessful) //timeframe already booked
                {
                    return new(Result.Failure(isOpentoBook.ErrorMessage, isOpentoBook.StatusCode));
                }
            }

            // Add a new booking
            Booking booking = new()
            {
                UserId = userId,
                ListingId = listingId,
                FullPrice = fullPrice,
                BookingStatusId = BookingStatus.CONFIRMED,
                TimeFrames = chosenTimeframes
            };

            var createBooking = await _bookingService.AddNewBooking(booking).ConfigureAwait(false);

            if(!createBooking.IsSuccessful || createBooking.Payload == 0)
            {
                // failed to create new booking
                _loggerService.Log(LogLevel.ERROR, Category.BUSINESS, createBooking.ErrorMessage);
                return new(Result.Failure("Scheduling error. Booking not created. Refresh page or try again later", createBooking.StatusCode));
            }
            // Process payload
            int bookingId = createBooking.Payload;

            // Notify User and Host
            var notifyUsers = await NotifyUsers(bookingId, userId, ownerId, BookingStatus.CONFIRMED).ConfigureAwait(false);
            if (!notifyUsers.IsSuccessful)
            {
                return new(Result.Failure(notifyUsers.ErrorMessage, notifyUsers.StatusCode));
            }

            // Log successful booking
            string logString = string.Format("User #{0} booked Listing #{1}, Booking #{2}", userId, listingId, bookingId);
            _loggerService.Log(LogLevel.INFO, Category.BUSINESS, logString);

            // Prep confirmation for Booking View
            var confirmation = getListingDetails.Payload;
            confirmation.UserId = userId;
            confirmation.BookingId = bookingId;
            confirmation.FullPrice = booking.FullPrice;
            confirmation.BookedTimeFrames = chosenTimeframes;
            
            return Result<BookingViewDTO>.Success(confirmation);
        }
    }
}