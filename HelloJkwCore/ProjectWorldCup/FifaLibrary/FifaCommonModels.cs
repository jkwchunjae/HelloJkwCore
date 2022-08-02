namespace ProjectWorldCup.FifaLibrary;

public enum Gender
{
    Men,
    Women,
}

public class OverviewTeam
{
    [JsonProperty("groupPlacement")]
    public string Placement { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("flag")]
    public TeamFlag Flag { get; set; }
}

public class OverviewGroup
{
    [JsonProperty("groupTitle")]
    public string GroupName { get; set; }
    [JsonProperty("teams")]
    public List<OverviewTeam> Teams { get; set; }

    [JsonIgnore]
    public string Name => GroupName;
}

public class OverviewGroupDataRoot
{
    [JsonProperty("groups")]
    public List<OverviewGroup> Groups { get; set; }
}


public class TeamFlag
{
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("src")]
    public string Src { get; set; }
    [JsonProperty("alt")]
    public string Alt { get; set; }
}

public class TeamTag
{
    public string Id { get; set; }
    public string Text { get; set; }
}