namespace DevelopmentHell.Hubba.Models.DTO
{
    public class ListingRatingEditorDTO
    {
        public int ListingId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool? Anonymous { get; set; }
        public DateTime? LastEdited { get; set; }
    }
}