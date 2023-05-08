﻿using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;

namespace DevelopmentHell.Hubba.Scheduling.Manager.Abstraction
{
    public interface ISchedulingManager
    {
        Task<Result<List<ListingAvailabilityDTO>>> FindListingAvailabiityByMonth(int listingId, int month, int year);
        Task<Result<BookingViewDTO>> ReserveBooking(int userId, int listingId, float fullPrice, List<BookedTimeFrame> chosenTimeframes, BookingStatus bookingStatus = BookingStatus.CONFIRMED);
        Task<Result<bool>> CancelBooking(int userId, int bookingId);
        Task<Result<BookingViewDTO>> GetBookingDetails(int userId, int bookingId);
    }
}
