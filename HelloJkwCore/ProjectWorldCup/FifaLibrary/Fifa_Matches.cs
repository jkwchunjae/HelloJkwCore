namespace ProjectWorldCup;

public partial class Fifa : IFifa
{
    private readonly string GroupStageMatchesCacheKey = nameof(GroupStageMatchesCacheKey);
    private readonly string KnockoutStageMatchesCacheKey = nameof(KnockoutStageMatchesCacheKey);

    public async Task<List<FifaMatchData>> GetGroupStageMatchesAsync()
    {
        return await GetFromCacheOrAsync<List<FifaMatchData>>(GroupStageMatchesCacheKey, async () =>
        {
            var url = "https://www.fifa.com/tournaments/mens/worldcup/qatar2022";
            var pageData = await GetPageData(new Uri(url));

            var contents = pageData?["content"]?.ToArray();

            var groupPhaseMatches = contents
                .Select(content => content["groupPhaseMatches"])
                .FirstOrDefault(content => content != null);

            return groupPhaseMatches
                ?.SelectMany(x => x["matches"].Select(e => e.ToObject<FifaMatchData>()).ToList())
                .ToList() ?? new();
        });
    }

    public async Task<List<FifaMatchData>> GetKnockoutStageMatchesAsync()
    {
        return await GetFromCacheOrAsync<List<FifaMatchData>>(KnockoutStageMatchesCacheKey, async () =>
        {
            var url = "https://www.fifa.com/tournaments/mens/worldcup/qatar2022";
            var pageData = await GetPageData(new Uri(url));

            var contents = pageData?["content"]?.ToArray();

            var knockoutStageMatches = contents
                .Select(content => content["knockoutPhaseMatches"])
                .FirstOrDefault(content => content != null);

            return knockoutStageMatches
                ?.SelectMany(x => x["matches"].Select(e => e.ToObject<FifaMatchData>()).ToList())
                .ToList() ?? new();
        });
    }
}