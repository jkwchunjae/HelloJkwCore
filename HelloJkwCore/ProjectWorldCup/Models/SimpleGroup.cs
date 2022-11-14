namespace ProjectWorldCup;

public class SimpleTeam
{
    public string Placement { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string Flag { get; set; }

    public SimpleTeam() { }
    public SimpleTeam(OverviewTeam team)
    {
        Placement = team.Placement;
        Id = team.Id;
        Name = team.Name;
        Flag = team.Flag;
    }
}

public class SimpleGroup
{
    public string GroupName { get; set; }
    public List<SimpleTeam> Teams { get; set; }

    [JsonIgnore]
    public string Name => GroupName;

    public SimpleGroup() { }
    public SimpleGroup(OverviewGroup group)
    {
        GroupName = group.GroupName;
        Teams = group.Teams
            .Select(team => new SimpleTeam(team))
            .ToList();
    }
}

