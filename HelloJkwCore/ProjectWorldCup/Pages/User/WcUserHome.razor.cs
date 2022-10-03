using Microsoft.JSInterop;

namespace ProjectWorldCup.Pages.User;

public partial class WcUserHome : JkwPageBase
{
    [Inject] public IBettingService BettingService { get; set; }

    BettingUser BettingUser { get; set; }
    bool JoinedUser => BettingUser?.JoinStatus == UserJoinStatus.Joined;

    protected override async Task OnPageInitializedAsync()
    {
        if (IsAuthenticated)
        {
            BettingUser = await BettingService.GetBettingUserAsync(User);
        }

        if (!JoinedUser)
        {
            Navi.NavigateTo("/worldcup");
        }
    }
}
