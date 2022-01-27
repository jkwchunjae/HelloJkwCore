<Query Kind="Program">
  <Output>DataGrids</Output>
  <Reference Relative="..\bin\Debug\net6.0\Common.dll">D:\jkw\major\GitHub\HelloJkwCore\HelloJkwCore\ProjectWorldCup\bin\Debug\net6.0\Common.dll</Reference>
  <Reference Relative="..\bin\Debug\net6.0\ProjectWorldCup.dll">D:\jkw\major\GitHub\HelloJkwCore\HelloJkwCore\ProjectWorldCup\bin\Debug\net6.0\ProjectWorldCup.dll</Reference>
  <NuGetReference>Jkw.Extensions</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Common</Namespace>
  <Namespace>JkwExtensions</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>ProjectWorldCup</Namespace>
  <Namespace>ProjectWorldCup.Pages</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
    var fifa = new Fifa2();

    var matches = await fifa.GetGroupStageMatches2Async();
    
    matches.Dump();
}

public class Fifa2: Fifa
{
    public async Task<List<FifaMatchData>> GetGroupStageMatches2Async()
    {
        return await GetFromCacheOrAsync<List<FifaMatchData>>("GroupStageMatchesCacheKey", async () =>
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
}