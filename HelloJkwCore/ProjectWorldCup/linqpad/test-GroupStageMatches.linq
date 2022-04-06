<Query Kind="Program">
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
	
    var fifamatches = await fifa.GetGroupStageMatches2Async();
    
	// fifamatches.Dump();
	//
	//fifa.GetGroups2Async(fifamatches).Dump();
}

public class Fifa2 : Fifa
{
	public async Task<List<FifaMatchData>> GetGroupStageMatches2Async()
	{
		return await GetFromCacheOrAsync<List<FifaMatchData>>("GroupStageMatchesCacheKey", async () =>
        {
            var url = "https://www.fifa.com/tournaments/mens/worldcup/qatar2022/match-center";
            var pageData = await GetPageData(new Uri(url));
			
            var groupPhaseMatches = pageData["matchesOverviewProps"]["groupPhaseMatches"].ToArray();

            return groupPhaseMatches
                ?.SelectMany(x => x["matches"].Select(e => e.ToObject<FifaMatchData>()).ToList())
                .ToList() ?? new();
        });
    }

	public List<WcGroup> GetGroups2Async(List<FifaMatchData> fifamatches)
	{
		var teams = fifamatches
			.SelectMany(x => GetTeamInfoFromFifaMatchData(x))
			.GroupBy(x => x.Id)
			.Select(x => x.First())
			.OrderBy(x => x.Placeholder)
			.ToList();

		var matches = fifamatches
			.Select(m => GetMatchFromFifaMatchData(m, teams))
			.ToList();

        var groups = matches
            .GroupBy(x => x.GroupName)
            .Select(matches =>
            {
                var league = new WcGroup
                {
                    Name = matches.Key,
                };

                var teams = matches.SelectMany(match => new[] { match.HomeTeam, match.AwayTeam })
                    .Distinct()
                    .OrderBy(team => team.Placeholder)
                    .ToList();

				teams.ForEach(team => league.AddTeam(team));
				matches.ForEach(match => league.AddMatch(match));

				return league;
			})
			.ToList();

		return groups;
	}


	private IEnumerable<GroupTeam> GetTeamInfoFromFifaMatchData(FifaMatchData matchData)
	{
		var homeTeam = matchData.HomeTeam;
		var awayTeam = matchData.AwayTeam;

		var home = new GroupTeam
		{
			GroupName = GetGroupName(matchData.PlaceholderA),
			Placeholder = GetPlaceholder(matchData.PlaceholderA),
			Id = homeTeam?.Abbreviation ?? matchData.PlaceholderA,
			Name = homeTeam?.TeamName ?? matchData.PlaceholderA,
			Flag = homeTeam?.PictureUrl,
		};

		var away = new GroupTeam
		{
			GroupName = GetGroupName(matchData.PlaceholderB),
			Placeholder = GetPlaceholder(matchData.PlaceholderB),
			Id = awayTeam?.Abbreviation ?? matchData.PlaceholderB,
			Name = awayTeam?.TeamName ?? matchData.PlaceholderB,
			Flag = awayTeam?.PictureUrl,
		};

		yield return home;
		yield return away;

		string GetPlaceholder(string ph)
		{
			if (ph == "EUR")
				return "B4";
			if (ph == "ICP 1")
				return "D2";
			if (ph == "ICP 2")
				return "E2";
			return ph;
		}

		string GetGroupName(string ph)
		{
			return $"Group{GetPlaceholder(ph).Left(1)}";
		}
	}

	private GroupMatch GetMatchFromFifaMatchData(FifaMatchData matchData, List<GroupTeam> teams)
	{
		var homePlaceholder = GetPlaceholder(matchData.PlaceholderA);
		var awayPlaceholder = GetPlaceholder(matchData.PlaceholderB);

		var homeTeam = teams.First(x => x.Placeholder == homePlaceholder);
		var awayTeam = teams.First(x => x.Placeholder == awayPlaceholder);

		var placeholder = GetPlaceholder(matchData.PlaceholderA);

		var match = new GroupMatch()
		{
			GroupName = homeTeam.GroupName,
			HomeTeam = homeTeam,
			AwayTeam = awayTeam,
			HomeScore = matchData.HomeTeam?.Score ?? 0,
			AwayScore = matchData.AwayTeam?.Score ?? 0,
			Status = MatchStatus.Before,
			Time = matchData.Date,
		};

		return match;

		string GetPlaceholder(string ph)
		{
			if (ph == "EUR")
				return "B4";
			if (ph == "ICP 1")
				return "D2";
			if (ph == "ICP 2")
				return "E2";
			return ph;
		}
	}
}
