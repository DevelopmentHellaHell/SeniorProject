namespace DevelopmentHell.Hubba.WebAPI.DTO.ListingProfile
{
    public class FilesToEditDTO
    {
        public int ListingId { get; set; }
        public List<string>? DeleteNames { get; set; }
        public List<Tuple<string, string>>? AddFiles { get; set; }
    }
}
