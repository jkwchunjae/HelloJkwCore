namespace ProjectWorldCup.FifaLibrary;

public partial class Fifa : IFifa
{
    public async Task<List<OverviewGroup>> GetGroupOverview()
    {
        try
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
        catch
        {
            return await GetFailoverData<List<OverviewGroup>>("GroupOverview.json");
        }
    }

    public async Task<List<FifaStandingData>> GetStandingDataAsync()
    {
        try
        {
            var cacheTime = DateTime.Now.ToString("yyyyMMdd.HHmm").Left(12); // 분의 앞자리만 쓴다. 10분에 한 번 캐시
            return await GetFromCacheOrAsync<List<FifaStandingData>>($"{nameof(GetStandingDataAsync)}_{cacheTime}0", async () =>
            {
                var url = $"https://api.fifa.com/api/v3/calendar/17/255711/285063/standing?language=en";
                var res = await _httpClient.GetAsync(url);
                var text = await res.Content.ReadAsStringAsync();
                text = text.Replace("{format}", "sq").Replace("{size}", "2");
                var dataRoot = Json.Deserialize<FifaDataRoot<FifaStandingData>>(text);

                if (dataRoot?.Results?.Any() ?? false)
                {
                    return dataRoot.Results
                        .OrderBy(x => x.Group[0].Description)
                        .ThenBy(x => x.Position)
                        .ToList();
                }
                else
                {
                    throw new Exception();
                }
            });
        }
        catch
        {
            return await GetFailoverData<List<FifaStandingData>>("StandingData.json");
        }
    }
}
