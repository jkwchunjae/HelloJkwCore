using Microsoft.JSInterop;

namespace ProjectWorldCup.Pages.Wc2022;

public partial class Betting2022Final : JkwPageBase
{
    [Inject] IWorldCupService WorldCupService { get; set; }
    [Inject] IBettingService BettingService { get; set; }
    [Inject] IBettingFinalService BettingFinalService { get; set; }

    private BettingUser BettingUser { get; set; }
    List<(string StageId, List<KnMatch> Matches)> StageMatches { get; set; } = new();

    List<KnMatch> Matches { get; set; } = new();
    WcFinalBettingItem<Team> BettingItem { get; set; }
    List<WcFinalBettingItem<Team>> BettingItems { get; set; }

    Dictionary<string, string> StageName = new Dictionary<string, string>
    {
        [Fifa.Round8StageId] = "8강전",
        [Fifa.Round4StageId] = "4강전",
        [Fifa.ThirdStageId] = "3,4위전",
        [Fifa.FinalStageId] = "결승전",
    };
    bool AllMatchesAreSetted => StageMatches.Any() && StageMatches[0].Matches
        .All(match => match.Teams.All(team => team?.Id?.Length == 3));
    bool TimeOver = false;
    bool CheckRandom1 = false;
    bool CheckRandom2 = false;
    bool CheckRandom3 = false;

    protected override async Task OnPageInitializedAsync()
    {
        BettingUser = await BettingService.GetBettingUserAsync(User);
        Matches = await WorldCupService.GetFinalMatchesAsync();

        var quarterFinalMatches = await WorldCupService.GetQuarterFinalMatchesAsync();
        StageMatches.Add((Fifa.Round8StageId, quarterFinalMatches));

        //await Js.InvokeVoidAsync("console.log", quarterFinalMatches, AllMatchesAreSetted);

        var bettingUser = await BettingService.GetBettingUserAsync(User);
        BettingItem = await BettingFinalService.GetBettingAsync(bettingUser);
        BettingItems = await BettingFinalService.GetAllBettingsAsync();
        EvaluateUserBetting();
    }

    private void EvaluateUserBetting()
    {
        var quarters = StageMatches.First().Matches;
        //BettingItem.Picked = new List<Team>
        //    {
        //        quarters[0].HomeTeam,
        //        quarters[2].HomeTeam,
        //        quarters[1].AwayTeam,
        //        quarters[3].AwayTeam,
        //    };
        if (BettingItem?.Picked?.Count() == 4)
        {
            StageMatches = BettingFinalService.EvaluateUserBetting(quarters, BettingItem, Matches);
            StateHasChanged();
        }
    }

    private TeamButtonType GetButtonType(string stageId, Team team)
    {
        return BettingFinalService.GetButtonType(stageId, team, StageMatches, BettingItem);
    }

    private async Task PickTeamAsync(string stageId, string matchId, Team team)
    {
        if (TimeOver)
            return;
        if (BettingItem?.IsRandom ?? false)
            return;
        if (team?.Id == null)
            return;

        var bettingUser = await BettingService.GetBettingUserAsync(User);
        if (bettingUser.JoinedBetting.Empty(x => x == BettingType.Final))
        {
            bettingUser = await BettingService.JoinBettingAsync(bettingUser, BettingType.Final);
        }
        if (bettingUser.JoinedBetting.Empty(x => x == BettingType.Final))
        {
            // 참가할 수 없는 경우
            return;
        }

        StageMatches = BettingFinalService.PickTeam(stageId, matchId, team, StageMatches, Matches);

        if (stageId == Fifa.Round8StageId)
        {
            BettingItem.Picked = new List<Team> { null, null, null, null };
        }
        if (stageId == Fifa.Round4StageId)
        {
            BettingItem.Picked = new List<Team> { null, null, null, null };
        }
        else if (stageId == Fifa.ThirdStageId)
        {
            var third = StageMatches.First(s => s.StageId == Fifa.ThirdStageId).Matches[0];
            var loser = third.HomeTeam == team ? third.AwayTeam : third.HomeTeam;
            BettingItem.Picked = new List<Team>
            {
                BettingItem.Pick0,
                BettingItem.Pick1,
                team,
                loser,
            };
        }
        else if (stageId == Fifa.FinalStageId)
        {
            var final = StageMatches.First(s => s.StageId == Fifa.FinalStageId).Matches[0];
            var loser = final.HomeTeam == team ? final.AwayTeam : final.HomeTeam;
            BettingItem.Picked = new List<Team>
            {
                team,
                loser,
                BettingItem.Pick2,
                BettingItem.Pick3,
            };
        }

        if (BettingItem.Picked?.Count(x => x != null) == 4)
        {
            await BettingFinalService.SaveBettingItemAsync(BettingItem);
        }
    }

    private async Task SelectFullRandom()
    {
        if (TimeOver)
            return;
        if (BettingItem?.IsRandom ?? false)
            return;
        if (!AllMatchesAreSetted)
            return;

        var bettingUser = await BettingService.GetBettingUserAsync(User);
        if (bettingUser.JoinedBetting.Empty(x => x == BettingType.Final))
        {
            bettingUser = await BettingService.JoinBettingAsync(bettingUser, BettingType.Final);
        }
        if (bettingUser.JoinedBetting.Empty(x => x == BettingType.Final))
        {
            // 참가할 수 없는 경우
            return;
        }

        (StageMatches, var pickTeams) = BettingFinalService.PickRandom(StageMatches, Matches);
        BettingItem.Picked = pickTeams;
        BettingItem.IsRandom = true;

        await BettingFinalService.SaveBettingItemAsync(BettingItem);
        StateHasChanged();
    }
}
