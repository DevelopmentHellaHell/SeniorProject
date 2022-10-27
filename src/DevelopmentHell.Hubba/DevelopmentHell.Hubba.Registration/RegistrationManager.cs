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

        //public bool ValidateEmail()
        //{
        //    //TODO: email contains 1 '@'
        //    //var result = new Result();
        //    if (_email.Contains('@'))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        public Result ValidateEmail()
        {
            //TODO: email contains 1 '@'
            var result = new Result();
            if (_email.Count(x => x == '@') == 1)
            {
                result.IsValid = true;
            }
            return result;
        }
    }
}