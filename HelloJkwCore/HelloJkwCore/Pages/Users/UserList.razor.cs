using Microsoft.AspNetCore.Identity;
using MudBlazor;

namespace HelloJkwCore.Pages.Users;

public partial class UserList : JkwPageBase
{
    [Inject] UserManager<AppUser> UserManager { get; set; }
    [Inject] IDialogService DialogService { get; set; }

    private List<AppUser> Users { get; set; }

    private IEnumerable<AppUser> FilteredUsers => Users
        ?.Where(user => CheckedRoles.Empty() ? true :
            CheckedRoles.All(role => user.Roles?.Contains(role) ?? false));

    private HashSet<UserRole> CheckedRoles = new();

    protected override async Task OnPageInitializedAsync()
    {
        if (!User?.HasRole(UserRole.Admin) ?? true)
        {
            Navi.NavigateTo("/login");
            return;
        }
        Users = (await UserManager.GetUsersInRoleAsync("all"))
            .OrderByDescending(x => x.LastLoginTime)
            .ToList();
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

    private async Task DeleteUserAsync(AppUser user)
    {
        await UserManager.DeleteAsync(user);

        Users.Remove(user);
        StateHasChanged();
    }

    private async Task ManageUserRole(AppUser user)
    {
        var param = new DialogParameters
        {
            ["User"] = user,
        };
        var dialog = DialogService.Show<UserRoleDialog>($"{user.DisplayName} 권한 관리", param);
        var result = await dialog.Result;

        if (result.Canceled)
        {
            return;
        }

        if (result.Data is UserRoleResult userRoleResult)
        {
            var userRoles = userRoleResult.Role;
            var appliedUser = userRoleResult.User;
            if (user == appliedUser && !IsSameRole(user.Roles, userRoles))
            {
                // user.Roles와 userRoles를 비교해서 변경된 것만 변경하도록 해야함
                var removeRoles = user.Roles.Except(userRoles).ToArray();
                var addRoles = userRoles.Except(user.Roles).ToArray();
                foreach (var role in removeRoles)
                {
                    await UserManager.RemoveFromRoleAsync(user, role.ToString());
                }
                foreach (var role in addRoles)
                {
                    await UserManager.AddToRoleAsync(user, role.ToString());
                }
            }
        }

        bool IsSameRole(IEnumerable<UserRole> roles1, IEnumerable<UserRole> roles2)
        {
            return roles1.OrderBy(x => x).SequenceEqual(roles2.OrderBy(x => x));
        }
    }
}

record UserRoleResult(AppUser User, List<UserRole> Role);

