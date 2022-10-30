namespace DevelopmentHell.Hubba.Registration
{
    public class Account
    {
        public String Email { get; set; }
        public String Passphrase { get; set; }
        public String Username { get; set; }

        public Account() { }
        public Account(String email, String passphrase)
        {
            this.Email = email;
            this.Passphrase = passphrase;
        }
    }
}