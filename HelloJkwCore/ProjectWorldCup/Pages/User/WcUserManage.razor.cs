namespace ProjectWorldCup.Pages.User;

public partial class WcUserManage : JkwPageBase
{
    [Inject] public IBettingService BettingService { get; set; }
    [Parameter] public string UserId { get; set; }

    BettingUser TargetUser { get; set; } = null;

    BettingHistory InputHistory = new();

    protected override Task OnPageInitializedAsync()
    {
        if (!IsAuthenticated)
        {
            Navi.NavigateTo("/worldcup");
            return Task.CompletedTask;
        }
        if (User?.HasRole(UserRole.WcManager) ?? false)
        {
            // GOOD !
        }
        else
        {
            Navi.NavigateTo("/worldcup");
        }
        return Task.CompletedTask;
    }

    protected override async Task OnPageParametersSetAsync()
    {
        if (User?.HasRole(UserRole.WcManager) ?? false)
        {
            var appUser = await UserStore.FindByIdAsync(UserId.Replace("_", "."), default);
            if (appUser != null)
            {
                TargetUser = await BettingService.GetBettingUserAsync(appUser);
                StateHasChanged();
            }
        }
    }

    private async Task AddHistoryAsync()
    {
        if (string.IsNullOrEmpty(InputHistory.Comment))
            return;

        var addHistory = new BettingHistory
        {
            Type = InputHistory.Type,
            Comment = InputHistory.Comment,
            Value = InputHistory.Value,
            ResultUrl = InputHistory.ResultUrl,
        };
        InputHistory = new();
        TargetUser = await BettingService.AddHistoryAsync(TargetUser, addHistory);
        StateHasChanged();
    }
}
