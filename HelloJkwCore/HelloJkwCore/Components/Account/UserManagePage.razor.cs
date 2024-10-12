using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HelloJkwCore.Components.Account;

public partial class UserManagePage : JkwPageBase
{
    [Inject] AppUserManager UserManager { get; set; } = default!;
    [Inject] IDialogService DialogService { get; set; } = default!;

    private List<AppUser>? Users { get; set; }

    private TvOptions? tvOptions;

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
        tvOptions = MakeTvOptions();
    }

    private async Task DeleteUserAsync(AppUser user)
    {
        await UserManager.DeleteAsync(user);

        if (Users != null)
        {
            Users.Remove(user);
            StateHasChanged();
        }
    }

    private async Task ManageUserRole(AppUser user)
    {
        var param = new DialogParameters
        {
            ["User"] = user,
        };
        DialogOptions options = new DialogOptions() { CloseOnEscapeKey = true };
        var dialog = DialogService.Show<UserRoleDialog>($"{user.DisplayName} 권한 관리", param, options);
        var result = await dialog.Result;

        if (result?.Canceled ?? true)
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

    private TvOptions MakeTvOptions()
    {
        return new TvOptions()
        {
            Title = "사용자 관리",
            GlobalOpenDepth = 1,
            ColumnVisible =
            {
                new TvColumnVisibleOption
                {
                    Before = [nameof(User.NickName), nameof(User.Id), nameof(User.UserName), nameof(User.CreateTime), nameof(User.LastLoginTime)],
                    After = [nameof(User.Id), nameof(User.UserName), nameof(User.NickName), nameof(User.CreateTime), nameof(User.LastLoginTime), nameof(User.Roles), nameof(User.Logins)],
                }
            },
            Buttons =
            {
                new TvPopupAction<AppUser>
                {
                    Action = DeleteUserAsync,
                    Label = "삭제",
                    Style =
                    {
                        Color = Color.Error,
                        StartIcon = Icons.Material.Filled.Delete,
                        IconColor = Color.Error,
                    },
                    PopupTitle = (user) => $"유저 삭제",
                    PopupContent = (user) => $"정말로 {user.DisplayName}을 삭제하시겠습니까?",
                    InnerButtonOptions =
                    {
                        ConfirmLabel = "삭제",
                        ConfirmButtonStyle =
                        {
                            Color = Color.Error,
                        },
                        CloseLabel = "취소",
                        CloseButtonStyle =
                        {
                            Color = Color.Dark,
                            Variant = Variant.Outlined,
                        },
                    },
                },
                new TvAction<AppUser>
                {
                    Action = ManageUserRole,
                    Label = "권한 관리",
                },
            },
            DateTime = new TvDateTimeOption
            {
                Format = "yyyy-MM-dd HH:mm:ss",
            },
            DisableKeys =
            {
                nameof(AppLoginInfo.ProviderDisplayName),
                nameof(AppLoginInfo.ConnectedUserId),
                nameof(AppLoginInfo.LoginInfo),
                nameof(AppLoginInfo.계정종류),
                nameof(AppLoginInfo.아이디),
                nameof(AppLoginInfo.연결시간),
            },
        };
    }
}

record UserRoleResult(AppUser User, List<UserRole> Role);