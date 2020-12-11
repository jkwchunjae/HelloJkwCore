using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace HelloJkwCore.User
{
    public class AppUserManager<TUser> : UserManager<TUser>
    where TUser : class
    {
        public AppUserManager(IUserStore<TUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<TUser> passwordHasher,
            IEnumerable<IPasswordValidator<TUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<AppUserManager<TUser>> logger)
            : base(store,
                  optionsAccessor,
                  passwordHasher,
                  new List<IUserValidator<TUser>>(),
                  passwordValidators,
                  keyNormalizer,
                  errors,
                  services,
                  logger)
        {
        }
    }
}
