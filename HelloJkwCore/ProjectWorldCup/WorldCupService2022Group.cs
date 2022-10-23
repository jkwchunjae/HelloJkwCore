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

    /// <summary>
    /// 순위정보를 반영함
    /// </summary>
    public async Task<List<WcGroup>> GetGroupsAsync()
    {
        var simpleGroups = await GetSimpleGroupsAsync();

        var groups = simpleGroups
            .Select(sg =>
            {
                var groupName = sg.Name.Right(1);
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
