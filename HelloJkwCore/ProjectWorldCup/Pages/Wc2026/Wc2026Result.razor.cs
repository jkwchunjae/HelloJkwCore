namespace ProjectWorldCup.Pages.Wc2026;

public partial class Wc2026Result : JkwPageBase
{
    [Inject(Key = "2026")]
    private IBettingResultService WorldCupService2026 { get; set; }

    private List<WcFinalBettingItem<Team>> BettingItemsFinal { get; set; } = new();
    private List<WcBettingItem<Team>> BettingItemsRound16 { get; set; } = new();
    private List<WcBettingItem<Team>> BettingItemsRound32 { get; set; } = new();
    private List<WcBettingItem<Team>> BettingItemsGroupStage { get; set; } = new();
    private List<UserResult> BettingSummary { get; set; } = new();

    protected override async Task OnPageInitializedAsync()
    {
        BettingItemsGroupStage = await WorldCupService2026.GetGroupStageBettingResultAsync();
        BettingItemsRound32 = await WorldCupService2026.GetRound32BettingResultAsync();
        BettingItemsRound16 = await WorldCupService2026.GetRound16BettingResultAsync();
        BettingItemsFinal = await WorldCupService2026.GetFinalBettingResultAsync();
        BettingSummary = await WorldCupService2026.GetBettingSummaryAsync();
    }
}
