namespace ProjectWorldCup.Pages.Wc2026;

public partial class Betting2026RewardResult : JkwPageBase
{
    [Inject] IBettingService BettingService { get; set; }
    [Inject(Key = "2026")] IBettingGroupStageService BettingGroupStageService { get; set; }
    [Inject(Key = "2026-round32")] IBettingRound16Service BettingRound32Service { get; set; }
    [Inject(Key = "2026-round16")] IBettingRound16Service BettingRound16Service { get; set; }
    [Inject(Key = "2026")] IBettingFinalService BettingFinalService { get; set; }
    [Inject] IWorldCupService WorldCupService { get; set; }

    List<WcBettingItem<GroupTeam>> GroupStageResult;
    List<WcBettingItem<Team>> Round32Result;
    List<WcBettingItem<Team>> Round16Result;
    List<WcFinalBettingItem<Team>> FinalResult;

    string GroupStageButtonText = "정산";
    string Round32ButtonText = "정산";
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
        Round32Result = await BettingRound32Service.GetAllBettingsAsync();
        Round16Result = await BettingRound16Service.GetAllBettingsAsync();
        FinalResult = await BettingFinalService.GetAllBettingsAsync();
    }

    protected override void OnPageAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            if (DateTime.Now < WorldCupConst.Round16Match1StartTime)
            {
                tabs.ActivatePanel(0);
            }
            else if (DateTime.Now < WorldCupConst.Round8StartTime)
            {
                tabs.ActivatePanel(1);
            }
            else if (DateTime.Now < WorldCupConst.FinalStartTime)
            {
                tabs.ActivatePanel(2);
            }
            else
            {
                tabs.ActivatePanel(3);
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

    private async Task ApplyRewardRound32()
    {
        await ApplyReward(
            bettingResults: Round32Result,
            rewardType: HistoryType.Reward2,
            buttonTextAction: (text) => Round32ButtonText = text
        );
    }


    private async Task ApplyRewardRound16()
    {
        await ApplyReward(
            bettingResults: Round16Result,
            rewardType: HistoryType.Reward3,
            buttonTextAction: (text) => Round16ButtonText = text
        );
    }

    private async Task ApplyRewardFinal()
    {
        await ApplyReward(
            bettingResults: FinalResult,
            rewardType: HistoryType.Reward4,
            buttonTextAction: (text) => FinalButtonText = text
        );
    }
}
