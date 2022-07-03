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
        var matches = await LeagueUpdator.CreateMatches();
        var newMatches = matches
            .Select(match =>
            {
                var prevMatch = League.MatchList?.FirstOrDefault(m => m?.Id == match?.Id);

                if (prevMatch == null)
                    return match;

                if (prevMatch.Winner != null)
                    return prevMatch;

                return match;
            })
            .ToList();

        var newLeagueData = await Service!.UpdateLeagueAsync(League!.Id, league =>
        {
            league.MatchIdList = newMatches.Select(x => x.Id).ToList();
            league.MatchList = newMatches;

            return league;
        });

        return newLeagueData;
    }
}
