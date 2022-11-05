using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Registration.Implementation
{
    public class AccountDictionary
    {

        readonly Account _account;

        public AccountDictionary(Account account)
        {
            _account = account;
        }

        public Dictionary<String, Object> CreateDictionary()
        {

            var accountDictionary = JsonSerializer.Serialize(_account);
            return JsonSerializer.Deserialize<Dictionary<string, object>>(accountDictionary);

        }

    }
}
