namespace ProjectSuFc;

[JsonNetConverter(typeof(StringIdJsonNetConverter<SeasonId>))]
[TextJsonConverter(typeof(StringIdTextJsonConverter<SeasonId>))]
public class SeasonId : StringId
{
}

public class Season
{
    public SeasonId Id { get; set; }
    public string Name { get; set; }
}