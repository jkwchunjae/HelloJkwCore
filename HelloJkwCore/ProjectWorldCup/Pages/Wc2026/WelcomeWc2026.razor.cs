namespace ProjectWorldCup.Pages.Wc2026;

public partial class WelcomeWc2026 : JkwPageBase
{
    [Inject] IBettingService Service { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }

    BettingUser BettingUser;
    bool ReadBlog = false;

    protected override async Task OnPageInitializedAsync()
    {
        if (IsAuthenticated)
        {
            BettingUser = await Service.GetBettingUserAsync(User);

            if (BettingUser?.JoinStatus == UserJoinStatus.Joined)
            {
                Navi.NavigateTo("/worldcup/2026/betting");
            }
        }
    }

    private async Task JoinAsync()
    {
        if (IsAuthenticated)
        {
            BettingUser = await Service.MakeJoinRequestAsync(User);
            StateHasChanged();
        }
    }

    private async Task CancelAsync()
    {
        if (IsAuthenticated && BettingUser?.JoinStatus == UserJoinStatus.Requested)
        {
            await Service.CancelJoinRequestAsync(User);
            BettingUser = await Service.GetBettingUserAsync(User);
            StateHasChanged();
        }
    }

    private void OnClickBlogLink()
    {
        ReadBlog = true;
    }
}
