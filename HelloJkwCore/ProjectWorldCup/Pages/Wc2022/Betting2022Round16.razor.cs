namespace ProjectWorldCup.Pages.Wc2022;

public partial class Betting2022Round16 : JkwPageBase
{
    [Inject] IWorldCupService WorldCupService { get; set; }
    [Inject] IBettingService BettingService { get; set; }
    [Inject] IBettingRound16Service BettingRound16Service { get; set; }
    //[Inject] IFifa Fifa { get; set; }

    List<KnMatch> Round16Matches = new();
    WcBettingItem<Team> BettingItem = new();

    protected override async Task OnPageInitializedAsync()
    {
        Round16Matches = await WorldCupService.GetRound16MatchesAsync();
        var bettingUser = await BettingService.GetBettingUserAsync(User);
        BettingItem = await BettingRound16Service.GetBettingAsync(bettingUser);
    }

    private async Task PickTeamAsync(Team team)
    {
        var bettingUser = await BettingService.GetBettingUserAsync(User);
        if (bettingUser.JoinedBetting.Empty(x => x == BettingType.Round16))
        {
            bettingUser = await BettingService.JoinBettingAsync(bettingUser, BettingType.Round16);
        }
        if (bettingUser.JoinedBetting.Empty(x => x == BettingType.Round16))
        {
            // 참가할 수 없는 경우
            return;
        }
        BettingItem = await BettingRound16Service.PickTeamAsync(bettingUser, team);
        StateHasChanged();
    }

    private string GetRemainTime(DateTime utcTime)
    {
        if (DateTime.UtcNow > utcTime)
        {
            return "경기가 이미 시작했습니다";
        }
        var sub = utcTime - DateTime.UtcNow;

        List<string> result = new();
        if (sub.TotalDays > 0)
            result.Add($"{(int)sub.TotalDays}일");
        if (sub.Hours > 0)
            result.Add($"{sub.Hours}시간");
        if (sub.Minutes > 0)
            result.Add($"{sub.Minutes}분");
        if (sub.Seconds > 0)
            result.Add($"{sub.Seconds}초");

        return result.StringJoin(" ") + "남았습니다";
    }

    private TeamButtonType GetButtonType(Team team)
    {
        if (team == null)
            return TeamButtonType.Disabled;

        if (BettingItem.Picked.Any(x => x == team))
            return TeamButtonType.Picked;

        var match = Round16Matches.FirstOrDefault(m => m.HomeTeam == team || m.AwayTeam == team);

        if (match == null)
            return TeamButtonType.Disabled;

        if (match.Time < DateTime.UtcNow)
            return TeamButtonType.Disabled;

        return TeamButtonType.Pickable;
    }
}
