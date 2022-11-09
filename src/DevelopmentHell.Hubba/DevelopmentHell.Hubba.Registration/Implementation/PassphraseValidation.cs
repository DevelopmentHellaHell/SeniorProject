using System.Text.RegularExpressions;
using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Registration.Implementation
{
    public class PassphraseValidation
    {
        //passphrase satisfies length and format
        //length: min 8 character
        //format: valid characters:
        //blank space
        //a - z
        //A - Z
        //0 - 9
        //.,@!-
        public static Result validate(string passphrase)
        {
            Result result = new Result();
            result.IsSuccessful = false;
            Regex rx = new(@"[^A-Za-z0-9.,@! -]");
            if (rx.IsMatch(passphrase) || passphrase.Length < 8)
            {
                result.ErrorMessage = "Passphrase provided is invalid. Retry or contact admin.";
                return result;
            }
            result.IsSuccessful = true;
            return result;
        }
    }
}
