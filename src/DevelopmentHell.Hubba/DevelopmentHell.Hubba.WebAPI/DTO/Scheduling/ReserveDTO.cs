using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.WebAPI.DTO.Scheduling
{
    public class ReserveDTO
    {
        public int UserId { get; set; }
        public int ListingId { get; set; }
        public float FullPrice { get; set; }
        public List<BookedTimeFrame>? ChosenTimeFrames { get; set; }
    }
}
