namespace DevelopmentHell.Hubba.WebAPI.DTO.Notification
{
    public class UpdateNotificationSettingsDTO
    {
        public bool? SiteNotifications { get; set; }
        public bool? EmailNotifications { get; set; }
        public bool? TextNotifications { get; set; }
        public bool? TypeScheduling { get; set; }
        public bool? TypeWorkspace { get; set; }
        public bool? TypeProjectShowcase { get; set; }
        public bool? TypeOther { get; set; }
    }
}
