namespace ProjectWorldCup;

public partial class Fifa : IFifa
{
    private readonly string GroupStageMatchesCacheKey = nameof(GroupStageMatchesCacheKey);
    private readonly string KnockoutStageMatchesCacheKey = nameof(KnockoutStageMatchesCacheKey);

    public async Task<List<FifaMatchData>> GetGroupStageMatchesAsync()
    {
        return await GetFromCacheOrAsync<List<FifaMatchData>>(GroupStageMatchesCacheKey, async () =>
        {
            var url = "https://www.fifa.com/tournaments/mens/worldcup/qatar2022/match-center";
            var pageData = await GetPageData(new Uri(url));
            var groupPhaseMatches = pageData["matchesOverviewProps"]["groupPhaseMatches"].ToArray();

            return groupPhaseMatches
                ?.SelectMany(x => x["matches"].Select(e => e.ToObject<FifaMatchData>()).ToList())
                .Where(x => x.PlaceholderA != null)
                .ToList() ?? new();
        });
    }

    public async Task<List<FifaMatchData>> GetKnockoutStageMatchesAsync()
    {
        return await GetFromCacheOrAsync<List<FifaMatchData>>(KnockoutStageMatchesCacheKey, async () =>
        {
            var url = "https://www.fifa.com/tournaments/mens/worldcup/qatar2022/match-center";
            var pageData = await GetPageData(new Uri(url));
            var knockoutStageMatches = pageData["matchesOverviewProps"]["knockoutPhaseMatches"].ToArray();

            return knockoutStageMatches
                ?.SelectMany(x => x["matches"].Select(e => e.ToObject<FifaMatchData>()).ToList())
                .ToList() ?? new();
        });
    }
}