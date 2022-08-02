namespace ProjectWorldCup.FifaLibrary;

public partial class Fifa : IFifa
{
    public async Task<List<OverviewGroup>> GetGroupOverview()
    {
        return await GetFromCacheOrAsync<List<OverviewGroup>>(nameof(GetGroupOverview), async () =>
        {
            var url = "https://cxm-api.fifa.com/fifaplusweb/api/sections/tournamentGroupOverview/15xDmWzQAu51lEIjP4fVfz?locale=en";
            var res = await _httpClient.GetAsync(url);
            var text = await res.Content.ReadAsStringAsync();
            var dataRoot = Json.Deserialize<OverviewGroupDataRoot>(text);
            foreach (var group in dataRoot.Groups)
            {
                group.Teams = group.Teams
                    .OrderBy(t => t.Placement)
                    .ToList();
            }
            return dataRoot.Groups;
        });
    }
}
