namespace DevelopmentHell.Hubba.Registration
{
    public class Account
    {
        public int id { get; set; }
        public string email { get; set; }
        public string passphrase { get; set; }
        //public string hashedPassphrase { get; set; } = string.Empty;
        public string username { get; set; }
        public string? displayName { get; set; } = string.Empty;
        public bool adminAccount { get; set; } = false;
        public DateTime birthDate { get; set; }
        public DateTime? lastLogIn { get; set; } 

    }
}