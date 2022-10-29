namespace ProjectWorldCup.Pages.User;

public partial class WcTotalResult : JkwPageBase
{
    [Inject] IBettingService BettingService { get; set; }

    class UserResult
    {
        public int Rank;
        public string Name;
        public long Reward1;
        public long Reward2;
        public long Reward3;
        public long Total => Reward1 + Reward2 + Reward3;
    }
    IEnumerable<UserResult> Results { get; set; } = new List<UserResult>();

    protected override async Task OnPageInitializedAsync()
    {
        if (!IsAuthenticated)
        {
            Navi.NavigateTo("/worldcup");
            return;
        }
        var users = await BettingService.GetBettingUsersAsync();
        Results = MakeUserResult(users);
    }

    IEnumerable<UserResult> MakeUserResult(IEnumerable<BettingUser> bettingUser)
    {
        var results = bettingUser
            .Select(user => new UserResult
            {
                Name = user.AppUser.DisplayName,
                Reward1 = GetValue(user, HistoryType.Reward1),
                Reward2 = GetValue(user, HistoryType.Reward2),
                Reward3 = GetValue(user, HistoryType.Reward3),
            })
            .Reduce(new List<UserResult>(), (list, userResult, index, source) =>
            {
                var rank = source.Count(x => x.Total > userResult.Total) + 1;
                userResult.Rank = rank;
                list.Add(userResult);
                return list;
            })
            .OrderBy(x => x.Rank)
            .ToList();

        return results;
    }

    private long GetValue(BettingUser user, HistoryType historyType)
    {
        var value = user.BettingHistories
            ?.FirstOrDefault(x => x.Type == historyType)
            ?.Value;

        return value ?? 0;
    }
}
