namespace ProjectSuFc;

[JsonConverter(typeof(StringIdJsonConverter<SeasonId>))]
public class SeasonId : StringId
{
}

public class Season
{
    public SeasonId Id { get; set; }
    public string Name { get; set; }
}