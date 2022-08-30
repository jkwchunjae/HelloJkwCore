namespace ProjectWorldCup.Pages.User;

public partial class WcUserListPage : JkwPageBase
{
    [Inject] IBettingService Service { get; set; }

    IEnumerable<BettingUser> Users { get; set; } = new List<BettingUser>();
    bool HasRole => IsAuthenticated && User.HasRole(UserRole.WcManager);

    protected override async Task OnPageInitializedAsync()
    {
        if (HasRole)
        {
            Users = await Service.GetBettingUsersAsync();
        }
    }

    private async Task ApproveAsync(BettingUser targetUser)
    {
        if (HasRole)
        {
            await Service.ApproveUserAsync(targetUser, User);
            Users = await Service.GetBettingUsersAsync();
            StateHasChanged();
        }
    }

    private async Task RejectAsync(BettingUser targetUser)
    {
        if (HasRole)
        {
            await Service.RejectUserAsync(targetUser, User);
            Users = await Service.GetBettingUsersAsync();
            StateHasChanged();
        }
    }
}
