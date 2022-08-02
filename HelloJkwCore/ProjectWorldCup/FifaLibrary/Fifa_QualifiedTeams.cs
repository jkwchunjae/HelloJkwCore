namespace ProjectWorldCup.FifaLibrary;

public class QualifiedTeam
{
    public string Id { get; set; }
    public string Name { get; set; }
    public TeamFlag Flag { get; set; }
    public string Status { get; set; }
    public RankingTeamData Ranking { get; set; }

    public int Rank => Ranking?.RankingItem?.Rank ?? 0;
}

public partial class Fifa : IFifa
{
    private readonly string QualifiedTeamCacheKey = nameof(QualifiedTeamCacheKey);

    public async Task<List<QualifiedTeam>> GetQualifiedTeamsAsync()
    {
        return await GetFromCacheOrAsync<List<QualifiedTeam>>(QualifiedTeamCacheKey, async () =>
        {
            var url = @"https://www.fifa.com/tournaments/mens/worldcup/qatar2022/qualifiers";
            var pageData = await GetPageData(new Uri(url));
            var list = pageData?["tournamentQualificationTeamsProps"]?["teams"]?.AsEnumerable() ?? null;

            var result = list?
                .Select(x => x.ToObject<QualifiedTeam>())
                .Where(x => x != null)
                .Where(x => x?.Status == "Q")
                .ToList();

            return result ?? new();
        });
    }
}