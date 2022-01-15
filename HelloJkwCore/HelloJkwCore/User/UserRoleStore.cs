﻿using Microsoft.AspNetCore.Identity;

namespace HelloJkwCore.User;

public partial class UserStore : IUserRoleStore<AppUser>
{
    public async Task AddToRoleAsync(AppUser user, string roleName, CancellationToken ct)
    {
        var role = ParseRole(roleName);
        if (user.Roles?.Contains(role) ?? false)
            return;

        if (user.Roles == null)
            user.Roles = new();

        user.Roles.Add(role);
        await UpdateAsync(user, ct);
    }

    public async Task RemoveFromRoleAsync(AppUser user, string roleName, CancellationToken ct)
    {
        var role = ParseRole(roleName);
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
        var role = ParseRole(roleName);
        return Task.FromResult(user.Roles?.Contains(role) ?? false);
    }

    public async Task<IList<AppUser>> GetUsersInRoleAsync(string roleName, CancellationToken ct)
    {
        var userList = await LoadUserListAsync(ct);

        if (roleName.ToLower() == "all")
            return userList;

        var role = ParseRole(roleName);
        return userList.Where(x => x.Roles?.Contains(role) ?? false).ToList();
    }

    private UserRole ParseRole(string roleName)
    {
        var roles = typeof(UserRole).GetValues<UserRole>()
            .Select(role => new { RoleName = role.ToString().ToLower(), Role = role })
            .ToDictionary(x => x.RoleName, x => x.Role);

        return roles[roleName.ToLower()];
    }
}