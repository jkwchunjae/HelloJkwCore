namespace ProjectWorldCup.Pages.Wc2018;

public partial class BettingFinalResult2018Component : JkwPageBase
{
    [Parameter]
    public List<WcFinalBettingItem<Team>> BettingItems { get; set; }

    BettingResultTable<WcFinalBettingItem<Team>> BettingResult { get; set; }

    WcFinalBettingItem<Team> FirstItem => BettingResult?.FirstOrDefault();

    public BettingFinalResult2018Component()
    {
        BettingResult = new BettingResultTable<WcFinalBettingItem<Team>>(new List<WcFinalBettingItem<Team>>());
    }

    protected override void OnPageParametersSet()
    {
        BettingResult = new BettingResultTable<WcFinalBettingItem<Team>>(BettingItems, new BettingTableOption
        {
            RewardForUser = reward => (reward / 10) * 10,
        });
    }
}
