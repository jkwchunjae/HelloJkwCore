namespace ProjectWorldCup.Pages;

public partial class Wc2018Result : JkwPageBase
{
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
