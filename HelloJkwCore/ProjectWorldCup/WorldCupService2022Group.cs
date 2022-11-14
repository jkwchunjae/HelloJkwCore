namespace ProjectWorldCup;

public partial class WorldCupService
{
    public async Task<List<SimpleGroup>> GetSimpleGroupsAsync()
    {
        var standings = await _fifa.GetStandingDataAsync();
        var groups = await _fifa.GetGroupOverview();
        var simpleGroups = groups
            .Select(group => new SimpleGroup(group))
            .ToList();
        foreach (var group in simpleGroups)
        {
            foreach (var team in group.Teams)
            {
                var standingTeam = standings.First(x => x.Team.IdCountry == team.Id);
                team.Name = standingTeam.Team.Name[0].Description;
            }
        }

        return simpleGroups;
    }

    /// <summary>
    /// 순위정보를 반영함
    /// </summary>
    public async Task<List<WcGroup>> GetGroupsAsync()
    {
        var simpleGroups = await GetSimpleGroupsAsync();

        var groups = simpleGroups
            .Select(sg =>
            {
                var groupName = sg.Name;
                var league = new WcGroup
                {
                    Name = groupName,
                };
                var teams = sg.Teams
                    .OrderBy(t => t.Placement)
                    .Select(team =>
                    {
                        var newTeam = new GroupTeam
                        {
                            Id = team.Flag.Substring(team.Flag.LastIndexOf('/') + 1),
                            Placement = team.Placement,
                            GroupName = groupName,
                            Name = team.Name,
                            Flag = team.Flag,
                        };
                        return newTeam;
                    })
                    .ToList();

                teams.ForEach(team => league.AddTeam(team));

                return league;
            })
            .ToList();

        var fifaStandings = await _fifa.GetStandingDataAsync();
        groups.ForEach(group => group.WriteStanding(fifaStandings));

        return groups;
    }
}
