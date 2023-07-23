using Microsoft.AspNetCore.Identity;

namespace HelloJkwCore.Pages.Users;

public partial class UserList : JkwPageBase
{
    [Inject]
    private UserManager<AppUser> UserManager { get; set; }

    private List<AppUser> Users { get; set; }

    private IEnumerable<AppUser> FilteredUsers => Users
        ?.Where(user => CheckedRoles.Empty() ? true :
            CheckedRoles.All(role => user.Roles?.Contains(role) ?? false));

    private List<UserRole> CheckedRolesData { get; set; }

    private HashSet<UserRole> CheckedRoles = new();

    public UserList()
    {
        CheckedRolesData = typeof(UserRole).GetValues<UserRole>().ToList();
    }

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

    private Task FilterRoleChangedAsync(UserRole role, bool check)
    {
        if (check)
        {
            CheckedRoles.Add(role);
        }
        else
        {
            CheckedRoles.Remove(role);
        }

        StateHasChanged();

        return Task.CompletedTask;
    }

    private async Task DeleteUserAsync(AppUser user)
    {
        await UserManager.DeleteAsync(user);

        Users.Remove(user);
        StateHasChanged();
    }
}