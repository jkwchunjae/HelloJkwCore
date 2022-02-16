namespace ProjectWorldCup.Pages;

public partial class Wc2018Result : JkwPageBase
{
    [Inject]
    private IWorldCupService WorldCupService { get; set; }

    private List<WcBettingItem> GroupStageItems { get; set; } = new();
    private List<WcBettingItem> Round16Items { get; set; } = new();
    private List<WcFinalBettingItem> FinalItems { get; set; } = new();

    protected override async Task OnPageInitializedAsync()
    {
        GroupStageItems = await WorldCupService.Get2018GroupStageBettingResult();
        Round16Items = await WorldCupService.Get2018Round16BettingResult();
        FinalItems = await WorldCupService.Get2018FinalBettingResult();
    }
}
