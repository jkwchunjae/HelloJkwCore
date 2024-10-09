using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Common;

public class AppUserManager : UserManager<AppUser>
{
    public AppUserManager(
        IUserStore<AppUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<AppUser> passwordHasher,
        //IEnumerable<IUserValidator<AppUser>> userValidators,
        IEnumerable<IPasswordValidator<AppUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<AppUser>> logger)
        : base(store,
            optionsAccessor,
            passwordHasher,
            new List<IUserValidator<AppUser>>(),
            passwordValidators,
            keyNormalizer,
            errors,
            services,
            logger)
    {
    }
}
