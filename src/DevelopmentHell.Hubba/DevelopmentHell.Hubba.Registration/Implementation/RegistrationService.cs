namespace DevelopmentHell.Hubba.Registration
{
    public class RegistrationService
    {
        public bool CheckUnusedEmail(String email)
        {
            //TODO: call Data Access to look up this email in database
            return true;
        }

        public String CreateAccount(String email, String passphrase)
        {
            if (email == "" && passphrase == "")
            {
                return "Please enter a valid email and passphrase to register";
            }
            if (CheckUnusedEmail(email) == false)
            {
                return "Email provided already exists";
            }
            var myAccount = new Account(email, passphrase);
            //TODO: call Data Access to INSERT to database

            return "Account created successfully";
        }
    }
}