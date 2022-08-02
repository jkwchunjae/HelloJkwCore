namespace ProjectWorldCup;

public partial class WorldCupService
{
    public async Task<List<SimpleGroup>> GetSimpleGroupsAsync()
    {
        var groups = await _fifa.GetGroupOverview();
        return groups
            .Select(group => new SimpleGroup(group))
            .ToList();
    }

    //public async Task<List<WcGroup>> GetGroupsAsync()
    //{
    //    var fifamatches = await _fifa.GetGroupStageMatchesAsync();

    //    var teams = fifamatches
    //        .SelectMany(x => GetTeamInfoFromFifaMatchData(x))
    //        .GroupBy(x => x.Id)
    //        .Select(x => x.First())
    //        .OrderBy(x => x.Placeholder)
    //        .ToList();

    //    var matches = fifamatches
    //        .Select(matchData => GroupMatch.CreateFromFifaMatchData(matchData, teams))
    //        .ToList();

    //    var groups = matches
    //        .GroupBy(x => x.GroupName)
    //        .Select(matches =>
    //        {
    //            var league = new WcGroup
    //            {
    //                Name = matches.Key,
    //            };

    //            var teams = matches.SelectMany(match => new[] { match.HomeTeam, match.AwayTeam })
    //                .Distinct()
    //                .OrderBy(team => team.Placeholder)
    //                .ToList();

    //            teams.ForEach(team => league.AddTeam(team));
    //            matches.ForEach(match => league.AddMatch(match));

    //            return league;
    //        })
    //        .ToList();

    //    return groups;
    //}

    //public async Task<List<GroupMatch>> GetGroupStageMatchesAsync()
    //{
    //    var groups = await GetGroupsAsync();

    //    var matches = groups.SelectMany(group => group.Matches).ToList();

    //    return matches;
    //}

    //private IEnumerable<GroupTeam> GetTeamInfoFromFifaMatchData(FifaMatchData matchData)
    //{
    //    var homeTeam = matchData.Home;
    //    var awayTeam = matchData.Away;

    //    yield return GroupTeam.CreateFromFifaMatchTeam(homeTeam, matchData.GroupName, matchData.PlaceholderA);
    //    yield return GroupTeam.CreateFromFifaMatchTeam(awayTeam, matchData.GroupName, matchData.PlaceholderB);
    //}
}
