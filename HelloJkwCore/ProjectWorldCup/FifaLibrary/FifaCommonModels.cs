namespace ProjectWorldCup.FifaLibrary;

public class FifaDataRoot<T> where T : class
{
    [JsonPropertyName("ContinuationToken")] public object ContinuationToken { get; set; }
    [JsonPropertyName("ContinuationHash")] public object ContinuationHash { get; set; }
    [JsonPropertyName("Results")] public IEnumerable<T> Results { get; set; }
}

public class FifaIdName
{
    [JsonPropertyName("Locale")] public string Locale { get; set; }
    [JsonPropertyName("Description")] public string Description { get; set; }
}


public enum Gender
{
    Men,
    Women,
}

public class OverviewTeam
{
    [JsonPropertyName("placement")]
    public string Placement { get; set; }
    [JsonPropertyName("sourceId")]
    public string Id { get; set; }
    [JsonPropertyName("sourceCategory")]
    public string Name { get; set; }
    [JsonPropertyName("flagOrLogoUrl")]
    public string Flag { get; set; }
}

public class OverviewGroup
{
    [JsonPropertyName("groupTitle")]
    public string GroupName { get; set; }
    [JsonPropertyName("teams")]
    public List<OverviewTeam> Teams { get; set; }

    [JsonIgnore]
    public string Name => GroupName;
}

public class OverviewGroupDataRoot
{
    [JsonPropertyName("groups")]
    public List<OverviewGroup> Groups { get; set; }
}


public class TeamFlag
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("src")]
    public string Src { get; set; }
    [JsonPropertyName("alt")]
    public string Alt { get; set; }
}


