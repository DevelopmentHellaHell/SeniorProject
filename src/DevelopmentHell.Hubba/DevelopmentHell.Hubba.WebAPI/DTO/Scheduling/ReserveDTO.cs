using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.WebAPI.DTO.Scheduling
{
    public class ReserveDTO
    {
        public int UserId { get; set; }
        public int ListingId { get; set; }
        public float FullPrice { get; set; }
        public List<BookedTimeFrameDTO> ChosenTimeFrames { get; set; }
    }
    public class BookedTimeFrameDTO
    {
        public int AvailabilityId { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
    }
}
