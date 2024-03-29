﻿using Microsoft.AspNetCore.Identity;

namespace HelloJkwCore.User;

public partial class UserStore : IUserLoginStore<AppUser>, IUserEmailStore<AppUser>
{
    private readonly Func<Paths, string> _usersPath;

    private readonly ILogger _logger;
    private readonly IFileSystem _fs;

    public UserStore(
        CoreOption coreOption,
        ILoggerFactory loggerFactory,
        IFileSystemService fsService
        )
    {
        _logger = loggerFactory.CreateLogger<UserStore>();
        _fs = fsService.GetFileSystem(coreOption.UserStoreFileSystem, coreOption.Path);

        _usersPath = path => path[UserPathType.UsersPath];
    }

    public void Dispose()
    {
    }

    private async Task<List<AppUser>> LoadUserListAsync(CancellationToken ct = default)
    {
        var files = await _fs.GetFilesAsync(_usersPath, ".json", ct);
        var users = await files.Select(async file =>
            {
                try
                {
                    return await _fs.ReadJsonAsync<AppUser>(path => path.UserFilePathByFileName(file));
                }
                catch
                {
                    return null;
                }
            })
            .WhenAll();

        return users
            .Where(user => user != null)
            .ToList();
    }

    private async Task CreateUsersDirectoryAsync(CancellationToken ct = default)
    {
        if (await _fs.DirExistsAsync(_usersPath, ct))
        {
            return;
        }

        await _fs.CreateDirectoryAsync(_usersPath, ct);
    }

    private async Task<AppUser> GetUserAsync(UserId userId, CancellationToken ct = default)
    {
        await CreateUsersDirectoryAsync(ct);

        Func<Paths, string> userPath = path => path.UserFilePathByUserId(userId);
        if (await _fs.FileExistsAsync(userPath))
        {
            return await _fs.ReadJsonAsync<AppUser>(userPath);
        }
        else if (await _fs.FileExistsAsync(path => path.DeletedUserFilePathByUserId(userId), ct))
        {
            return await _fs.ReadJsonAsync<AppUser>(path => path.DeletedUserFilePathByUserId(userId), ct);
        }
        else
        {
            return null;
        }
    }

    private async Task SaveUserAsync(AppUser user, CancellationToken ct = default)
    {
        await CreateUsersDirectoryAsync(ct);

        await _fs.WriteJsonAsync(path => path.UserFilePathByUserId(user.Id), user, ct);
        if (await _fs.FileExistsAsync(path => path.DeletedUserFilePathByUserId(user.Id), ct))
        {
            await _fs.DeleteFileAsync(path => path.DeletedUserFilePathByUserId(user.Id), ct);
        }
    }

    private async Task<bool> ExistsUserAsync(UserId userId, CancellationToken ct = default)
    {
        var aliveExists = await _fs.FileExistsAsync(path => path.UserFilePathByUserId(userId), ct);
        if (aliveExists)
        {
            return true;
        }

        var deletedExists = await _fs.FileExistsAsync(path => path.DeletedUserFilePathByUserId(userId), ct);
        return deletedExists ? true : false;
    }

    public Task AddLoginAsync(AppUser user, UserLoginInfo login, CancellationToken ct)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public async Task<IdentityResult> CreateAsync(AppUser user, CancellationToken ct)
    {
        try
        {
            var exists = await ExistsUserAsync(user.Id);

            if (exists)
            {
                return IdentityResult.Failed(new[] { new IdentityError { Code = "AlreadyExists" } });
            }

            await SaveUserAsync(user);

            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateAsync");
            return IdentityResult.Failed();
        }
    }

    public async Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken ct)
    {
        try
        {
            if (await ExistsUserAsync(user.Id))
            {
                var text = await _fs.ReadTextAsync(path => path.UserFilePathByUserId(user.Id));
                await _fs.WriteTextAsync(path => path.DeletedUserFilePathByUserId(user.Id), text);
                await _fs.DeleteFileAsync(path => path.UserFilePathByUserId(user.Id));
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteAsync");
            return IdentityResult.Failed();
        }
    }

    private async Task<AppUser> FindByAsync(Func<AppUser, bool> func, CancellationToken ct)
    {
        var userList = await LoadUserListAsync(ct);
        var user = userList.FirstOrDefault(func);
        return user;
    }

    public async Task<AppUser> FindByEmailAsync(string normalizedEmail, CancellationToken ct)
    {
        return await FindByAsync(x => x.Email == normalizedEmail, ct);
    }

    public async Task<AppUser> FindByIdAsync(string userId, CancellationToken ct)
    {
        return await GetUserAsync(new UserId(userId), ct);
    }

    public async Task<AppUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken ct)
    {
        var userId = AppUser.UserId(loginProvider, providerKey);

        return await FindByIdAsync(userId.Id, ct);
    }

    public async Task<AppUser> FindByNameAsync(string normalizedUserName, CancellationToken ct)
    {
        return await FindByAsync(x => x.UserName == normalizedUserName, ct);
    }

    public Task<string> GetEmailAsync(AppUser user, CancellationToken ct)
    {
        return Task.FromResult(user.Email);
    }

    public Task<bool> GetEmailConfirmedAsync(AppUser user, CancellationToken ct)
    {
        return Task.FromResult(true);
    }

    public Task<IList<UserLoginInfo>> GetLoginsAsync(AppUser user, CancellationToken ct)
    {
        return Task.FromResult<IList<UserLoginInfo>>(new List<UserLoginInfo>());
    }

    public Task<string> GetNormalizedEmailAsync(AppUser user, CancellationToken ct)
    {
        return Task.FromResult(user.Email);
    }

    public Task<string> GetNormalizedUserNameAsync(AppUser user, CancellationToken ct)
    {
        return Task.FromResult(user.UserName);
    }

    public Task<string> GetUserIdAsync(AppUser user, CancellationToken ct)
    {
        return Task.FromResult(user.Id.Id);
    }

    public Task<string> GetUserNameAsync(AppUser user, CancellationToken ct)
    {
        return Task.FromResult(user.UserName);
    }

    public Task RemoveLoginAsync(AppUser user, string loginProvider, string providerKey, CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public Task SetEmailAsync(AppUser user, string email, CancellationToken ct)
    {
        user.Email = email;
        return Task.CompletedTask;
    }

    public Task SetEmailConfirmedAsync(AppUser user, bool confirmed, CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public Task SetNormalizedEmailAsync(AppUser user, string normalizedEmail, CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public Task SetNormalizedUserNameAsync(AppUser user, string normalizedName, CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public Task SetUserNameAsync(AppUser user, string userName, CancellationToken ct)
    {
        user.UserName = userName;
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken ct)
    {
        try
        {
            if (await ExistsUserAsync(user.Id, ct))
            {
                await SaveUserAsync(user, ct);
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateAsync");
            return IdentityResult.Failed();
        }
    }
}