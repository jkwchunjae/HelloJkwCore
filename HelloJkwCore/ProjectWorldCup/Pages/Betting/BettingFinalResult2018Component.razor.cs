namespace ProjectWorldCup.Pages.Betting;

public partial class BettingFinalResult2018Component : JkwPageBase
{
    [Parameter]
    public List<WcFinalBettingItem> BettingItems { get; set; }

    BettingResultTable<WcFinalBettingItem> BettingResult { get; set; }

    WcFinalBettingItem FirstItem => BettingResult?.FirstOrDefault();

    public BettingFinalResult2018Component()
    {
        BettingResult = new BettingResultTable<WcFinalBettingItem>(new List<WcFinalBettingItem>());
    }

    protected override void OnPageParametersSet()
    {
        BettingResult = new BettingResultTable<WcFinalBettingItem>(BettingItems, new BettingTableOption
        {
            RewardForUser = reward => (reward / 10) * 10,
        });
    }
}
