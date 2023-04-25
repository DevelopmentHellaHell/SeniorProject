namespace DevelopmentHell.Hubba.WebAPI.DTO.AccountSystem
{
    public class UpdatePasswordDTO
    {
        public string? oldPassword { get; set; }
        public string? newPassword { get; set; }
        public string? newPasswordDupe { get; set; }
    }
}
