namespace DevelopmentHell.Hubba.Registration
{
    public class Result
    {
        public bool IsValid { get; set; }
        //public string ErrorMessage { get; set; }
        //public object Payload { get; set; }
    }

    public class RegistrationManager
    {
        private String _email;
        private String _passphrase;
        public RegistrationManager()
        {
            this._email = "N/A";
            this._passphrase = "N/A";
        }

        public RegistrationManager(String email, String passphrase)
        {
            this._email = email;
            this._passphrase = passphrase;
        }

        public Result ValidateEmail()
        {
            //TODO: email contains only 1 '@', at least '.', a-z, 0-9, no other special char
            var result = new Result();
            if (_email.Count(x => x == '@') != 1) //using LINQ
            {
                result.IsValid = false;
            }
            result.IsValid = true;
            return result;
        }

        public Result ValidatePassphrase()
        {
            var result = new Result();
            return result;
        }
    }
}