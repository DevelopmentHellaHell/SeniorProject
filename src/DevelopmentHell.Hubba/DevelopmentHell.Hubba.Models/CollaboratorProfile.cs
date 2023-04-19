namespace DevelopmentHell.Hubba.Models
{
    public class CollaboratorProfile
    {
        public string? Name { get; set; }
        public string? PfpUrl { get; set; }
        public string? ContactInfo { get; set; }
        public string? Tags { get; set; }
        public string? Description { get; set; }
        public string? Availability { get; set; }
        public int Votes { get; set; }
        public List<string>? CollabUrls { get; set; }
        public bool Published { get; set; }
    }
}
