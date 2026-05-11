namespace ProjectWorldCup.Pages.Wc2022;

public partial class Wc2022Result : JkwPageBase
{
    [Inject(Key = "2022")]
    private IBettingResultService WorldCupService2022 { get; set; }

    private List<WcFinalBettingItem<Team>> BettingItemsFinal { get; set; } = new();
    private List<WcBettingItem<Team>> BettingItemsRound16 { get; set; } = new();
    private List<WcBettingItem<Team>> BettingItemsGroupStage { get; set; } = new();
    private List<UserResult> BettingSummary { get; set; } = new();

    protected override async Task OnPageInitializedAsync()
    {
        BettingItemsGroupStage = await WorldCupService2022.GetGroupStageBettingResultAsync();
        BettingItemsRound16 = await WorldCupService2022.GetRound16BettingResultAsync();
        BettingItemsFinal = await WorldCupService2022.GetFinalBettingResultAsync();
        BettingSummary = await WorldCupService2022.GetBettingSummaryAsync();
    }
}
