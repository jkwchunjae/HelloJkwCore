using JkwExtensions;
using Microsoft.AspNetCore.Identity;

namespace HelloJkwCore2.Authentication;

public class AppUserStore : IUserLoginStore<ApplicationUser>
{
    private readonly ILogger _logger;
    private readonly IFileSystem _fs;

    public AppUserStore(
        CoreOption coreOption,
        ILoggerFactory loggerFactory,
        IFileSystemService fsService)
    {
        _logger = loggerFactory.CreateLogger<AppUserStore>();
        _fs = fsService.GetFileSystem(coreOption.UserStoreFileSystem, coreOption.Path);
    }

    public void Dispose()
    {
    }
    public async Task AddLoginAsync(ApplicationUser user, UserLoginInfo login, CancellationToken cancellationToken)
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

    public async Task<ApplicationUser?> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
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

    public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var logins = user.Logins.Select(x => x.LoginInfo).ToList();
        return Task.FromResult<IList<UserLoginInfo>>(logins);
    }

    public async Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
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

    public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var userFileName = $"{user.Id}.json";
        var userFilePath = (Paths path) => Path.Join(path["Users"], userFileName);
        await _fs.WriteJsonAsync<ApplicationUser>(userFilePath, user, cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
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

    public async Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var userFileName = $"{userId}.json";
        var userFilePath = (Paths path) => Path.Join(path["Users"], userFileName);
        if (await _fs.FileExistsAsync(userFilePath))
        {
            var appUser = await _fs.ReadJsonAsync<ApplicationUser>(userFilePath, cancellationToken);
            return appUser;
        }
        return default;
    }

    public Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id.Id);
    }

    public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var userName = user.UserName;
        return Task.FromResult(userName);
    }

    public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var userFileName = $"{user.Id}.json";
        var userFilePath = (Paths path) => Path.Join(path["Users"], userFileName);
        await _fs.WriteJsonAsync<ApplicationUser>(userFilePath, user, cancellationToken);
        return IdentityResult.Success;
    }
}