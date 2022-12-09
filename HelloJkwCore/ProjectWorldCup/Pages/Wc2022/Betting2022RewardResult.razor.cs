namespace ProjectWorldCup.Pages.Wc2022;

public partial class Betting2022RewardResult : JkwPageBase
{
    [Inject] IBettingService BettingService { get; set; }
    [Inject] IBettingGroupStageService BettingGroupStageService { get; set; }
    [Inject] IBettingRound16Service BettingRound16Service { get; set; }
    [Inject] IBettingFinalService BettingFinalService { get; set; }
    [Inject] IWorldCupService WorldCupService { get; set; }

    List<WcBettingItem<GroupTeam>> GroupStageResult;
    List<WcBettingItem<Team>> Round16Result;
    List<WcFinalBettingItem<Team>> FinalResult;

    IEnumerable<ITeam> GroupStageFixed;
    IEnumerable<ITeam> Round16Fixed;
    IEnumerable<ITeam> FinalFixed;

    string GroupStageButtonText = "정산";
    string Round16ButtonText = "정산";
    string FinalButtonText = "정산";

    MudTabs tabs;

    protected override async Task OnPageInitializedAsync()
    {
        if (!IsAuthenticated)
        {
            Navi.NavigateTo("/worldcup");
            return;
        }
        GroupStageResult = await BettingGroupStageService.GetAllBettingsAsync();
        Round16Result = await BettingRound16Service.GetAllBettingsAsync();
        FinalResult = await BettingFinalService.GetAllBettingsAsync();

        var knockoutMatches = await WorldCupService.GetKnockoutStageMatchesAsync();
        GroupStageFixed = knockoutMatches.Where(m => m.StageId == Fifa.Round16StageId)
            .SelectMany(m => new[] { m.HomeTeam, m.AwayTeam })
            .ToList();
        Round16Fixed = knockoutMatches.Where(m => m.StageId == Fifa.Round8StageId)
            .SelectMany(m => new[] { m.HomeTeam, m.AwayTeam })
            .ToList();
        FinalFixed = new List<ITeam>
        {
            knockoutMatches.First(m => m.StageId == Fifa.FinalStageId).Winner.Team,
            knockoutMatches.First(m => m.StageId == Fifa.FinalStageId).Looser.Team,
            knockoutMatches.First(m => m.StageId == Fifa.ThirdStageId).Winner.Team,
            knockoutMatches.First(m => m.StageId == Fifa.ThirdStageId).Looser.Team,
        };
    }

    protected override void OnPageAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            if (DateTime.Now < WorldCupConst.Round8StartTime)
            {
                tabs.ActivatePanel(0);
            }
            else if (DateTime.Now < WorldCupConst.FinalStartTime)
            {
                tabs.ActivatePanel(1);
            }
            else
            {
                tabs.ActivatePanel(2);
            }
        }
    }

    private async Task ApplyReward(IEnumerable<IWcBettingItem<ITeam>> bettingResults, HistoryType rewardType, Action<string> buttonTextAction)
    {
        var total = bettingResults.Count();
        var count = 0;
        var failCount = 0;
        foreach (var result in bettingResults)
        {
            var user = await BettingService.GetBettingUserAsync(result.User);
            if (user != default)
            {
                await BettingService.AddRewardAsync(user, rewardType, result.Reward);
                count++;
            }
            else
            {
                failCount++;
            }
            if (failCount > 0)
            {
                buttonTextAction.Invoke($"정산 ({count}/{total}). fail: {failCount}");
            }
            else
            {
                buttonTextAction.Invoke($"정산 ({count}/{total})");
            }
            StateHasChanged();
        }
    }

    private async Task ApplyRewardGroupStage()
    {
        await ApplyReward(
            bettingResults: GroupStageResult,
            rewardType: HistoryType.Reward1,
            buttonTextAction: (text) => GroupStageButtonText = text
        );
    }

    private async Task ApplyRewardRound16()
    {
        await ApplyReward(
            bettingResults: Round16Result,
            rewardType: HistoryType.Reward2,
            buttonTextAction: (text) => Round16ButtonText = text
        );
    }

    private async Task ApplyRewardFinal()
    {
        await ApplyReward(
            bettingResults: FinalResult,
            rewardType: HistoryType.Reward3,
            buttonTextAction: (text) => FinalButtonText = text
        );
    }
}
