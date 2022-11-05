using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;

namespace ProjectWorldCup.Pages.Betting;

public partial class BettingResultComponent : JkwPageBase
{
    [Inject]
    public IWorldCupService WorldCupService { get; set; }

    [Parameter]
    public IEnumerable<IWcBettingItem<ITeam>> BettingItems { get; set; }

    IBettingResultTable<IWcBettingItem<ITeam>> BettingResult { get; set; }

    public BettingResultComponent()
    {
    }

    private Task BettingItemsUpdated()
    {
        if (BettingItems != null)
        {
            BettingResult = new BettingResultTable<IWcBettingItem<ITeam>>(BettingItems);
        }
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
