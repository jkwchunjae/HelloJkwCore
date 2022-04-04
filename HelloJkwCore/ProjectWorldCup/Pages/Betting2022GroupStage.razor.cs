namespace ProjectWorldCup.Pages;

public partial class Betting2022GroupStage : JkwPageBase
{
    [Inject]
    private IWorldCupService Service { get; set; }
    [Inject]
    private IBettingService BettingService { get; set; }

    private List<WcGroup> Groups { get; set; } = new();

    private WcBettingItem BettingItem { get; set; } = new();

    protected override async Task OnPageInitializedAsync()
    {
        Groups = await Service.GetGroupsAsync();

        if (IsAuthenticated)
        {
            BettingItem = await BettingService.GetBettingItemAsync(BettingType.GroupStage, User);
        }
    }

    private async Task PickTeam(Team team)
    {
        var buttonType = GetButtonType(team);

        if (buttonType == TeamButtonType.Pickable)
        {
            BettingItem.Picked.Add(team);
            await BettingService.SaveBettingItemAsync(BettingType.GroupStage, BettingItem);
        }
    }

    private async Task UnpickTeam(Team team)
    {
        var buttonType = GetButtonType(team);

        if (buttonType == TeamButtonType.Picked)
        {
            var removeTeam = BettingItem.Picked.Find(x => x.Id == team.Id);
            if (removeTeam != null)
            {
                BettingItem.Picked.Remove(removeTeam);
                await BettingService.SaveBettingItemAsync(BettingType.GroupStage, BettingItem);
            }
        }
    }

    private TeamButtonType GetButtonType(Team team)
    {
        if (BettingItem.Picked.Any(x => x.Id == team.Id))
        {
            return TeamButtonType.Picked;
        }

        var groupTeams = Groups.First(g => g.Teams.Any(t => t.Id == team.Id));
        var groupPickCount = groupTeams.Teams.Count(t => BettingItem.Picked.Any(x => x.Id == t.Id));

        if (groupPickCount == 2)
        {
            return TeamButtonType.Disabled;
        }
        else // 0 or 1
        {
            return TeamButtonType.Pickable;
        }
    }
}
