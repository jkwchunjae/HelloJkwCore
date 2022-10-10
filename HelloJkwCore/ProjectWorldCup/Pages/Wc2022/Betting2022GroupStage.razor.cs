using Microsoft.JSInterop;

namespace ProjectWorldCup.Pages.Wc2022;

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
    private List<WcBettingItem<GroupTeam>> BettingItems { get; set; }

    bool TimeOver { get; set; } = false;

    protected override async Task OnPageInitializedAsync()
    {
        Groups = await WcService.GetGroupsAsync();
        BettingUser = await BettingService.GetBettingUserAsync(User);

        if (BettingUser?.JoinStatus != UserJoinStatus.Joined)
        {
            Navi.NavigateTo("/worldcup");
            return;
        }

        if (IsAuthenticated && (BettingUser?.JoinedBetting?.Contains(BettingType.GroupStage) ?? false))
        {
            BettingItem = await GroupStageService.GetBettingAsync(BettingUser);
        }

        BettingItems = await GroupStageService.GetAllBettingsAsync();
    }

    private async Task PickTeam(GroupTeam team)
    {
        if (TimeOver)
            return;

        var bettingUser = await BettingService.GetBettingUserAsync(User);
        if (bettingUser.JoinedBetting.Empty(x => x == BettingType.GroupStage))
        {
            bettingUser = await BettingService.JoinBettingAsync(bettingUser, BettingType.GroupStage);
        }
        if (bettingUser.JoinedBetting.Empty(x => x == BettingType.GroupStage))
        {
            // 참가할 수 없는 경우
            return;
        }

        var buttonType = GetButtonType(team);

        if (buttonType == TeamButtonType.Pickable)
        {
            BettingItem = await GroupStageService.PickTeamAsync(BettingUser, team);
            StateHasChanged();
        }
    }

    private async Task UnpickTeam(GroupTeam team)
    {
        if (TimeOver)
            return;

        var buttonType = GetButtonType(team);

        if (buttonType == TeamButtonType.Picked)
        {
            BettingItem = await GroupStageService.UnpickTeamAsync(BettingUser, team);
            StateHasChanged();
        }
    }

    private TeamButtonType GetButtonType(GroupTeam team)
    {
        if (BettingItem.Picked.Any(x => x == team))
        {
            return TeamButtonType.Picked;
        }

        var groupTeams = Groups.First(g => g.Teams.Any(t => t == team));
        var groupPickCount = groupTeams.Teams.Count(t => BettingItem.Picked.Any(x => x == t));

        if (groupPickCount == 2)
        {
            return TeamButtonType.Disabled;
        }
        else // 0 or 1
        {
            return TeamButtonType.Pickable;
        }
    }

    private void OnTimeOver()
    {
        TimeOver = true;
        StateHasChanged();
    }
}
