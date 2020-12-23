using Common;
using Common.Extensions;
using Common.FileSystem;
using Common.User;
using JkwExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HelloJkwCore.User
{
    public class UserStore : IUserLoginStore<AppUser>, IUserEmailStore<AppUser>
    {
        private readonly string _usersPath;

        private readonly ILogger _logger;
        private readonly IFileSystem _fs;

        public UserStore(
            ILoggerFactory loggerFactory,
            IFileSystem fileSystem
            )
        {
            _logger = loggerFactory.CreateLogger<UserStore>();
            _usersPath = PathType.UsersPath.GetPath();
            _fs = fileSystem;
        }

        public void Dispose()
        {
        }

        private string UserFilePath(string userId) => $"{_usersPath}/user.{userId}.json";

        private async Task<List<AppUser>> LoadUserListAsync(CancellationToken ct = default)
        {
            var files = await _fs.GetFilesAsync(_usersPath, ".json", ct);
            var users = await files.Select(async file =>
                {
                    try
                    {
                        return await _fs.ReadJsonAsync<AppUser>($"{_usersPath}/{file}");
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

        private async Task<AppUser> GetUserAsync(string userId, CancellationToken ct = default)
        {
            await CreateUsersDirectoryAsync(ct);

            var userPath = UserFilePath(userId);
            if (await _fs.FileExistsAsync(userPath))
            {
                return await _fs.ReadJsonAsync<AppUser>(userPath);
            }
            else
            {
                return null;
            }
        }

        private async Task SaveUserAsync(AppUser user, CancellationToken ct = default)
        {
            await CreateUsersDirectoryAsync(ct);

            await _fs.WriteJsonAsync(UserFilePath(user.Id), user, ct);
        }

        private async Task<bool> ExistsUserAsync(string userId, CancellationToken ct = default)
        {
            return await _fs.FileExistsAsync(UserFilePath(userId), ct);
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
                    await _fs.DeleteFileAsync(UserFilePath(user.Id));
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
            return await GetUserAsync(userId, ct);
        }

        public async Task<AppUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken ct)
        {
            var userId = AppUser.UserId(loginProvider, providerKey);

            return await FindByIdAsync(userId, ct);
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
            return Task.FromResult(user.Id);
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
}
