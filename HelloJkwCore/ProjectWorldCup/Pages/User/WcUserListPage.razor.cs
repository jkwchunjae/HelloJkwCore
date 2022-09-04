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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetUser"></param>
    /// <param name="initValue">입금 금액</param>
    /// <returns></returns>
    private async Task ApproveAsync(BettingUser targetUser, int initValue)
    {
        if (HasRole)
        {
            await Service.ApproveUserAsync(targetUser, initValue, User);
            await Service.JoinBettingAsync(targetUser, BettingType.GroupStage);
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

    private async Task ClearUserCache()
    {
        if (User?.HasRole(UserRole.Admin) ?? false)
        {
            Service.ClearUserCache();
            Users = await Service.GetBettingUsersAsync();
            StateHasChanged();
        }
    }
}
