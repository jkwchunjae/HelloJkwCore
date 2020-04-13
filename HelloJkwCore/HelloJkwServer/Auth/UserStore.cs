using Common;
using HelloJkwServer.Misc;
using HelloJkwServer.Models;
using JkwExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelloJkwServer.Auth
{
    public class UserStore : IUserLoginStore<AppUser>, IUserEmailStore<AppUser>
    {
        private string _userListPath;

        private readonly ILogger _logger;

        public UserStore(
            IConfiguration configuration,
            ILoggerFactory loggerFactory
            )
        {
            _logger = loggerFactory.CreateLogger<UserStore>();
            _userListPath = configuration.GetPath(PathOf.UserListFile);
        }

        public void Dispose()
        {
        }

        private async Task<List<AppUser>> LoadUserListAsync(CancellationToken cancellationToken)
        {
            if (!File.Exists(_userListPath))
            {
                return new List<AppUser>();
            }
            var text = await File.ReadAllTextAsync(_userListPath, cancellationToken);
            return JsonConvert.DeserializeObject<List<AppUser>>(text);
        }

        private async Task SaveUserList(List<AppUser> userList, CancellationToken cancellationToken)
        {
            var jsonText = JsonConvert.SerializeObject(userList, Formatting.Indented);
            await File.WriteAllTextAsync(_userListPath, jsonText, new UTF8Encoding(), cancellationToken);
        }

        public Task AddLoginAsync(AppUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public async Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken)
        {
            try
            {
                var userList = await LoadUserListAsync(cancellationToken);
                if (userList.Empty(x => x.Id == user.Id))
                {
                    userList.Add(user);
                    await SaveUserList(userList, cancellationToken);
                }

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync");
                return IdentityResult.Failed();
            }
        }

        public async Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken)
        {
            try
            {
                var userList = await LoadUserListAsync(cancellationToken);
                if (userList.Any(x => x.Id == user.Id))
                {
                    userList = userList
                        .Where(x => x.Id != user.Id)
                        .ToList();
                    await SaveUserList(userList, cancellationToken);
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

        private async Task<AppUser> FindByAsync(Func<AppUser, bool> func, CancellationToken cancellationToken)
        {
            var userList = await LoadUserListAsync(cancellationToken);
            var user = userList.FirstOrDefault(func);
            return user;
        }

        public async Task<AppUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return await FindByAsync(x => x.Email == normalizedEmail, cancellationToken);
        }

        public async Task<AppUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await FindByAsync(x => x.Id == userId, cancellationToken);
        }

        public async Task<AppUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var userId = AppUser.UserId(loginProvider, providerKey);

            return await FindByIdAsync(userId, cancellationToken);
        }

        public async Task<AppUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await FindByAsync(x => x.UserName == normalizedUserName, cancellationToken);
        }

        public Task<string> GetEmailAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult<IList<UserLoginInfo>>(new List<UserLoginInfo>());
        }

        public Task<string> GetNormalizedEmailAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<string> GetNormalizedUserNameAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<string> GetUserIdAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task RemoveLoginAsync(AppUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetEmailAsync(AppUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(AppUser user, bool confirmed, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(AppUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(AppUser user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(AppUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken)
        {
            try
            {
                var userList = await LoadUserListAsync(cancellationToken);
                var userIndex = userList.FindIndex(x => x.Id == user.Id);
                if (userIndex == -1)
                {
                    return IdentityResult.Failed();
                }
                else
                {
                    userList[userIndex] = user;
                    await SaveUserList(userList, cancellationToken);

                    return IdentityResult.Success;
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
