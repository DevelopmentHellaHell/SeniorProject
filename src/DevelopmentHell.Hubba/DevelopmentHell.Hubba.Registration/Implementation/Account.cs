namespace DevelopmentHell.Hubba.Registration
{
    public class Account
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PassphraseHash { get; set; } = string.Empty;
        public string PassphraseSalt { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? DisplayName { get; set; } = string.Empty;
        public bool AdminAccount { get; set; } = false;
        public DateTime BirthDate { get; set; }
        public DateTime? LastLogIn { get; set; }
    }
}