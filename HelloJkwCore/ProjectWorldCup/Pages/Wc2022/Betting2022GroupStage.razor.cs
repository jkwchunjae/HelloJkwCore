using Microsoft.JSInterop;

namespace ProjectWorldCup.Pages.Wc2022;

public partial class Betting2022GroupStage : JkwPageBase
{
    [Inject] public ISnackbar Snackbar { get; set; }
    [Inject] private IWorldCupService WcService { get; set; }
    [Inject] private IBettingService BettingService { get; set; }
    [Inject] private IBettingGroupStageService GroupStageService { get; set; }

    private List<WcGroup> Groups { get; set; } = new();

    private BettingUser BettingUser { get; set; }
    private WcBettingItem<GroupTeam> BettingItem { get; set; } = new();
    private List<WcBettingItem<GroupTeam>> BettingItems { get; set; }

    bool TimeOver { get; set; } = false;
    bool CheckRandom1 = false;
    bool CheckRandom2 = false;
    bool CheckRandom3 = false;

    protected override async Task OnPageInitializedAsync()
    {
        if (IsAuthenticated)
        {
            BettingUser = await BettingService.GetBettingUserAsync(User);

            if (BettingUser?.JoinedBetting?.Contains(BettingType.GroupStage) ?? false)
            {
                BettingItem = await GroupStageService.GetBettingAsync(BettingUser);
            }
        }

        Groups = await WcService.GetGroupsAsync();
        BettingItems = await GroupStageService.GetAllBettingsAsync();
    }

    private void ShowLoginRequireMessage()
    {
        Snackbar.Clear();
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
        Snackbar.Add("로그인을 해주세요", Severity.Warning);
    }

    private async Task PickTeam(GroupTeam team)
    {
        if (TimeOver)
            return;
        if (!IsAuthenticated)
        {
            ShowLoginRequireMessage();
            return;
        }
        if (BettingItem?.IsRandom ?? false)
            return;

        try
        {
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
        catch (Exception ex)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add(ex.Message, Severity.Normal);
        }
    }

    private async Task UnpickTeam(GroupTeam team)
    {
        if (TimeOver)
            return;
        if (!IsAuthenticated)
        {
            ShowLoginRequireMessage();
            return;
        }
        if (BettingItem?.IsRandom ?? false)
            return;

        try
        {
            var buttonType = GetButtonType(team);

            if (buttonType == TeamButtonType.Picked)
            {
                BettingItem = await GroupStageService.UnpickTeamAsync(BettingUser, team);
                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add(ex.Message, Severity.Normal);
        }
    }

    private TeamButtonType GetButtonType(GroupTeam team)
    {
        if (BettingItem == null)
        {
            return TeamButtonType.Pickable;
        }

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

    private async Task SelectFullRandom()
    {
        if (TimeOver)
            return;
        if (!IsAuthenticated)
        {
            ShowLoginRequireMessage();
            return;
        }
        if (BettingItem?.IsRandom ?? false)
            return;

        try
        {
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

            BettingItem = await GroupStageService.PickRandomAsync(BettingUser);
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add(ex.Message, Severity.Normal);
        }
    }
}
