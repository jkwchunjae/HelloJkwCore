namespace ProjectPingpong.Pages.League;

public partial class PpLeagueMatchList : JkwPageBase
{
    [Parameter] public LeagueData League { get; set; }
    [Parameter] public EventCallback<LeagueData> LeagueChanged { get; set; }

    [Inject] public IPpService? Service { get; set; }
    [Inject] public IPpMatchService? MatchService { get; set; }

    LeagueUpdator LeagueUpdator;

    protected override async Task OnPageParametersSetAsync()
    {
        LeagueUpdator = new LeagueUpdator(League, Service!, MatchService!);

        League = await UpdateLeagueMatch();
    }

    private async Task<LeagueData> UpdateLeagueMatch()
    {
        var newMatches = LeagueUpdator.CreateMatches();
        var merged = newMatches
            .Select(match =>
            {
                var existMatch = League!.MatchList?.FirstOrDefault(m => m.Id == match.Id);
                return existMatch ?? match;
            })
            .ToList();
        foreach (var match in merged)
        {
            if (League!.MatchList?.Any(m => m.Id == match.Id) ?? false)
            {
                // 기존에 매치가 있는 경우. // 그냥 넘어가자.
            }
            else
            {
                // 기존에 매치가 없는 경우. 파일을 만들자.
                await MatchService!.CreateMatchAsync(match.Id, match);
            }
        }

        var newLeagueData = await Service!.UpdateLeagueAsync(League!.Id, league =>
        {
            league.MatchIdList = merged.Select(x => x.Id).ToList();
            league.MatchList = merged;

            return league;
        });

        return newLeagueData!;
    }
}
