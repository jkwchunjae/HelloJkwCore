using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectWorldCup.Pages.Wc2026;

public partial class Betting2026GroupStage : JkwPageBase
{
    [Inject] public ISnackbar Snackbar { get; set; }
    [Inject] private IWorldCupService WcService { get; set; }
    [Inject] private IBettingService BettingService { get; set; }
    [Inject(Key = "2026")] private IBettingGroupStageService GroupStageService { get; set; }
    [Inject] private UserManager<AppUser> UserManager { get; set; }

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

        Groups = await WcService.GetGroupsFromStandingAsync();
        var bettingItems = await GroupStageService.GetAllBettingsAsync();
        var users = (await UserManager.GetUsersInRoleAsync("all"))
            .ToDictionary(user => user.Id);
        foreach (var item in bettingItems.Where(item => users.ContainsKey(item.User.Id)))
        {
            item.User = users[item.User.Id];
        }
        BettingItems = bettingItems;
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
                return;
            }

            var buttonType = GetButtonType(team);

            if (buttonType == TeamButtonType.Pickable)
            {
                BettingItem = await GroupStageService.PickTeamAsync(BettingUser, team);
                StateHasChanged();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Configuration.ShowTransitionDuration = 100;
                Snackbar.Configuration.VisibleStateDuration = 3000;
                Snackbar.Configuration.HideTransitionDuration = 100;
                Snackbar.Configuration.MaxDisplayedSnackbars = 3;
                Snackbar.Add($"{team.Name} 팀을 선택하였습니다. (저장되었습니다)", Severity.Success);
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
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add($"{team.Name} 팀을 제외했습니다. (저장되었습니다)", Severity.Success);
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

        var groupTeams = Groups.FirstOrDefault(g => g.Teams.Any(t => t == team));
        if (groupTeams == null)
            return TeamButtonType.Pickable;

        var groupPickCount = groupTeams.Teams.Count(t => BettingItem.Picked.Any(x => x == t));

        if (groupPickCount == 2)
        {
            return TeamButtonType.Disabled;
        }
        else
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
