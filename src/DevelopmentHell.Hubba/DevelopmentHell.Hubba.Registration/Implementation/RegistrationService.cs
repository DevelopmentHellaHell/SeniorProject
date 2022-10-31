namespace DevelopmentHell.Hubba.Registration
{
    public class RegistrationService
    {
        public Result CheckUnusedEmail(string email)
        {
            //TODO: call Data Access to look up this email in database
            var result = new Result();

            return result;
        }

        public Result CreateAccount(string email, string hashedpassphrase)
        {
            var result = new Result();
            if (email == "" && hashedpassphrase == "")
            {
                //do something
                return result;
            }
            if (CheckUnusedEmail(email).IsValid == false)
            {
                //do something
                return result;
            }
            var myAccount = new Account(email, hashedpassphrase);
            //TODO: call Data Access to INSERT to database

            return result;
        }
        public Result HashPassphrase(string passphrase)
        {
            //TODO: use Crytography library
            var result = new Result();
            return result;
        }
    }
}