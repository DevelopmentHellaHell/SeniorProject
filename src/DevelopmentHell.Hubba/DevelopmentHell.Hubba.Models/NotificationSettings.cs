namespace DevelopmentHell.Hubba.Models
{
	public class NotificationSettings
	{
		public int UserId { get; set; }
		public bool? SiteNotifications { get; set; }
		public bool? EmailNotifications { get; set; }
		public bool? TextNotifications { get; set; }
		public bool? TypeScheduling { get; set; }
		public bool? TypeWorkspace { get; set; }
		public bool? TypeProjectShowcase { get; set; }
		public bool? TypeOther { get; set; }
	}
}
