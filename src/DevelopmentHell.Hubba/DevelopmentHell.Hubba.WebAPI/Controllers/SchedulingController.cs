using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Models.DTO;
using DevelopmentHell.Hubba.Scheduling.Manager.Abstraction;
using DevelopmentHell.Hubba.WebAPI.DTO.Scheduling;
using Microsoft.AspNetCore.Mvc;

namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SchedulingController : HubbaController
    {
        private readonly ISchedulingManager _schedulingManager;
        public SchedulingController(ISchedulingManager schedulingManager)
        {
            _schedulingManager = schedulingManager;
        }
        [HttpPost]
        [Route("findListingAvailabilityByMonth")]
        public async Task<IActionResult> FindListingAvailabilityByMonth(FindAvailabilityDTO findAvailabilityDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
                var result = await _schedulingManager.FindListingAvailabiityByMonth(
                    findAvailabilityDTO.ListingId,
                    findAvailabilityDTO.Month,
                    findAvailabilityDTO.Year).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode, result.Payload);
            }).ConfigureAwait(false);
        }
        [HttpPost]
        [Route("reserve")]
        public async Task<IActionResult> Reserve(ReserveDTO reserveDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
                List<BookedTimeFrame> chosenTimeFrames = new();
                foreach (var item in reserveDTO.ChosenTimeFrames)
                {
                    chosenTimeFrames.Add(new BookedTimeFrame()
                    {
                        ListingId = reserveDTO.ListingId,
                        AvailabilityId = item.AvailabilityId,
                        StartDateTime = DateTime.Parse(item.StartDateTime),
                        EndDateTime = DateTime.Parse(item.EndDateTime)
                    });
                }
                var result = await _schedulingManager.ReserveBooking(
                    reserveDTO.UserId,
                    reserveDTO.ListingId,
                    reserveDTO.FullPrice,
                    chosenTimeFrames).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode, result.Payload);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("cancel")]
        public async Task<IActionResult> Cancel(CancelBookingDTO cancelBookingDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _schedulingManager.CancelBooking(cancelBookingDTO.UserId, cancelBookingDTO.BookingId).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode, result.Payload);
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("getbookingdetails")]
        public async Task<IActionResult> GetBookingDetails(BookingViewDTO bookingViewDTO)
        {
            return await GuardedWorkload(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var result = await _schedulingManager.GetBookingDetails((int)bookingViewDTO.UserId!, bookingViewDTO.BookingId).ConfigureAwait(false);
                if (!result.IsSuccessful)
                {
                    return StatusCode(result.StatusCode, result.ErrorMessage);
                }

                return StatusCode(result.StatusCode, result.Payload);
            }).ConfigureAwait(false);
        }
    }
}
