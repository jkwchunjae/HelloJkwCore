using Microsoft.AspNetCore.Identity;

namespace ProjectWorldCup.Pages.Betting;

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
