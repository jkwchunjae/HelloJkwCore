using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace HelloJkwCore.Authentication;

public class AppUserManager : UserManager<ApplicationUser>
{
    public AppUserManager(
        IUserStore<ApplicationUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<ApplicationUser> passwordHasher,
        //IEnumerable<IUserValidator<ApplicationUser>> userValidators,
        IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<ApplicationUser>> logger)
        : base(store,
            optionsAccessor,
            passwordHasher,
            new List<IUserValidator<ApplicationUser>>(),
            passwordValidators,
            keyNormalizer,
            errors,
            services,
            logger)
    {
    }
}
