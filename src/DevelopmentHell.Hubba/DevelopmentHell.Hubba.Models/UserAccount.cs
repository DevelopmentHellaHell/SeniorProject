namespace DevelopmentHell.Hubba.Models
{
	public class UserAccount
	{
		public int? Id { get; set; }
		public string? Email { get; set; }
		public string? PasswordHash { get; set; }
		public string? PasswordSalt { get; set; }
	}
}