namespace DevelopmentHell.Hubba.WebAPI.DTO.Discovery
{
    public class SearchDTO
    {
        public string Query { get; set; }
        public string Category { get; set; }
        public int Offset { get; set; }
        public string Filter { get; set; }
    }
}
