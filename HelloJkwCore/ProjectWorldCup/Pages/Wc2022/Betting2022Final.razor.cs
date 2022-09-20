﻿using Microsoft.JSInterop;

namespace ProjectWorldCup.Pages.Wc2022;

public partial class Betting2022Final : JkwPageBase
{
    [Inject] IWorldCupService WorldCupService { get; set; }
    [Inject] IBettingService BettingService { get; set; }
    [Inject] IBettingFinalService BettingFinalService { get; set; }

    List<(string StageId, List<KnMatch> Matches)> StageMatches { get; set; } = new();

    List<KnMatch> Matches { get; set; } = new();
    WcFinalBettingItem<Team> BettingItem { get; set; }

    Dictionary<string, string> StageName = new Dictionary<string, string>
    {
        [Fifa.Round8StageId] = "8강",
        [Fifa.Round4StageId] = "4강",
        [Fifa.ThirdStageId] = "3,4위",
        [Fifa.FinalStageId] = "결승",
    };

    protected override async Task OnPageInitializedAsync()
    {
        Matches = await WorldCupService.GetFinalMatchesAsync();

        var quarterFinalMatches = await WorldCupService.GetQuarterFinalMatchesAsync();
        StageMatches.Add((Fifa.Round8StageId, quarterFinalMatches));

        var bettingUser = await BettingService.GetBettingUserAsync(User);
        BettingItem = await BettingFinalService.GetBettingAsync(bettingUser);
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

    private string GetRemainTime(List<KnMatch> quarters)
    {
        var startTime = quarters.Min(match => match.Time);
        return startTime.ToString();
    }

    private TeamButtonType GetButtonType(string stageId, Team team)
    {
        return BettingFinalService.GetButtonType(stageId, team, StageMatches, BettingItem);
    }

    private async Task PickTeamAsync(string stageId, string matchId, Team team)
    {
        await Js.InvokeVoidAsync("console.log", stageId, matchId, team);
        StageMatches = BettingFinalService.PickTeamAsync(stageId, matchId, team, StageMatches, Matches);

        if (stageId == Fifa.ThirdStageId)
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
        }
    }
}
