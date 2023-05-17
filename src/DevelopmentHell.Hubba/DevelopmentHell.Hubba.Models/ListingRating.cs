namespace DevelopmentHell.Hubba.Models
{
    public class ListingRating
    {
        public int ListingId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool? Anonymous { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastEdited { get; set; }
    }
}
