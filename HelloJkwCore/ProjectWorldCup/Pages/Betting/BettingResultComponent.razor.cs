using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;

namespace ProjectWorldCup.Pages.Betting;

public partial class BettingResultComponent : JkwPageBase
{
    [Inject]
    public IWorldCupService WorldCupService { get; set; }

    [Parameter]
    public List<WcBettingItem<GroupTeam>> BettingItems { get; set; }

    BettingResultTable<WcBettingItem<GroupTeam>> BettingResult { get; set; }

    public BettingResultComponent()
    {
    }

    private Task BettingItemsUpdated()
    {
        BettingResult = new BettingResultTable<WcBettingItem<GroupTeam>>(BettingItems);
        return Task.CompletedTask;
    }

    protected override async Task OnPageInitializedAsync()
    {
        await BettingItemsUpdated();
    }

    protected override async Task OnPageParametersSetAsync()
    {
        await BettingItemsUpdated();
    }
}
