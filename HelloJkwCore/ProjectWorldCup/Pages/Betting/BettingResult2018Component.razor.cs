namespace ProjectWorldCup.Pages.Betting;

public partial class BettingResult2018Component : JkwPageBase
{
    [Parameter]
    public List<WcBettingItem<Team>> BettingItems { get; set; }

    BettingResultTable<WcBettingItem<Team>> BettingResult { get; set; }

    public BettingResult2018Component()
    {
        BettingResult = new BettingResultTable<WcBettingItem<Team>>(new List<WcBettingItem<Team>>());
    }

    protected override void OnPageParametersSet()
    {
        BettingResult = new BettingResultTable<WcBettingItem<Team>>(BettingItems, new BettingTableOption
        {
            RewardForUser = reward => (reward / 10) * 10,
        });
    }
}
