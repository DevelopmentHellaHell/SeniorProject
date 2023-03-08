﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.UserManagement.Service.Abstractions
{
    public interface IUserManagementService
    {
        Task<Result> SetNames(string email, string firstName, string lastName);
        Task<Result> UpdateAccount(string email, Dictionary<string, object> data);
        Task<Result> DisableAccount(int accountId);
        Task<Result> EnableAccount(int accountId);

    }
}
