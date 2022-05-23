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

    private async Task BettingItemsUpdated()
    {
        var groups = await WorldCupService.GetGroupsAsync();
        var team16 = groups.SelectMany(group => group.Stands.Take(2))
            .Select(s => s.Team)
            .ToList();
        BettingItems.ForEach(bettingItem => bettingItem.Fixed = team16);
        BettingResult = new BettingResultTable<WcBettingItem<GroupTeam>>(BettingItems);
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
