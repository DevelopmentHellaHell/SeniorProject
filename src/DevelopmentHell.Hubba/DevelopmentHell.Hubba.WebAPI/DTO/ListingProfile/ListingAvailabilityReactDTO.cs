namespace DevelopmentHell.Hubba.WebAPI.DTO.ListingProfile
{
    public class ListingAvailabilityReactDTO
    {
        public int ListingId { get; set; }
        public int? OwnerId { get; set; }
        public int? AvailabilityId { get; set; }
        public string? Date { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public AvailabilityAction? Action { get; set; }
    }
}
