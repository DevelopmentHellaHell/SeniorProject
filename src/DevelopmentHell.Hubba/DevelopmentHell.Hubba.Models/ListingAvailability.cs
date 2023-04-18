
namespace DevelopmentHell.Hubba.Models
{
    public class ListingAvailability
    {
        public int? ListingId { get; set; }
        public int? AvailabilityId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
    // { [3: 3, 4], [4: 6, 7], [7: 10, 11] }
    // { [null: 1, 2], [4: 6, 8], [7: -1, -1] }
    // { [add], [update], [delete] } 
}
