using HelloJkwCore.Authentication;
using JkwExtensions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HelloJkwCore.Components.Account;

public partial class UserManagePage : JkwPageBase2
{
    [Inject] AppUserManager UserManager { get; set; } = default!;
    [Inject] IDialogService DialogService { get; set; } = default!;

    private List<ApplicationUser> Users { get; set; } = new();

    private IEnumerable<ApplicationUser> FilteredUsers => Users;

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

    private async Task DeleteUserAsync(ApplicationUser user)
    {
        await UserManager.DeleteAsync(user);

        Users.Remove(user);
        StateHasChanged();
    }

    private async Task ManageUserRole(ApplicationUser user)
    {
        var param = new DialogParameters
        {
            ["User"] = user,
        };
        DialogOptions options = new DialogOptions() { CloseOnEscapeKey = true };
        var dialog = DialogService.Show<UserRoleDialog>($"{user.DisplayName} 권한 관리", param, options);
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

record UserRoleResult(ApplicationUser User, List<UserRole> Role);