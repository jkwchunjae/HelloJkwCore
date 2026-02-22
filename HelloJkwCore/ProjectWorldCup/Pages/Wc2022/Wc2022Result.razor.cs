namespace ProjectWorldCup.Pages.Wc2022;

public partial class Wc2022Result : JkwPageBase
{
    [Inject]
    private IWorldCupResultService WorldCupService2022 { get; set; }

    private List<WcFinalBettingItem<Team>> BettingItemsFinal { get; set; } = new();
    private List<WcBettingItem<Team>> BettingItemsRound16 { get; set; } = new();
    private List<WcBettingItem<Team>> BettingItemsGroupStage { get; set; } = new();
    private List<User2022Result> BettingSummary { get; set; } = new();

    protected override async Task OnPageInitializedAsync()
    {
        BettingItemsGroupStage = await WorldCupService2022.Get2022GroupStageBettingResultAsync();
        BettingItemsRound16 = await WorldCupService2022.Get2022Round16BettingResultAsync();
        BettingItemsFinal = await WorldCupService2022.Get2022FinalBettingResultAsync();
        BettingSummary = await WorldCupService2022.Get2022BettingSummaryAsync();
    }
}
