namespace ProjectWorldCup.Pages.Wc2026;

public static class Wc2026ScenarioBettingSimulator
{
    public static List<IWcBettingItem<ITeam>> CreateSimulationItems(
        IEnumerable<WcBettingItem<GroupTeam>> bettingItems,
        IEnumerable<Wc2026ScenarioGroup> groups)
    {
        var fixedTeams = CreateFixedTeams(groups);

        return (bettingItems ?? Enumerable.Empty<WcBettingItem<GroupTeam>>())
            .Where(item => item?.User != null)
            .Select(item => (IWcBettingItem<ITeam>)new WcBettingItem<GroupTeam>
            {
                User = item.User,
                IsRandom = item.IsRandom,
                IsAi = item.IsAi,
                Picked = (item.Picked ?? Enumerable.Empty<GroupTeam>())
                    .Select(CloneTeam)
                    .ToList(),
                Fixed = fixedTeams
                    .Select(CloneTeam)
                    .ToList(),
            })
            .ToList();
    }

    public static List<GroupTeam> CreateFixedTeams(IEnumerable<Wc2026ScenarioGroup> groups)
    {
        return (groups ?? Enumerable.Empty<Wc2026ScenarioGroup>())
            .OrderBy(group => group.SortOrder)
            .ThenBy(group => group.Name)
            .SelectMany(group => group.Standings
                .Take(2)
                .Select((standing, index) => ToGroupTeam(group, standing.Team, index + 1)))
            .ToList();
    }

    private static GroupTeam ToGroupTeam(
        Wc2026ScenarioGroup group,
        Wc2026ScenarioTeam team,
        int placement)
    {
        return new GroupTeam
        {
            Id = string.IsNullOrWhiteSpace(team.BettingTeamId) ? team.Id : team.BettingTeamId,
            FifaTeamId = team.Id,
            GroupName = group.Name,
            Placement = placement.ToString(),
            Name = team.Name,
            Flag = team.Flag,
        };
    }

    private static GroupTeam CloneTeam(GroupTeam team)
    {
        return new GroupTeam
        {
            Id = team.Id,
            FifaTeamId = team.FifaTeamId,
            GroupName = team.GroupName,
            Placement = team.Placement,
            Name = team.Name,
            Flag = team.Flag,
        };
    }
}
