namespace DevelopmentHell.Hubba.Models
{
	public class HashData
	{
		public string Hash { get; set; }
		public string Salt { get; set; }
		public HashData(string hash, string salt)
		{
			Hash = hash;
			Salt = salt;
		}
	}
}
