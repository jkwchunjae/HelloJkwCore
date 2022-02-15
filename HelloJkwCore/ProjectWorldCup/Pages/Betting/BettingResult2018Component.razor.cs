namespace ProjectWorldCup.Pages.Betting;

public partial class BettingResult2018Component : JkwPageBase
{
    [Parameter]
    public List<WcBettingItem> BettingItems { get; set; }

    BettingResultTable<WcBettingItem> BettingResult { get; set; }

    public BettingResult2018Component()
    {
        BettingResult = new BettingResultTable<WcBettingItem>(new List<WcBettingItem>());
    }

    protected override void OnPageParametersSet()
    {
        BettingResult = new BettingResultTable<WcBettingItem>(BettingItems, new BettingTableOption
        {
            RewardForUser = reward => (reward / 10) * 10,
        });
    }
}
