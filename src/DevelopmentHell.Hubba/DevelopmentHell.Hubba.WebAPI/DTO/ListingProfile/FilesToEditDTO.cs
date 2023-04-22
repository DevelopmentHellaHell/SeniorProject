namespace DevelopmentHell.Hubba.WebAPI.DTO.ListingProfile
{
    public class FilesToEditDTO
    {
        public int ListingId { get; set; }
        public List<string>? DeleteNames { get; set; }
        //public Dictionary<string, IFormFile>? AddFiles { get; set; }
        public Dictionary<string, byte[]>? AddFiles { get; set; }
    }
}
