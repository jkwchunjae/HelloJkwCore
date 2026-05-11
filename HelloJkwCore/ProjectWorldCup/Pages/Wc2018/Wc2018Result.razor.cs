namespace ProjectWorldCup.Pages.Wc2018;

public partial class Wc2018Result : JkwPageBase
{
    class SummaryItem
    {
        public string Nickname { get; set; }
        public int Reward1 { get; set; }
        public int Reward2 { get; set; }
        public int Reward3 { get; set; }
        public int Total => Reward1 + Reward2 + Reward3;
    }
    [Inject]
    private IBettingService BettingService { get; set; }

    private List<WcBettingItem<Team>> GroupStageItems { get; set; } = new();
    private List<WcBettingItem<Team>> Round16Items { get; set; } = new();
    private List<WcFinalBettingItem<Team>> FinalItems { get; set; } = new();

    protected override async Task OnPageInitializedAsync()
    {
        GroupStageItems = await BettingService.Get2018GroupStageBettingResult();
        Round16Items = await BettingService.Get2018Round16BettingResult();
        FinalItems = await BettingService.Get2018FinalBettingResult();
    }
}
