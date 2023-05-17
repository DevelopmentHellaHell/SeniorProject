namespace DevelopmentHell.Hubba.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int ListingId { get; set; }
        public double FullPrice { get; set; }
        public BookingStatus? BookingStatusId { get; set; }
        public DateTime CreationDate { get; set; }
        public int LastEditUser { get; set; }

        public IEnumerable<BookedTimeFrame>? TimeFrames { get; set; }
    }
}
