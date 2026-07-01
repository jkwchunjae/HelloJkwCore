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
            Dictionary<string, int> teamOrderDict = TeamOrder
                .Where(team => team?.Id != null)
                .Select((team, index) => new { team.Id, Index = index })
                .ToDictionary(x => x.Id, x => x.Index);
            return teams
                .OrderBy(team => teamOrderDict.ContainsKey(team.Id) ? teamOrderDict[team.Id] : int.MaxValue);
        }
        else
        {
            return teams;
        }
    }
}
