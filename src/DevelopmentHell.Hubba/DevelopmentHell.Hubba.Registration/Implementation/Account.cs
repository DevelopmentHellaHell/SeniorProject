namespace DevelopmentHell.Hubba.Registration
{
    public class Account
    {
        public string Email { get; set; } = string.Empty;
        public string HashedPassphrase { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;

        public Account() { }
        public Account(string email, string hashedpassphrase)
        {
            this.Email = email;
            this.HashedPassphrase = hashedpassphrase;
        }
    }
}