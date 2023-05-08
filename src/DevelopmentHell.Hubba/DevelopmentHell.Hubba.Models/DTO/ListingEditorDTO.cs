namespace DevelopmentHell.Hubba.Models.DTO
{
    public class ListingEditorDTO
    {
        public int OwnerId { get; set; }
        public string? Title { get; set; }
        public int ListingId { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public double? Price { get; set; }
        public DateTime? LastEdited { get; set; }
        public bool Published { get; set; }
    }
}