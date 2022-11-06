namespace DevelopmentHell.Hubba.Registration
{
    public class Account
    {
        public int id { get; set; }
        public string email { get; set; } = string.Empty;
        public string passphrase { get; set; } = string.Empty;
        public string hashedPassphrase { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string displayName { get; set; } = string.Empty;
        public bool adminAccount { get; set; }
    }
}