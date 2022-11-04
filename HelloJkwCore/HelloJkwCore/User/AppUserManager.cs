using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace HelloJkwCore.User;

public class AppUserManager<TUser> : UserManager<TUser> where TUser : class
{
    private readonly IFileSystem _fs;

    public AppUserManager(
        CoreOption coreOption,
        IFileSystemService fsService,
        IUserStore<TUser> store,
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
        _fs = fsService.GetFileSystem(coreOption.UserStoreFileSystem, coreOption.Path);
    }
}