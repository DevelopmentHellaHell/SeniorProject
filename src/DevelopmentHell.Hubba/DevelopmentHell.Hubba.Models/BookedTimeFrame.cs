namespace DevelopmentHell.Hubba.Models
{
    public class BookedTimeFrame
    {
        public int BookingId { get; set; }
        public int ListingId { get; set; }
        public int AvailabilityId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }
}
