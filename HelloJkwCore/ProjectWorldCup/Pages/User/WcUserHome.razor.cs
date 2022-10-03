using Microsoft.JSInterop;

namespace ProjectWorldCup.Pages.User;

public partial class WcUserHome : JkwPageBase
{
    [Inject] public ISnackbar Snackbar { get; set; }
    [Inject] public IBettingService BettingService { get; set; }

    BettingUser BettingUser { get; set; }
    bool JoinedUser => BettingUser?.JoinStatus == UserJoinStatus.Joined;

    string InputNickname { get; set; }

    protected override async Task OnPageInitializedAsync()
    {
        if (IsAuthenticated)
        {
            InputNickname = User.NickName ?? User.UserName;
            BettingUser = await BettingService.GetBettingUserAsync(User);
        }

        if (!JoinedUser)
        {
            Navi.NavigateTo("/worldcup");
        }
    }

    private async Task ChangeNickname()
    {
        if (!CheckNickname(InputNickname))
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add("적절하지 않은 이름입니다", Severity.Warning);
            return;
        }
        User.NickName = InputNickname;
        await BettingService.AddHistoryAsync(BettingUser, new BettingHistory
        {
            Type = HistoryType.ChangeNickname,
            Value = 0,
            Comment = $"닉네임 변경 ({InputNickname})",
        });
        await UserStore.UpdateAsync(User, default);
        StateHasChanged();
    }

    private async Task ChangeNickname500()
    {
        if (!CheckNickname(InputNickname))
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add("적절하지 않은 이름입니다", Severity.Warning);
            return;
        }
        await BettingService.AddHistoryAsync(BettingUser, new BettingHistory
        {
            Type = HistoryType.ChangeNickname,
            Value = -500,
            Comment = $"닉네임 변경 ({InputNickname})",
        });
        User.NickName = InputNickname;
        await UserStore.UpdateAsync(User, default);
        await Task.Delay(TimeSpan.FromSeconds(1));
        StateHasChanged();
    }

    private static bool CheckNickname(string nickname)
    {
        nickname = nickname.Trim();
        if (string.IsNullOrWhiteSpace(nickname))
            return false;
        if (nickname.Length > 15)
            return false;
        return true;
    }
}
