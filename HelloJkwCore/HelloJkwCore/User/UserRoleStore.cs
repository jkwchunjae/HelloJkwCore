using Common;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HelloJkwCore.User
{
    public partial class UserStore : IUserLoginStore<AppUser>, IUserEmailStore<AppUser>, IUserRoleStore<AppUser>
    {
        public async Task AddToRoleAsync(AppUser user, string roleName, CancellationToken ct)
        {
            var role = Enum.Parse<UserRole>(roleName);
            if (user.Roles?.Contains(role) ?? false)
                return;

            if (user.Roles == null)
                user.Roles = new();

            user.Roles.Add(role);
            await UpdateAsync(user, ct);
        }

        public async Task RemoveFromRoleAsync(AppUser user, string roleName, CancellationToken ct)
        {
            var role = Enum.Parse<UserRole>(roleName);
            if (!user.Roles?.Contains(role) ?? true)
                return;

            user.Roles?.RemoveAll(x => x == role);
            await UpdateAsync(user, ct);
        }

        public Task<IList<string>> GetRolesAsync(AppUser user, CancellationToken ct)
        {
            var roles = user.Roles?.Select(x => x.ToString())?.ToList() ?? new List<string>();
            return Task.FromResult<IList<string>>(roles);
        }

        public Task<bool> IsInRoleAsync(AppUser user, string roleName, CancellationToken ct)
        {
            var role = Enum.Parse<UserRole>(roleName);
            return Task.FromResult(user.Roles?.Contains(role) ?? false);
        }

        public async Task<IList<AppUser>> GetUsersInRoleAsync(string roleName, CancellationToken ct)
        {
            var role = Enum.Parse<UserRole>(roleName);
            var userList = await LoadUserListAsync(ct);

            return userList.Where(x => x.Roles?.Contains(role) ?? false).ToList();
        }
    }
}
