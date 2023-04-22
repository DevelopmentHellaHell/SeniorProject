namespace DevelopmentHell.Hubba.WebAPI.DTO.ListingProfile
{
    public class RatingToAddDTO
    {
        public int ListingId { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public bool Anonymous { get; set; }
    }
}
