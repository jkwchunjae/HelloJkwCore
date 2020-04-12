﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwServer.Models
{
    public class AppUser
    {
        public static string UserId(string loginProvider, string providerKey)
            => $"{loginProvider}.{providerKey}";

        public string Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public AppUser() { }

        public AppUser(string loginProvider, string providerKey)
        {
            Id = UserId(loginProvider, providerKey);
        }
    }
}
