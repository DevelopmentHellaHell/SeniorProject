using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            if (rx.IsMatch(passphrase))
            {
                result.ErrorMessage = "Passphrase provided is invalid. Retry or contact admin.";
                return result;
            } else if (passphrase.Length < 8)
            {
                result.ErrorMessage = "Passphrase provided is invalid. Retry or contact admin.";
                return result;
            }
            result.IsSuccessful = true;
            return result;
        }
    }
}
