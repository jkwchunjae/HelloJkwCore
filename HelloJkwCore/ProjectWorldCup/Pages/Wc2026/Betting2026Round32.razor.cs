using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectWorldCup.Pages.Wc2026;

public partial class Betting2026Round32 : JkwPageBase
{
    [Inject] public ISnackbar Snackbar { get; set; }
    [Inject] IWorldCupService WorldCupService { get; set; }
    [Inject] IBettingService BettingService { get; set; }
    [Inject(Key = "2026-round32")] private IBettingRound16Service BettingRound32Service { get; set; }
    [Inject] private UserManager<AppUser> UserManager { get; set; }

    List<KnMatch> Round32Matches = new();
    WcBettingItem<Team> BettingItem = new();
    List<WcBettingItem<Team>> BettingItems = null;

    bool AllMatchesAreSetted => Round32Matches.SelectMany(m => m.Teams)
        .All(team => team?.Id != null && team.Id.Length == 3);

    bool[] TimeOver = new bool[16];
    bool CheckRandom1 = false;
    bool CheckRandom2 = false;
    bool CheckRandom3 = false;

    protected override async Task OnPageInitializedAsync()
    {
        Round32Matches = await WorldCupService.GetRound32MatchesAsync();
        if (IsAuthenticated)
        {
            var bettingUser = await BettingService.GetBettingUserAsync(User);
            BettingItem = await BettingRound32Service.GetBettingAsync(bettingUser);
        }
        var bettingItems = await BettingRound32Service.GetAllBettingsAsync();
        var users = (await UserManager.GetUsersInRoleAsync("all"))
            .ToDictionary(user => user.Id);
        foreach (var item in bettingItems.Where(item => users.ContainsKey(item.User.Id)))
        {
            item.User = users[item.User.Id];
        }
        BettingItems = bettingItems;
    }

    private async Task PickTeamAsync(int matchIndex, Team team)
    {
        if (TimeOver[matchIndex])
            return;

        if (team?.Id == null)
            return;

        try
        {
            var bettingUser = await BettingService.GetBettingUserAsync(User);
            if (bettingUser.JoinedBetting.Empty(x => x == BettingType.Round32))
            {
                bettingUser = await BettingService.JoinBettingAsync(bettingUser, BettingType.Round32);
            }
            if (bettingUser.JoinedBetting.Empty(x => x == BettingType.Round32))
            {
                return;
            }
            BettingItem = await BettingRound32Service.PickTeamAsync(bettingUser, team);
            if (BettingItem.Picked.Contains(team))
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add("저장되었습니다", Severity.Success);
            }
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add(ex.Message, Severity.Normal);
        }
    }

    private TeamButtonType GetButtonType(Team team)
    {
        if (team == null)
            return TeamButtonType.Disabled;

        if (BettingItem.Picked.Any(x => x == team))
            return TeamButtonType.Picked;

        var match = Round32Matches.FirstOrDefault(m => m.HomeTeam == team || m.AwayTeam == team);

        if (match == null)
            return TeamButtonType.Disabled;

        if (match.Time < DateTime.UtcNow)
            return TeamButtonType.Disabled;

        return TeamButtonType.Pickable;
    }

    private void OnTimeOver(int index)
    {
        TimeOver[index] = true;
        if (index == 2)
        {
            for (var i = 2; i < 16; i++)
            {
                TimeOver[i] = true;
            }
        }
    }

    private async Task SelectFullRandom()
    {
        if (TimeOver.Any(x => x))
            return;
        if (!AllMatchesAreSetted)
            return;
        if (BettingItem?.IsRandom ?? false)
            return;

        try
        {
            var bettingUser = await BettingService.GetBettingUserAsync(User);
            if (bettingUser.JoinedBetting.Empty(x => x == BettingType.Round32))
            {
                bettingUser = await BettingService.JoinBettingAsync(bettingUser, BettingType.Round32);
            }
            if (bettingUser.JoinedBetting.Empty(x => x == BettingType.Round32))
            {
                return;
            }

            BettingItem = await BettingRound32Service.PickRandomAsync(bettingUser);
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
