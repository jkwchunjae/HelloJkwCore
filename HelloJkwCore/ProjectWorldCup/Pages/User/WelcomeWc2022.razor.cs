namespace ProjectWorldCup.Pages.User;

public partial class WelcomeWc2022 : JkwPageBase
{
    [Inject] IBettingService Service { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }

    BettingUser BettingUser;
    protected override async Task OnPageInitializedAsync()
    {
        if (IsAuthenticated)
        {
            BettingUser = await Service.GetBettingUserAsync(User);

            if (BettingUser?.JoinStatus == UserJoinStatus.Joined)
            {
                Navi.NavigateTo("/worldcup/2022/betting"); // TODO: 가입 한 사람이 가야할 곳으로 안내
            }
        }
    }

    private async Task JoinAsync()
    {
        if (IsAuthenticated)
        {
            if (BettingUser?.BettingHistories?.Count > 13)
            {
                await Service.RejectUserAsync(BettingUser, null);
                BettingUser = await Service.GetBettingUserAsync(User);
                return;
            }
            BettingUser = await Service.MakeJoinRequestAsync(User);
            StateHasChanged();
        }
    }

    private async Task CancelAsync()
    {
        if (IsAuthenticated && BettingUser?.JoinStatus == UserJoinStatus.Requested)
        {
            if (BettingUser.BettingHistories.Count > 15)
            {
                return;
            }
            if (BettingUser.BettingHistories.Count > 10)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add("장난하지 마십시요", Severity.Error);
            }
            await Service.CancelJoinRequestAsync(User);
            BettingUser = await Service.GetBettingUserAsync(User);
            StateHasChanged();
        }
    }
}
