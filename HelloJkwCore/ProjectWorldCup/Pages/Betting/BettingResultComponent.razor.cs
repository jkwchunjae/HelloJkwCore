using Microsoft.AspNetCore.Identity;

namespace ProjectWorldCup.Pages.Betting;

public class CommonBettingItem : BettingResultItem
{
    public AppUser User { get; init; }
}

public partial class BettingResultComponent : JkwPageBase
{
    [Inject]
    UserManager<AppUser> UserManager { get; set; }
    BettingResultTable<CommonBettingItem> BettingResult { get; set; }


    public BettingResultComponent()
    {
        BettingResult = new BettingResultTable<CommonBettingItem>(new List<CommonBettingItem>());
    }

    protected override async Task OnPageInitializedAsync()
    {
        var users = (await UserManager.GetUsersInRoleAsync("all")).ToList();

        var list = new List<CommonBettingItem>();
        var scores = new[] { 3, 7, 1, 2, 8 };
        var index = 0;
        foreach (var user in users.Concat(users))
        {
            if (user != null)
            {
                list.Add(new CommonBettingItem
                {
                    User = user,
                    Id = User.Id.ToString(),
                    Score = scores[index % scores.Length],
                });
            }
            index++;
        }

        BettingResult = new BettingResultTable<CommonBettingItem>(list);
    }
}
