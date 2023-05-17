namespace DevelopmentHell.Hubba.Models
{
    public class Reservations
    {
        public int OwnerId { get; set; }
        public int UserId { get; set; }
        public int ListingId { get; set; }
        public string? Title { get; set; }

    }
}
