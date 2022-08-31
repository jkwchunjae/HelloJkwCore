namespace ProjectWorldCup.Pages.User;

public partial class NotJoinedUserPage : JkwPageBase
{
    [Inject] IBettingService Service { get; set; }

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
            BettingUser = await Service.MakeJoinRequestAsync(User);
            StateHasChanged();
        }
    }
}
