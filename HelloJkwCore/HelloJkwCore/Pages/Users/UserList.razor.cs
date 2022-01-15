using Common;
using HelloJkwCore.Shared;
using JkwExtensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.Users;

public partial class UserList : JkwPageBase
{
    [Inject]
    private UserManager<AppUser> UserManager { get; set; }

    private List<AppUser> Users { get; set; }

    private IEnumerable<AppUser> FilteredUsers => Users
        ?.Where(x => CheckedRoles.Empty() ? true :
            CheckedRoles.All(e => x.Roles.Contains(e)));

    private IEnumerable<UserRole> CheckedRolesData { get; set; }

    private IEnumerable<UserRole> CheckedRoles = new List<UserRole>();

    public UserList()
    {
        CheckedRolesData = typeof(UserRole).GetValues<UserRole>();
    }

    protected override async Task OnPageInitializedAsync()
    {
        if (!User?.HasRole(UserRole.Admin) ?? true)
        {
            Navi.NavigateTo("/login");
            return;
        }
        Users = (await UserManager.GetUsersInRoleAsync("all")).ToList();
    }

    private async Task UserRoleChangedAsync(AppUser user, UserRole role, bool check)
    {
        if (check)
        {
            await UserManager.AddToRoleAsync(user, role.ToString());
        }
        else
        {
            await UserManager.RemoveFromRoleAsync(user, role.ToString());
        }
    }

    private async Task UserRoleToggleAsync(AppUser user, UserRole role)
    {
        var check = !user.Roles.Contains(role);
        await UserRoleChangedAsync(user, role, check);
    }
}