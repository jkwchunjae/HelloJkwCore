using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;

namespace ProjectWorldCup.Pages.Betting;

public partial class BettingResultComponent : JkwPageBase
{
    [Inject]
    public IWorldCupService WorldCupService { get; set; }

    [Parameter]
    public IEnumerable<IWcBettingItem<ITeam>> BettingItems { get; set; }

    [Parameter]
    public IEnumerable<ITeam> TeamOrder { get; set; }

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

    private IEnumerable<ITeam> OrderedTeams(IEnumerable<ITeam> teams)
    {
        if (TeamOrder != null)
        {
            Dictionary<string, int> teamOrderDict = TeamOrder.Select((team, index) => new { team.Name, Index = index })
                .ToDictionary(x => x.Name, x => x.Index);
            return teams
                .OrderBy(team => teamOrderDict.ContainsKey(team.Name) ? teamOrderDict[team.Name] : int.MaxValue);
        }
        else
        {
            return teams;
        }
    }
}
