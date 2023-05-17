namespace DevelopmentHell.Hubba.WebAPI.Controllers
{
    public class GetReservationsDTO
    {
        public string? sort { get; set; }
        public int reservationCount { get; set; }
        public int page { get; set; }
    }
}
