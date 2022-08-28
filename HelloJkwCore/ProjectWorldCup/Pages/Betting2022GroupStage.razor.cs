using Microsoft.JSInterop;

namespace ProjectWorldCup.Pages;

public partial class Betting2022GroupStage : JkwPageBase
{
    [Inject]
    private IWorldCupService WcService { get; set; }
    [Inject]
    private IBettingService BettingService { get; set; }
    [Inject]
    private IBettingGroupStageService GroupStageService { get; set; }

    private List<WcGroup> Groups { get; set; } = new();

    private BettingUser BettingUser { get; set; }
    private WcBettingItem<GroupTeam> BettingItem { get; set; } = new();

    protected override async Task OnPageInitializedAsync()
    {
        Groups = await WcService.GetGroupsAsync();
        BettingUser = await BettingService.GetBettingUserAsync(User);

        if (IsAuthenticated && (BettingUser?.JoinedBetting?.Contains(BettingType.GroupStage) ?? false))
        {
            BettingItem = await GroupStageService.GetBettingAsync(BettingUser);
        }
    }

    private async Task PickTeam(GroupTeam team)
    {
        var buttonType = GetButtonType(team);

        if (buttonType == TeamButtonType.Pickable)
        {
            BettingItem = await GroupStageService.PickTeamAsync(BettingUser, team);
            StateHasChanged();
        }
    }

    private async Task UnpickTeam(GroupTeam team)
    {
        var buttonType = GetButtonType(team);

        if (buttonType == TeamButtonType.Picked)
        {
            BettingItem = await GroupStageService.UnpickTeamAsync(BettingUser, team);
            StateHasChanged();
        }
    }

    private TeamButtonType GetButtonType(GroupTeam team)
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
