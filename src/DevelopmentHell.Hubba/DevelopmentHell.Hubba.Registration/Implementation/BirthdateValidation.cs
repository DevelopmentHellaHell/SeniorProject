
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Registration.Implementation
{
    public class BirthdateValidation
    {
        public static Result validate(DateTime birthdate)
        {
            Result result = new Result();
            result.IsSuccessful = false;
            DateTime today = DateTime.Now;
            if (today.Subtract(birthdate).Days < (365 * 14))
            {
                result.ErrorMessage = "Age requirement not reached.";
                return result;
            }
            result.IsSuccessful = true;

            return result;
        }
    }
}
