namespace ProjectWorldCup.Pages.Wc2022;

public partial class Betting2022Round16 : JkwPageBase
{
    [Inject] public ISnackbar Snackbar { get; set; }
    [Inject] IWorldCupService WorldCupService { get; set; }
    [Inject] IBettingService BettingService { get; set; }
    [Inject] IBettingRound16Service BettingRound16Service { get; set; }
    //[Inject] IFifa Fifa { get; set; }

    List<KnMatch> Round16Matches = new();
    WcBettingItem<Team> BettingItem = new();
    List<WcBettingItem<Team>> BettingItems = null;

    bool AllMatchesAreSetted => Round16Matches.SelectMany(m => m.Teams)
        .All(team => team?.Id != null && team.Id.Length == 3);

    bool[] TimeOver = new[] { false, false, false, false, false, false, false, false };
    bool CheckRandom1 = false;
    bool CheckRandom2 = false;
    bool CheckRandom3 = false;

    protected override async Task OnPageInitializedAsync()
    {
        if (!IsAuthenticated)
        {
            Navi.NavigateTo("/worldcup");
            return;
        }
        Round16Matches = await WorldCupService.GetRound16MatchesAsync();
        var bettingUser = await BettingService.GetBettingUserAsync(User);
        BettingItem = await BettingRound16Service.GetBettingAsync(bettingUser);
        BettingItems = await BettingRound16Service.GetAllBettingsAsync();
    }

    private async Task PickTeamAsync(int matchIndex, Team team)
    {
        if (TimeOver[matchIndex])
            return;

        if (team?.Id == null)
            return;

        try
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
        catch (Exception ex)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add(ex.Message, Severity.Normal);
        }
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

    private void OnTimeOver(int index)
    {
        TimeOver[index] = true;
        if (index == 2)
        {
            for (var i = 2; i < 8; i++)
            {
                TimeOver[i] = true;
            }
        }
    }

    private async Task SelectFullRandom()
    {
        if (TimeOver.Any(x => x))
            return;
        if (!AllMatchesAreSetted)
            return;
        if (BettingItem?.IsRandom ?? false)
            return;

        try
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

            BettingItem = await BettingRound16Service.PickRandomAsync(bettingUser);
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add(ex.Message, Severity.Normal);
        }
    }
}
