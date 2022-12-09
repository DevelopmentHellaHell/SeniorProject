namespace DevelopmentHell.Hubba.Models
{
	public class UserAccount
	{
		int Id { get; set; }
		string Email { get; set; }
		string PasswordHash { get; set; }
		string PasswordSalt { get; set; }
	}
}