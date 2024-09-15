using JkwExtensions;
using Microsoft.AspNetCore.Identity;

namespace HelloJkwCore.Authentication;

public class AppUserStore : IUserLoginStore<AppUser>, IUserRoleStore<AppUser>
{
    private readonly ILogger _logger;
    private readonly IFileSystem _fs;
    private readonly Dictionary<string, UserRole> _cachedRoles;

    public AppUserStore(
        CoreOption coreOption,
        ILoggerFactory loggerFactory,
        IFileSystemService fsService)
    {
        if (coreOption.UserStoreFileSystem == null)
            throw new ArgumentNullException(nameof(coreOption.UserStoreFileSystem));
        if (coreOption.Path == null)
            throw new ArgumentNullException(nameof(coreOption.Path));

        _logger = loggerFactory.CreateLogger<AppUserStore>();
        _fs = fsService.GetFileSystem(coreOption.UserStoreFileSystem, coreOption.Path);
        _cachedRoles = typeof(UserRole).GetValues<UserRole>()
            .Select(role => new { RoleName = role.ToString().ToLower(), Role = role })
            .ToDictionary(x => x.RoleName, x => x.Role);
    }

    public void Dispose()
    {
    }

    private async Task<List<AppUser>> LoadUserListAsync(CancellationToken ct = default)
    {
        var files = await _fs.GetFilesAsync(path => path["Users"], ".json", ct);
        var users = await files.Select(async file =>
            {
                try
                {
                    return await _fs.ReadJsonAsync<AppUser>(path => Path.Join(path["Users"], file));
                }
                catch
                {
                    return null;
                }
            })
            .WhenAll();

        return users
            .Where(user => user != null)
            .Select(user => user!)
            .ToList();
    }
    public async Task AddLoginAsync(AppUser user, UserLoginInfo login, CancellationToken cancellationToken)
    {
        var externalId = $"{login.LoginProvider}.{login.ProviderKey}";
        var loginFileName = $"{externalId}.json";
        var loginFilePath = (Paths path) => Path.Join(path["Logins"], loginFileName);

        var loginInfo = new AppLoginInfo
        {
            Provider = login.LoginProvider,
            ProviderKey = login.ProviderKey,
            ProviderDisplayName = login.ProviderDisplayName,
            CreateTime = DateTime.Now,
            LastLoginTime = DateTime.Now,
            ConnectedUserId = user.Id
        };
        await _fs.WriteJsonAsync<AppLoginInfo>(loginFilePath, loginInfo, cancellationToken);

        user.Logins.Add(loginInfo);
    }

    public async Task<AppUser?> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        var loginFileName = $"{loginProvider}.{providerKey}.json";
        var loginFilePath = (Paths path) => Path.Join(path["Logins"], loginFileName);
        if (await _fs.FileExistsAsync(loginFilePath, cancellationToken))
        {
            var loginInfo = await _fs.ReadJsonAsync<AppLoginInfo>(loginFilePath);
            if (loginInfo.ConnectedUserId?.Id != null)
            {
                var appUser = await FindByIdAsync(loginInfo.ConnectedUserId.Id, cancellationToken);
                return appUser;
            }
        }
        return default;
    }

    public Task<IList<UserLoginInfo>> GetLoginsAsync(AppUser user, CancellationToken cancellationToken)
    {
        var logins = user.Logins.Select(x => x.LoginInfo).ToList();
        return Task.FromResult<IList<UserLoginInfo>>(logins);
    }

    public async Task RemoveLoginAsync(AppUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        var loginFileName = $"{loginProvider}.{providerKey}.json";
        var loginFilePath = (Paths path) => Path.Join(path["Logins"], loginFileName);
        if (await _fs.FileExistsAsync(loginFilePath, cancellationToken))
        {
            var loginInfo = await _fs.ReadJsonAsync<AppLoginInfo>(loginFilePath, cancellationToken);
            loginInfo.ConnectedUserId = null;
            await _fs.WriteJsonAsync<AppLoginInfo>(loginFilePath, loginInfo, cancellationToken);
        }

        user.Logins.RemoveAll(x => x.Provider == loginProvider && x.ProviderKey == providerKey);
    }

    public async Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken)
    {
        var userFileName = $"{user.Id}.json";
        var userFilePath = (Paths path) => Path.Join(path["Users"], userFileName);
        await _fs.WriteJsonAsync<AppUser>(userFilePath, user, cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken)
    {
        await user.Logins
            .Select(async loginInfo =>
            {
                var loginFileName = $"{loginInfo.Provider}.{loginInfo.ProviderKey}.json";
                var loginFilePath = (Paths path) => Path.Join(path["Logins"], loginFileName);
                if (await _fs.FileExistsAsync(loginFilePath))
                {
                    await _fs.DeleteFileAsync(loginFilePath);
                }
            })
            .WhenAll();

        var userFileName = $"{user.Id}.json";
        var userFilePath = (Paths path) => Path.Join(path["Users"], userFileName);
        if (await _fs.FileExistsAsync(userFilePath))
        {
            await _fs.DeleteFileAsync(userFilePath);
        }

        return IdentityResult.Success;
    }

    public async Task<AppUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var userFileName = $"{userId}.json";
        var userFilePath = (Paths path) => Path.Join(path["Users"], userFileName);
        if (await _fs.FileExistsAsync(userFilePath))
        {
            var appUser = await _fs.ReadJsonAsync<AppUser>(userFilePath, cancellationToken);
            return appUser;
        }
        return default;
    }

    public Task<AppUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetNormalizedUserNameAsync(AppUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetUserIdAsync(AppUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id.Id);
    }

    public Task<string?> GetUserNameAsync(AppUser user, CancellationToken cancellationToken)
    {
        var userName = user.UserName;
        return Task.FromResult(userName);
    }

    public Task SetNormalizedUserNameAsync(AppUser user, string? normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    public Task SetUserNameAsync(AppUser user, string? userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken)
    {
        var userFileName = $"{user.Id}.json";
        var userFilePath = (Paths path) => Path.Join(path["Users"], userFileName);
        await _fs.WriteJsonAsync<AppUser>(userFilePath, user, cancellationToken);
        return IdentityResult.Success;
    }

    private UserRole ParseRole(string roleName)
    {
        return _cachedRoles[roleName.ToLower()];
    }
    public async Task AddToRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
    {
        var role = ParseRole(roleName);

        if (user.Roles.Contains(role))
            return;

        user.Roles.Add(role);
        await UpdateAsync(user, cancellationToken);
    }

    public async Task RemoveFromRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
    {
        var role = ParseRole(roleName);

        if (user.Roles.Contains(role))
        {
            user.Roles.RemoveAll(x => x == role);
            await UpdateAsync(user, cancellationToken);
        }
    }

    public Task<IList<string>> GetRolesAsync(AppUser user, CancellationToken cancellationToken)
    {
        var roles = user.Roles.Select(x => x.ToString()).ToList();
        return Task.FromResult<IList<string>>(roles);
    }

    public Task<bool> IsInRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"IsInRoleAsync: {user.UserName}, {roleName}");
        var role = ParseRole(roleName);
        return Task.FromResult(user.HasRole(role));
    }

    public async Task<IList<AppUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        var users = await LoadUserListAsync(cancellationToken);

        if (roleName.ToLower() == "all")
            return users;

        var role = ParseRole(roleName);
        return users.Where(user => user.HasRole(role)).ToList();
    }
}