namespace ProjectWorldCup.Pages.Betting;

public partial class BettingFinalResultComponent : JkwPageBase
{
    [Parameter]
    public List<WcFinalBettingItem<Team>> BettingItems { get; set; }
    [Parameter] public bool Visible { get; set; } = false;
    BettingResultTable<WcFinalBettingItem<Team>> BettingResult { get; set; }
    WcFinalBettingItem<Team> FirstItem => BettingResult?.FirstOrDefault();
    public BettingFinalResultComponent()
    {
        BettingResult = new BettingResultTable<WcFinalBettingItem<Team>>(new List<WcFinalBettingItem<Team>>());
    }

    protected override void OnPageParametersSet()
    {
        BettingResult = new BettingResultTable<WcFinalBettingItem<Team>>(BettingItems);
    }
}
