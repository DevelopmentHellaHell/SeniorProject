namespace DevelopmentHell.Hubba.WebAPI.DTO.Authentication
{
    public class UserToAuthenticateOtpDTO
    {
        public int AccountId { get; set; }
        public string Otp { get; set; }
        public string? IpAddress { get; set; }

    }
}
