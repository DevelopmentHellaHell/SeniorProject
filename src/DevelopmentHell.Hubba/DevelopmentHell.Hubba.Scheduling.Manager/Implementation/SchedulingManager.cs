using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.Notification.Service.Abstractions;
using DevelopmentHell.Hubba.Scheduling.Service.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DevelopmentHell.Hubba.Scheduling.Manager.Abstraction
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
		private Result AuthorizeUser(int userId)
		{
			var authzResult = _authorizationService.Authorize(new string[] { "AdminUser", "VerifiedUser" });
			if (!authzResult.IsSuccessful)
			{
				_loggerService.Log(LogLevel.ERROR, Category.DATA, authzResult.ErrorMessage!);
				return new(Result.Failure(authzResult.ErrorMessage!, StatusCodes.Status401Unauthorized));
			}

			var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
			var stringAccountId = claimsPrincipal?.FindFirstValue("sub");
			if (stringAccountId is null)
			{
				return new(Result.Failure("Error, invalid access token format."));
			}
			// Extracted user Id from JWT token
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
				// Failed to notify user, rollback delete inserted booking
				var deleteIncompleteBooking = await _bookingService.DeleteIncompleteBooking(bookingId).ConfigureAwait(false);
				if (!deleteIncompleteBooking.IsSuccessful)
				{
					return new(Result.Failure("System error. Please contact admin.", deleteIncompleteBooking.StatusCode));
				}
				_loggerService.Log(LogLevel.ERROR, Category.DATA, notifyUser.ErrorMessage!);
				return new(Result.Failure("Scheduling error. Please try again later or contact admin.", notifyUser.StatusCode));
			}

			var ownerNotificationMessage = string.Format("Booking #{0} {1} for one of your listings", bookingId, status.ToString());
			var notifyOwner = await _notificationService.AddNotification(ownerId, ownerNotificationMessage, NotificationType.SCHEDULING).ConfigureAwait(false);
			if (!notifyOwner.IsSuccessful)
			{
				// Failed to notify owner, rollback delete inserted booking
				var deleteIncompleteBooking = await _bookingService.DeleteIncompleteBooking(bookingId).ConfigureAwait(false);
				if (!deleteIncompleteBooking.IsSuccessful)
				{
					return new(Result.Failure("System error. Booking incomplete. Please contact admin."));
				}
				_loggerService.Log(LogLevel.ERROR, Category.DATA, notifyUser.ErrorMessage!);
				return new(Result.Failure("Notification error. Booking not complete. Please try again later or contact admin.", notifyOwner.StatusCode));
			}

			return new(Result.Success());
		}
		public async Task<Result<bool>> CancelBooking(int userId, int bookingId)
		{
			// TODO implement
			// Authorize user 
			var authzUser = AuthorizeUser(userId);
			if (!authzUser.IsSuccessful)
			{
				return new(Result.Failure(authzUser.ErrorMessage!, authzUser.StatusCode));
			}
			// User must have a booking with BookingId
			var getBooking = await _bookingService.GetBookingByBookingId(bookingId).ConfigureAwait(false);
			if (!getBooking.IsSuccessful || getBooking.Payload == null)
			{
				_loggerService.Log(LogLevel.ERROR, Category.DATA, getBooking.ErrorMessage!);
				return new(Result.Failure(getBooking.ErrorMessage!, getBooking.StatusCode));
			}
			if (userId != getBooking.Payload.UserId)
			{
				_loggerService.Log(LogLevel.ERROR, Category.BUSINESS, "UserId invalid for booking cancellation");
				return new(Result.Failure("Booking not found. Unable to make the cancellation.", StatusCodes.Status400BadRequest));
			}
			int listingId = getBooking.Payload.ListingId;

			// If Booking already cancelled
			if (getBooking.Payload.BookingStatusId == BookingStatus.CANCELLED)
			{
				_loggerService.Log(LogLevel.WARNING, Category.BUSINESS, getBooking.ErrorMessage!);
				return new(Result.Failure("Booking already cancelled.", StatusCodes.Status400BadRequest));
			}
			// Get Listing ownerId
			var getListingDetails = await _availabilityService.GetListingDetails(getBooking.Payload.ListingId).ConfigureAwait(false);
			if (!getListingDetails.IsSuccessful)
			{
				_loggerService.Log(LogLevel.ERROR, Category.DATA, getListingDetails.ErrorMessage!);
				return new(Result.Failure(getListingDetails.ErrorMessage!, getListingDetails.StatusCode));
			}
			int ownerId = (int)getListingDetails.Payload!.OwnerId!;

			// Cancel booking
			var cancelBooking = await _bookingService.CancelBooking(bookingId).ConfigureAwait(false);
			if (!cancelBooking.IsSuccessful)
			{
				_loggerService.Log(LogLevel.ERROR, Category.DATA, cancelBooking.ErrorMessage!);
				return new(Result.Failure(cancelBooking.ErrorMessage!, cancelBooking.StatusCode));
			}

			// Notify User and Listing's owner
			var notifyUsers = await NotifyUsers(bookingId, userId, ownerId, BookingStatus.CANCELLED).ConfigureAwait(false);
			if (!notifyUsers.IsSuccessful)
			{
				return new(Result.Failure(notifyUsers.ErrorMessage!, notifyUsers.StatusCode));
			}

			// Remove user from Listing History
			var removeUserFromListingHistory = await _bookingService.RemoveUserFromListingHistory(listingId, userId);
			if(!removeUserFromListingHistory.IsSuccessful)
			{
				_loggerService.Log(LogLevel.ERROR, Category.DATA, removeUserFromListingHistory.ErrorMessage!);
				return new (Result.Failure(removeUserFromListingHistory.ErrorMessage!, removeUserFromListingHistory.StatusCode));
			}

			return Result<bool>.Success(true);
		}

		public async Task<Result<List<ListingAvailabilityDTO>>> FindListingAvailabiityByMonth(int listingId, int month, int year)
		{
			var getOpenTimeSlots = await _availabilityService.GetOpenTimeSlotsByMonth(listingId, month, year).ConfigureAwait(false);
			// TODO: what if someone else booked during this searching?
			if (!getOpenTimeSlots.IsSuccessful || getOpenTimeSlots.Payload == null)
			{
				_loggerService.Log(LogLevel.ERROR, Category.DATA, getOpenTimeSlots.ErrorMessage!);
				return new(Result.Failure(getOpenTimeSlots.ErrorMessage!, getOpenTimeSlots.StatusCode));
			}
			return Result<List<ListingAvailabilityDTO>>.Success(getOpenTimeSlots.Payload);
		}

		public async Task<Result<BookingViewDTO>> ReserveBooking(int userId, int listingId, float fullPrice, List<BookedTimeFrame> chosenTimeframes, BookingStatus bookingStatus = BookingStatus.CONFIRMED)
		{
			// Time frame can't be null
			if (chosenTimeframes == null)
			{
				_loggerService.Log(LogLevel.ERROR, Category.DATA, "Invalid parameter");
				return new(Result.Failure("Time Frames can't be empty.", StatusCodes.Status400BadRequest));
			}
			// Authorize user 
			var authzUser = AuthorizeUser(userId);
			if (!authzUser.IsSuccessful)
			{
				return new(Result.Failure(authzUser.ErrorMessage!, authzUser.StatusCode));
			}
			// Owner can't book their own listing
			// Get OwnerID by ListingId
			var getListingDetails = await _availabilityService.GetListingDetails(listingId).ConfigureAwait(false);
			if (!getListingDetails.IsSuccessful || getListingDetails.Payload == null)
			{
				_loggerService.Log(LogLevel.ERROR, Category.DATA, getListingDetails.ErrorMessage!);
				return new(Result.Failure(getListingDetails.ErrorMessage!, getListingDetails.StatusCode));
			}

			int ownerId = (int)getListingDetails.Payload!.OwnerId!;
			if (userId == ownerId)
			{
				_loggerService.Log(LogLevel.ERROR, Category.BUSINESS, "Owner attempted to book their own listing.");
				return new(Result.Failure("Owner can't book their own listing", StatusCodes.Status400BadRequest));
			}

			// Check if timeframes are valid
			foreach (var timeframe in chosenTimeframes)
			{
				// Each pair of Start and End Time must be on the same date
				if (timeframe.StartDateTime.Date != timeframe.EndDateTime.Date)
				{
					_loggerService.Log(LogLevel.ERROR, Category.DATA, "Start and End Date are not the same.");
					return new(Result.Failure("Invalid chosen time frame. Start and End time must be on the same date", StatusCodes.Status400BadRequest));
				}
				// Check each chosen time frame against the BookedTimeFrames table
				var isOpentoBook = await _availabilityService.ValidateChosenTimeFrames(timeframe).ConfigureAwait(false);
				if (!isOpentoBook.IsSuccessful) //timeframe already booked
				{
					_loggerService.Log(LogLevel.ERROR, Category.BUSINESS, "Attempt to reserve time slots already booked");
					return new(Result.Failure(isOpentoBook.ErrorMessage!, isOpentoBook.StatusCode));
				}
			}
			// Add a new booking
			Booking booking = new()
			{
				UserId = userId,
				ListingId = listingId,
				FullPrice = fullPrice,
				BookingStatusId = BookingStatus.CONFIRMED,
				TimeFrames = chosenTimeframes,
			};

			var createBooking = await _bookingService.AddNewBooking(booking).ConfigureAwait(false);

			if (!createBooking.IsSuccessful || createBooking.Payload == 0)
			{
				// Failed to create new booking
				_loggerService.Log(LogLevel.ERROR, Category.BUSINESS, createBooking.ErrorMessage!);
				return new(Result.Failure("Scheduling error. Booking not created. Refresh page or try again later", createBooking.StatusCode));
			}
			// Process payload
			int bookingId = createBooking.Payload;

			// Notify User and Host
			var notifyUsers = await NotifyUsers(bookingId, userId, ownerId, BookingStatus.CONFIRMED).ConfigureAwait(false);
			if (!notifyUsers.IsSuccessful)
			{
				return new(Result.Failure(notifyUsers.ErrorMessage!, notifyUsers.StatusCode));
			}

			// Add to Listing History table
			var updateListingHistory = await _bookingService.AddUserToListingHistory(listingId,userId).ConfigureAwait(false);
			if(!updateListingHistory.IsSuccessful)
			{
				_loggerService.Log(LogLevel.ERROR, Category.DATA, updateListingHistory.ErrorMessage!);
				return new(Result.Failure(notifyUsers.ErrorMessage!, notifyUsers.StatusCode));
			}

			// Log successful booking
			string logString = string.Format("User #{0} booked, Booking #{2}", userId, listingId, bookingId);
			_loggerService.Log(LogLevel.INFO, Category.BUSINESS, logString);

			// Prep confirmation for Booking View
			var confirmation = getListingDetails.Payload;
			confirmation.UserId = userId;
			confirmation.BookingId = bookingId;
			confirmation.FullPrice = booking.FullPrice;
			confirmation.BookedTimeFrames = chosenTimeframes;

			return Result<BookingViewDTO>.Success(confirmation);
		}
		public async Task<Result<BookingViewDTO>> GetBookingDetails(int userId, int bookingId)
		{
			// Authorize user 
			var authzUser = AuthorizeUser(userId);
			if (!authzUser.IsSuccessful)
			{
				return new(Result.Failure(authzUser.ErrorMessage!, authzUser.StatusCode));
			}

			// Get booking details
			var getBookedTimeFrames = await _bookingService.GetBookedTimeFramesByBookingId(bookingId).ConfigureAwait(false);
			if (!getBookedTimeFrames.IsSuccessful)
			{
				_loggerService.Log(LogLevel.ERROR, Category.DATA, getBookedTimeFrames.ErrorMessage!);
				return new(Result.Failure(getBookedTimeFrames.ErrorMessage!, getBookedTimeFrames.StatusCode));
			}
			var bookingViewDTO = new BookingViewDTO();
			bookingViewDTO.UserId = userId;
			bookingViewDTO.BookingId = bookingId;
			bookingViewDTO.BookedTimeFrames = getBookedTimeFrames.Payload;

			return Result<BookingViewDTO>.Success(bookingViewDTO);
		}
	}
}