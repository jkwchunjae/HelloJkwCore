using Microsoft.AspNetCore.Identity;

namespace ProjectWorldCup.Pages.Betting;

public class WcBettingItem : IBettingResultItem
{
    public AppUser User { get; init; }
    public List<Team> Picked { get; set; }
    public List<Team> Fixed { get; set; }
    public int Reward { get; set; }

    public string Id
    {
        get => User.Id.ToString();
        set { }
    }
    public int Score
    {
        get => Success.Count;
        set { }
    }
    public List<Team> Success => Picked?.Where(s => Fixed?.Contains(s) ?? false).ToList() ?? new();
    public List<Team> Fail => Picked?.Where(s => !(Fixed?.Contains(s) ?? false)).ToList() ?? new();
}

public partial class BettingResultComponent : JkwPageBase
{
    [Inject]
    UserManager<AppUser> UserManager { get; set; }
    [Inject] 
    IWorldCupService WorldCupService { get; set; }

    BettingResultTable<WcBettingItem> BettingResult { get; set; }

    public BettingResultComponent()
    {
        BettingResult = new BettingResultTable<WcBettingItem>(new List<WcBettingItem>());
    }

    protected override async Task OnPageInitializedAsync()
    {
        var users = await UserManager.GetUsersInRoleAsync("all");
        var teams = await WorldCupService.Get2022QualifiedTeamsAsync();
        var @fixed = teams.RandomShuffle().Take(7).ToList();

        var list = users.Concat(users)
            .Where(user => user != null)
            .Select(user => new WcBettingItem
            {
                User = user,
                Id = User.Id.ToString(),
                Picked = teams.RandomShuffle().Take(7).ToList(),
                Fixed = @fixed,
            })
            .ToList();

        BettingResult = new BettingResultTable<WcBettingItem>(list);
    }
}
