﻿namespace ProjectWorldCup.Pages.Wc2022;

public partial class Betting2022Round16 : JkwPageBase
{
    [Inject] IWorldCupService WorldCupService { get; set; }
    [Inject] IBettingService BettingService { get; set; }
    [Inject] IBettingRound16Service BettingRound16Service { get; set; }
    //[Inject] IFifa Fifa { get; set; }

    List<KnMatch> Round16Matches = new();
    WcBettingItem<Team> BettingItem = new();
    List<WcBettingItem<Team>> BettingItems = null;

    bool[] TimeOver = new[] { false, false, false, false, false, false, false, false };

    protected override async Task OnPageInitializedAsync()
    {
        Round16Matches = await WorldCupService.GetRound16MatchesAsync();
        var bettingUser = await BettingService.GetBettingUserAsync(User);
        BettingItem = await BettingRound16Service.GetBettingAsync(bettingUser);
        BettingItems = await BettingRound16Service.GetAllBettingsAsync();
    }

    private async Task PickTeamAsync(int matchIndex, Team team)
    {
        if (TimeOver[matchIndex])
            return;

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
}